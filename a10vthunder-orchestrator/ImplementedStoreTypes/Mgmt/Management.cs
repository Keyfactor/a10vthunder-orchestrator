// Copyright 2025 Keyfactor
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;
using System.Text;
using System.Linq;
using Keyfactor.Logging;
using Keyfactor.Orchestrators.Common.Enums;
using Keyfactor.Orchestrators.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.OpenSsl;
using Renci.SshNet;
using Keyfactor.Orchestrators.Extensions.Interfaces;
using Renci.SshNet.Common;
using System.Net.Sockets;
using a10vthunder.Api;

namespace Keyfactor.Extensions.Orchestrator.A10vThunder.ThunderMgmt
{
    public class Management : IManagementJobExtension
    {
        private ILogger _logger;

        private readonly IPAMSecretResolver _resolver;

        public Management(IPAMSecretResolver resolver)
        {
            _resolver = resolver;
        }

        public string ExtensionName => String.Empty;
        private string ServerPassword { get; set; }
        private string ServerUserName { get; set; }

        public string ResolvePamField(string name, string value)
        {
            _logger.LogTrace($"Attempting to resolved PAM eligible field {name}");
            return _resolver.Resolve(value);
        }


        public JobResult ProcessJob(ManagementJobConfiguration config)
        {
            _logger = LogHandler.GetClassLogger<Management>();
            _logger.MethodEntry();

            try
            {
                dynamic props = JsonConvert.DeserializeObject(config.CertificateStoreDetails.Properties);

                string host = props.OrchToScpServerIp;
                string username = props.ScpUserName;
                string password = props.ScpPassword;
                string path = config.CertificateStoreDetails.StorePath;
                string filename = config.JobCertificate.Alias;
                int port = props.ScpPort != null ? (int)props.ScpPort : 22;

                string fullPath = $"{path}/{filename}.pem";

                switch (config.OperationType)
                {
                    case CertStoreOperationType.Add:
                        if (!config.Overwrite && RemoteFileExists(host, port, username, password, fullPath))
                        {
                            return Fail(config, "File already exists. Use Overwrite flag to replace it.");
                        }

                        ReplacePemFiles(config, host, port, username, password, fullPath);
                        ReplaceCertAndKeyOnA10(config, props, fullPath);

                        break;

                    case CertStoreOperationType.Remove:
                        RemovePemFiles(config, host, port, username, password, fullPath);
                        break;

                    default:
                        return Fail(config, "Unsupported operation. Only Add, Remove, and Replace are supported.");
                }

                _logger.MethodExit();
                return new JobResult { JobHistoryId = config.JobHistoryId, Result = OrchestratorJobStatusJobResult.Success };
            }
            catch (Exception ex)
            {
                return Fail(config, $"SSH SCP Error: {LogHandler.FlattenException(ex)}");
            }
        }

        private void ReplaceCertAndKeyOnA10(ManagementJobConfiguration config, dynamic props, string fullPath)
        {
            try
            {
                _logger.MethodEntry();
                
                ServerPassword = ResolvePamField("ServerPassword", config.ServerPassword);
                ServerUserName = ResolvePamField("ServerUserName", config.ServerUsername);

                _logger.LogTrace($"config settings: {JsonConvert.SerializeObject(config)}");
                dynamic properties = JsonConvert.DeserializeObject(config.CertificateStoreDetails.Properties);
                _logger.LogTrace($"properties: {JsonConvert.SerializeObject(properties)}");
                var Protocol = properties?.protocol == null || string.IsNullOrEmpty(properties.protocol.Value)
                    ? "https"
                    : properties.protocol.Value;
                var AllowInvalidCert =
                    properties?.allowInvalidCert == null || string.IsNullOrEmpty(properties.allowInvalidCert.Value)
                        ? false
                        : bool.Parse(properties.allowInvalidCert.Value);

                _logger.LogInformation("Calling A10 API to replace cert and key...");

                int lastSlash = fullPath.LastIndexOf('/');
                string basePath = fullPath.Substring(0, lastSlash);
                string fileOnly = Path.GetFileNameWithoutExtension(fullPath);

                string certPath = $"{basePath}/{fileOnly}.crt";
                string keyPath = $"{basePath}/{fileOnly}.key";

                string certUrl = $"scp://{properties?.ScpUserName}:{properties?.ScpPassword}@{properties?.A10ToScpServerIp}:{certPath}";
                string keyUrl = $"scp://{properties?.ScpUserName}:{properties?.ScpPassword}@{properties?.A10ToScpServerIp}:{keyPath}";

                using (var apiClient = new ApiClient(ServerUserName, ServerPassword,
                $"{Protocol}://{config.CertificateStoreDetails.ClientMachine.Trim()}", AllowInvalidCert))
                {
                    apiClient.Logon();
                    apiClient.ReplaceCertificateAndKey(certUrl, keyUrl);
                    apiClient.WriteMemory();
                    apiClient.LogOff();
                }

                _logger.LogInformation("A10 certificate and key replacement successful.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"SCP succeeded but A10 API update failed: {LogHandler.FlattenException(ex)}");
            }
        }


        private void ReplacePemFiles(ManagementJobConfiguration config, string host, int port, string user, string pass, string fullPath)
        {
            _logger.LogInformation($"Uploading certificate and key to {host}");

            try
            {
                var (certPem, keyPem) = GetCertificateAndKey(config);

                int lastSlash = fullPath.LastIndexOf('/');
                string basePath = fullPath.Substring(0, lastSlash);
                string filename = Path.GetFileNameWithoutExtension(fullPath);

                string certPath = $"{basePath}/{filename}.crt";
                string keyPath = $"{basePath}/{filename}.key";

                // Check if files already exist when Overwrite is false
                if (!config.Overwrite)
                {
                    if (RemoteFileExists(host, port, user, pass, certPath) || RemoteFileExists(host, port, user, pass, keyPath))
                    {
                        throw new InvalidOperationException("Certificate or key file already exists. Use Overwrite flag to replace them.");
                    }
                }

                var connectionInfo = new PasswordConnectionInfo(host, port, user, pass)
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };

                connectionInfo.AuthenticationBanner += (sender, args) =>
                {
                    _logger.LogInformation("SSH Banner: " + args.BannerMessage);
                };

                using (var client = new ScpClient(connectionInfo))
                {
                    _logger.LogInformation("Connecting to SCP client...");
                    client.Connect();

                    if (!client.IsConnected)
                    {
                        _logger.LogError("Failed to connect to the SCP server.");
                        return;
                    }

                    using (var certStream = new MemoryStream(Encoding.UTF8.GetBytes(certPem)))
                    using (var keyStream = new MemoryStream(Encoding.UTF8.GetBytes(keyPem)))
                    {
                        _logger.LogInformation($"Uploading certificate to {certPath}...");
                        client.Upload(certStream, certPath);

                        _logger.LogInformation($"Uploading private key to {keyPath}...");
                        client.Upload(keyStream, keyPath);
                    }

                    client.Disconnect();
                    _logger.LogInformation("Upload completed and disconnected.");
                }
            }
            catch (InvalidOperationException invEx)
            {
                _logger.LogError(invEx.Message);
                throw; // Let ProcessJob catch and turn into a JobResult.Failure
            }
            catch (SshAuthenticationException authEx)
            {
                _logger.LogError($"Authentication failed: {authEx.Message}");
            }
            catch (SshConnectionException connEx)
            {
                _logger.LogError($"SSH connection error: {connEx.Message}");
            }
            catch (SocketException sockEx)
            {
                _logger.LogError($"Socket error: {sockEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error during upload: {ex.Message}");
            }
        }


