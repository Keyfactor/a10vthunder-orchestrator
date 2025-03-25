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
using Org.BouncyCastle.Crypto;
using Renci.SshNet;

namespace linux_scp_orchestrator.ImplementedStoreTypes
{
    public class Management : IManagementJobExtension
    {
        private readonly ILogger<Management> _logger;
        public string ExtensionName => "ThunderMgmt";

        public Management(ILogger<Management> logger)
        {
            _logger = logger;
        }

        public JobResult ProcessJob(ManagementJobConfiguration config)
        {
            _logger.MethodEntry();

            try
            {
                dynamic props = JsonConvert.DeserializeObject(config.CertificateStoreDetails.Properties);

                string host = props.host;
                string username = props.username;
                string password = props.password;
                string path = props.path;
                string filename = config.JobCertificate.Alias;
                int port = props.port != null ? (int)props.port : 22;

                string fullPath = $"{path}/{filename}";

                switch (config.OperationType)
                {
                    case CertStoreOperationType.Add:
                        if (!config.Overwrite && RemoteFileExists(host, port, username, password, fullPath))
                        {
                            return Fail(config, "File already exists. Use Overwrite flag to replace it.");
                        }

                        ReplacePemFile(config, host, port, username, password, fullPath);
                        break;

                    case CertStoreOperationType.Remove:
                        RemovePemFile(config, host, port, username, password, fullPath);
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

        private void ReplacePemFile(ManagementJobConfiguration config, string host, int port, string user, string pass, string fullPath)
        {
            _logger.LogInformation($"Uploading PEM to {host}:{fullPath}");

            var pemData = GetPemFileContents(config);

            using (var client = new ScpClient(host, port, user, pass))
            {
                client.Connect();
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(pemData)))
                {
                    client.Upload(ms, fullPath);
                }
                client.Disconnect();
            }

            _logger.LogInformation("Upload completed.");
        }

        private void RemovePemFile(ManagementJobConfiguration config, string host, int port, string user, string pass, string fullPath)
        {
            _logger.LogInformation($"Removing PEM from {host}:{fullPath}");

            using (var ssh = new SshClient(host, port, user, pass))
            {
                ssh.Connect();
                ssh.RunCommand($"rm -f \"{fullPath}\"");
                ssh.Disconnect();
            }

            _logger.LogInformation("File removed.");
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

        private string GetPemFileContents(ManagementJobConfiguration config)
        {
            if (!string.IsNullOrEmpty(config.JobCertificate.PrivateKeyPassword))
            {
                var certData = Convert.FromBase64String(config.JobCertificate.Contents);
                var store = new Pkcs12Store(new MemoryStream(certData), config.JobCertificate.PrivateKeyPassword.ToCharArray());

                string alias = store.Aliases.Cast<string>().FirstOrDefault(a => store.IsKeyEntry(a));
                var cert = store.GetCertificate(alias).Certificate;
                var key = store.GetKey(alias).Key;

                StringBuilder pemBuilder = new StringBuilder();

                using (var certWriter = new StringWriter())
                {
                    new PemWriter(certWriter).WriteObject(cert);
                    pemBuilder.Append(certWriter.ToString());
                }

                using (var keyWriter = new StringWriter())
                {
                    new PemWriter(keyWriter).WriteObject(key);
                    pemBuilder.Append(keyWriter.ToString());
                }

                return pemBuilder.ToString();
            }
            else
            {
                return $"-----BEGIN CERTIFICATE-----\n{Pemify(config.JobCertificate.Contents)}\n-----END CERTIFICATE-----";
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