        private void RemovePemFiles(ManagementJobConfiguration config, string host, int port, string user, string pass, string fullPath)
        {
            _logger.LogInformation($"Removing certificate and key from {host}");

            int lastSlash = fullPath.LastIndexOf('/');
            string basePath = fullPath.Substring(0, lastSlash);
            string filename = Path.GetFileNameWithoutExtension(fullPath);

            string certPath = $"{basePath}/{filename}.crt";
            string keyPath = $"{basePath}/{filename}.key";

            using (var ssh = new SshClient(host, port, user, pass))
            {
                ssh.Connect();

                _logger.LogInformation($"Removing cert: {certPath}");
                ssh.RunCommand($"rm -f \"{certPath}\"");

                _logger.LogInformation($"Removing key: {keyPath}");
                ssh.RunCommand($"rm -f \"{keyPath}\"");

                ssh.Disconnect();
            }

            _logger.LogInformation("Certificate and key removed.");
        }


        private bool RemoteFileExists(string host, int port, string user, string pass, string fullPath)
        {
            using (var ssh = new SshClient(host, port, user, pass))
            {
                ssh.Connect();
                var result = ssh.RunCommand($"[ -f \"{fullPath}\" ] && echo \"exists\"");
                ssh.Disconnect();
                return result.Result.Trim() == "exists";
            }
        }

        private (string certPem, string keyPem) GetCertificateAndKey(ManagementJobConfiguration config)
        {
            if (!string.IsNullOrEmpty(config.JobCertificate.PrivateKeyPassword))
            {
                var certData = Convert.FromBase64String(config.JobCertificate.Contents);
                var store = new Pkcs12Store(new MemoryStream(certData), config.JobCertificate.PrivateKeyPassword.ToCharArray());

                string alias = store.Aliases.Cast<string>().FirstOrDefault(a => store.IsKeyEntry(a));
                var cert = store.GetCertificate(alias).Certificate;
                var key = store.GetKey(alias).Key;

                string certPem, keyPem;

                using (var certWriter = new StringWriter())
                {
                    new PemWriter(certWriter).WriteObject(cert);
                    certPem = certWriter.ToString();
                }

                using (var keyWriter = new StringWriter())
                {
                    new PemWriter(keyWriter).WriteObject(key);
                    keyPem = keyWriter.ToString();
                }

                return (certPem, keyPem);
            }
            else
            {
                string certPem = $"-----BEGIN CERTIFICATE-----\n{Pemify(config.JobCertificate.Contents)}\n-----END CERTIFICATE-----";
                string keyPem = ""; // No key provided
                return (certPem, keyPem);
            }
        }


        private string Pemify(string base64)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < base64.Length; i += 64)
                sb.AppendLine(base64.Substring(i, Math.Min(64, base64.Length - i)));
            return sb.ToString().Trim();
        }

        private JobResult Fail(ManagementJobConfiguration config, string message)
        {
            _logger.LogError(message);
            return new JobResult
            {
                JobHistoryId = config.JobHistoryId,
                Result = OrchestratorJobStatusJobResult.Failure,
                FailureMessage = message
            };
        }
    }
}
