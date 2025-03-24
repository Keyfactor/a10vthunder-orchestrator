using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using a10vthunder_orchestrator.Api.Models;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace a10vthunder_orchestrator.Api
{
    public sealed class ApiClient : IDisposable
    {
        private static readonly Encoding Encoding = Encoding.UTF8;

        #region Constructors

        public ApiClient(string user, string pass, string baseUrl, bool allowInvalidCert)
        {
            Logger = LogHandler.GetClassLogger<ApiClient>();
            BaseUrl = baseUrl;
            UserId = user;
            Password = pass;
            AllowInvalidCert = allowInvalidCert;
        }

        #endregion

        public string AuthenticationSignature { get; set; }
        public string BaseUrl { get; set; }
        public bool AllowInvalidCert { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        private ILogger Logger { get; }

        #region Interface Implementation

        public void Dispose()
        {
            LogOff();
        }

        #endregion

        #region Class Methods

        public void Logon()
        {
            Logger.MethodEntry();
            var authRequest = new AuthRequest
                {Credentials = new Credentials {Username = UserId, Password = Password}};
            var strRequest = JsonConvert.SerializeObject(authRequest);
            try
            {
                Logger.LogTrace($"Logging Login Request JSON: {strRequest}");
                var strResponse = ApiRequestString("POST", "/axapi/v3/auth", "POST", strRequest, false, false);
                Logger.LogTrace($"Logging Login Response JSON: {strResponse}");
                var authSignatureResponse = JsonConvert.DeserializeObject<AuthSignatureResponse>(strResponse);
                Logger.LogTrace($"Auth Signature Response: {JsonConvert.SerializeObject(authSignatureResponse)}");
                AuthenticationSignature = authSignatureResponse?.Response.Signature;
                Logger.LogTrace($"Auth Signature: {AuthenticationSignature}");
                Logger.MethodExit();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error Authenticating: {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public bool LogOff()
        {
            try
            {
                Logger.MethodEntry();
                ApiRequestString("POST", "/axapi/v3/logoff", "POST", "", false, true);
                Logger.MethodExit();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error Logging Off: {LogHandler.FlattenException(ex)}");
                return false;
            }
        }

        public void AddCertificate(SslCertificateRequest sslCertRequest, string importCertificate)
        {
            try
            {
                Logger.MethodEntry();
                Logger.LogTrace($"Ssl Certificate Request: {JsonConvert.SerializeObject(sslCertRequest)}");
                Logger.LogTrace($"importCertificate: {JsonConvert.SerializeObject(importCertificate)}");
                var certData = Encoding.ASCII.GetBytes(importCertificate);
                Logger.LogTrace("Got Cert Data Adding Certificate...");
                AddCertificate(sslCertRequest, certData);
                Logger.LogTrace("Added Certificate...");
                Logger.MethodExit();
            }
            catch (Exception ex)
            {
                Logger.LogError(
                    $"Error In ApiClient.AddCertificate(SslCertificateRequest sslCertRequest, string importCertificate): {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public void SetPartition(SetPartitionRequest sslSetPartitionRequest)
        {
            try
            {
                Logger.MethodEntry();
                Logger.LogTrace($"Set Partition Request: {JsonConvert.SerializeObject(sslSetPartitionRequest)}");
                ApiRequestString("POST", "/axapi/v3/active-partition", "POST", JsonConvert.SerializeObject(sslSetPartitionRequest),
                    false, true);
                Logger.LogTrace("Set Partition Complete...");
                Logger.MethodExit();
            }
            catch (Exception ex)
            {
                Logger.LogError(
                    $"Error In ApiClient.AddCertificate(SslCertificateRequest sslCertRequest, string importCertificate): {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public void WriteMemory()
        {
            try
            {
                Logger.MethodEntry();
                ApiRequestString("POST", "/axapi/v3/write/memory", "POST", "",
                    false, true);
                Logger.LogTrace("WriteMemory Complete...");
                Logger.MethodExit();
            }
            catch (Exception ex)
            {
                Logger.LogError(
                    $"Error In ApiClient.WriteMemory: {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public void AddCertificate(SslCertificateRequest sslCertRequest, byte[] certData)
        {
            try
            {
                Logger.MethodEntry();
                var strRequest = JsonConvert.SerializeObject(sslCertRequest);
                Logger.LogTrace($"sslCertRequest: {JsonConvert.SerializeObject(strRequest)}");
                var requestArray = Encoding.ASCII.GetBytes(strRequest);
                Logger.LogTrace("Got requestArray...");
                // Generate post objects
                var postParameters = new Dictionary<string, object>
                {
                    {"json", new FileParameter(requestArray, "a10.json", "application/json")},
                    {
                        "file",
                        new FileParameter(certData, sslCertRequest.SslCertificate.File, "application/octet-stream")
                    }
                };

                // Create request and receive response
                var userAgent = "Keyfactor Agent";
                var webResponse = MultipartFormDataPost("/axapi/v3/file/ssl-cert", userAgent, postParameters);
                Logger.LogTrace("Got webResponse...");
                using var responseReader = new StreamReader(webResponse.GetResponseStream() ?? Stream.Null);
                responseReader.ReadToEnd();
                webResponse.Close();
                Logger.MethodExit();
            }
            catch (Exception ex)
            {
                Logger.LogError(
                    $"Error In ApiClient.AddCertificate(SslCertificateRequest sslCertRequest, byte[] certData): {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public bool AddPrivateKey(SslKeyRequest sslKeyRequest, string importCertificateKey)
        {
            try
            {
                Logger.MethodEntry();
                Logger.LogTrace($"sslKeyRequest: {JsonConvert.SerializeObject(sslKeyRequest)}");
                Logger.LogTrace($"importCertificateKey: {importCertificateKey}");
                var keyArray = Encoding.ASCII.GetBytes(importCertificateKey);
                Logger.LogTrace("Got keyArray...");
                AddPrivateKey(sslKeyRequest, keyArray);
                Logger.LogTrace("Added PrivateKey...");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(
                    $"Error In AddPrivateKey(SslKeyRequest sslKeyRequest, string importCertificateKey): {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public void AddPrivateKey(SslKeyRequest sslKeyRequest, byte[] keyArray)
        {
            try
            {
                Logger.MethodEntry();
                var strRequest = JsonConvert.SerializeObject(sslKeyRequest);
                Logger.LogTrace($"sslKeyRequest: {JsonConvert.SerializeObject(sslKeyRequest)}");
                var requestArray = Encoding.ASCII.GetBytes(strRequest);

                // Generate post objects
                var postParameters = new Dictionary<string, object>
                {
                    {"json", new FileParameter(requestArray, "a10.json", "application/json")},
                    {"file", new FileParameter(keyArray, sslKeyRequest.SslKey.File, "application/octet-stream")}
                };

                // Create request and receive response
                var userAgent = "Keyfactor Agent";
                var webResponse = MultipartFormDataPost("/axapi/v3/file/ssl-key", userAgent, postParameters);
                Logger.LogTrace("Got webResponse...");

                // Process response
                using var responseReader = new StreamReader(webResponse.GetResponseStream() ?? Stream.Null);
                responseReader.ReadToEnd();
                webResponse.Close();
                Logger.MethodExit();
            }
            catch (Exception ex)
            {
                Logger.LogError(
                    $"Error In AddPrivateKey(SslKeyRequest sslKeyRequest, byte[] keyArray): {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public SslCollectionResponse GetCertificates(string certName = "")
        {
            try
            {
                Logger.MethodEntry();
                Logger.LogTrace($"certName: {certName}");
                var strResponse = ApiRequestString("GET",
                    certName.Length == 0
                        ? "/axapi/v3/slb/ssl-cert/oper"
                        : $"/axapi/v3/slb/ssl-cert/oper?name={certName}", "GET", "", false, true);
                Logger.LogTrace($"strResponse: {strResponse}");
                var sslColResponse = JsonConvert.DeserializeObject<SslCollectionResponse>(strResponse);
                Logger.LogTrace($"sslColResponse: {JsonConvert.SerializeObject(sslColResponse)}");
                Logger.MethodExit();
                return sslColResponse;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error In GetCertificates(string certName): {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public ServerTemplateListResponse GetServerTemplates()
        {
            try
            {
                Logger.MethodEntry();
                var strResponse = ApiRequestString("GET", $"/axapi/v3/slb/template/server-ssl-list", "GET", "", false, true);
                Logger.LogTrace($"strResponse: {strResponse}");
                var sslTemplateResponse = JsonConvert.DeserializeObject<ServerTemplateListResponse>(strResponse);
                Logger.LogTrace($"sslColResponse: {JsonConvert.SerializeObject(sslTemplateResponse)}");
                Logger.MethodExit();
                return sslTemplateResponse;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error In GetServerTemplates(): {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public ClientTemplateListResponse GetClientTemplates()
        {
            try
            {
                Logger.MethodEntry();
                var strResponse = ApiRequestString("GET", $"/axapi/v3/slb/template/client-ssl-list", "GET", "", false, true);
                Logger.LogTrace($"strResponse: {strResponse}");
                var sslTemplateResponse = JsonConvert.DeserializeObject<ClientTemplateListResponse>(strResponse);
                Logger.LogTrace($"sslColResponse: {JsonConvert.SerializeObject(sslTemplateResponse)}");
                Logger.MethodExit();
                return sslTemplateResponse;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error In GetClientTemplates(): {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public UpdateServerTemplateResponse UpdateServerTemplates(UpdateTemplateRequest request,string templateName)
        {
            try
            {
                Logger.MethodEntry();
                var strResponse = ApiRequestString("PUT", $"/axapi/v3/slb/template/server-ssl/{templateName}/certificate", "PUT", JsonConvert.SerializeObject(request),
                    false, true); 
                Logger.LogTrace($"strResponse: {strResponse}");
                var sslTemplateResponse = JsonConvert.DeserializeObject<UpdateServerTemplateResponse>(strResponse);
                Logger.LogTrace($"sslColResponse: {JsonConvert.SerializeObject(sslTemplateResponse)}");
                Logger.MethodExit();
                return sslTemplateResponse;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error In UpdateTemplates(UpdateTemplateRequest request,string templateName): {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public UpdateClientTemplateResponse UpdateClientTemplates(UpdateTemplateRequest request, string templateName)
        {
            try
            {
                Logger.MethodEntry();
                var strResponse = ApiRequestString("PUT", $"/axapi/v3/slb/template/client-ssl/{templateName}/certificate", "PUT", JsonConvert.SerializeObject(request),
                    false, true);
                Logger.LogTrace($"strResponse: {strResponse}");
                var sslTemplateResponse = JsonConvert.DeserializeObject<UpdateClientTemplateResponse>(strResponse);
                Logger.LogTrace($"sslColResponse: {JsonConvert.SerializeObject(sslTemplateResponse)}");
                Logger.MethodExit();
                return sslTemplateResponse;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error In UpdateTemplates(UpdateTemplateRequest request,string templateName): {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public string GetCertificate(string certificateName)
        {
            try
            {
                Logger.MethodEntry();
                var strResponse = ApiRequestString("GET", $"/axapi/v3/file/ssl-cert/{certificateName}", "GET", "",
                    false,
                    true);
                Logger.LogTrace($"strResponse: {strResponse}");
                Logger.MethodExit();
                return strResponse;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error In GetCertificate(string certificateName): {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public void RemoveCertificate(DeleteCertBaseRequest deleteCertRoot)
        {
            try
            {
                Logger.MethodEntry();
                Logger.LogTrace($"deleteCertRoot: {JsonConvert.SerializeObject(deleteCertRoot)}");
                ApiRequestString("POST", "/axapi/v3/pki/delete", "POST", JsonConvert.SerializeObject(deleteCertRoot),
                    false, true);
                Logger.MethodExit();
            }
            catch (Exception ex)
            {
                Logger.LogError(
                    $"Error In RemoveCertificate(DeleteCertBaseRequest deleteCertRoot): {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public HttpWebRequest CreateRequest(string baseUrl, string postUrl)
        {
            try
            {
                Logger.MethodEntry();
                Logger.LogTrace($"baseUrl: {baseUrl} postUrl: {postUrl}");
                var objRequest = (HttpWebRequest) WebRequest.Create(BaseUrl + postUrl);
                Logger.MethodExit();
                return objRequest;
            }
            catch (Exception ex)
            {
                Logger.LogError(
                    $"Error In CreateRequest(string baseUrl, string postUrl): {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public HttpWebResponse GetResponse(HttpWebRequest request)
        {
            try
            {
                Logger.MethodEntry();
                Logger.MethodExit();
                return (HttpWebResponse) request.GetResponse();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error In GetResponse(HttpWebRequest request): {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public string ApiRequestString(string strCall, string strPostUrl, string strMethod,
            string strQueryString,
            bool bWrite, bool bUseToken)
        {
            try
            {
                Logger.MethodEntry();
                var objRequest = CreateRequest(BaseUrl, strPostUrl);
                Logger.LogTrace(
                    $"Request Object Created... method will be {strMethod} postURL will be {strPostUrl} query string will be {strQueryString}");
                objRequest.Method = strMethod;
                objRequest.ContentType = "application/json";
                Logger.LogTrace($"Use Token {bUseToken}");
                Logger.LogTrace($"AuthenticationSignature {AuthenticationSignature}");
                if (bUseToken)
                    objRequest.Headers.Add("Authorization", "A10 " + AuthenticationSignature);

                if (!string.IsNullOrEmpty(strQueryString) && (strMethod == "POST" || strMethod=="PUT"))
                {
                    var postBytes = Encoding.UTF8.GetBytes(strQueryString);
                    Logger.LogTrace($"postBytes.Length {postBytes.Length}");
                    objRequest.ContentLength = postBytes.Length;
                    //This is for testing on an Azure VM with an invalid certificate
                    if (AllowInvalidCert)
                        ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
                    using var requestStream = objRequest.GetRequestStream();
                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Close();
                }

                Logger.LogTrace($"AllowInvalidCert {AllowInvalidCert}");
                //This is for testing on an Azure VM with an invalid certificate
                if (AllowInvalidCert)
                    ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
                var objResponse = GetResponse(objRequest);
                Logger.LogTrace("Got Response");
                using var strReader = new StreamReader(objResponse.GetResponseStream() ?? Stream.Null);
                var strResponse = strReader.ReadToEnd();
                Logger.MethodExit();
                return strResponse;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error In ApiRequestString: {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public HttpWebResponse MultipartFormDataPost(string postUrl, string userAgent,
            Dictionary<string, object> postParameters)
        {
            try
            {
                Logger.MethodEntry();
                var boundary = $"{Guid.NewGuid():N}";
                Logger.LogTrace($"boundary {boundary}");
                var formDataBoundary = $"------------------------{boundary}";
                Logger.LogTrace($"formDataBoundary {formDataBoundary}");
                var contentType = "multipart/form-data; boundary=" + formDataBoundary;
                Logger.LogTrace($"contentType {contentType}");
                var formData = GetMultipartFormData(postParameters, boundary);
                Logger.LogTrace("Got formData");
                Logger.MethodExit();
                return PostForm(postUrl, userAgent, contentType, formData);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error In MultipartFormDataPost: {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        private HttpWebResponse PostForm(string postUrl, string userAgent, string contentType,
            byte[] formData)
        {
            try
            {
                Logger.MethodEntry();
                Logger.LogTrace($"postUrl {postUrl}");
                Logger.LogTrace($"userAgent {userAgent}");
                Logger.LogTrace($"contentType {contentType}");
                var request = CreateRequest(BaseUrl, postUrl);
                if (request == null) throw new NullReferenceException("request is not a http request");
                Logger.LogTrace("Request Created...");
                // Set up the request properties.
                request.Method = "POST";
                request.ContentType = contentType;
                request.UserAgent = userAgent;
                request.ContentLength = formData.Length;
                Logger.LogTrace($"ContentLength {request.ContentLength}");
                Logger.LogTrace($"AuthenticationSignature {AuthenticationSignature}");
                request.Headers.Add("Authorization", "A10 " + AuthenticationSignature);
                //This is for testing on an Azure VM with an invalid certificate
                if (AllowInvalidCert)
                    ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
                // Send the form data to the request.
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(formData, 0, formData.Length);
                    requestStream.Close();
                }

                Logger.LogTrace($"AllowInvalidCert {AllowInvalidCert}");
                //This is for testing on an Azure VM with an invalid certificate
                if (AllowInvalidCert)
                    ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
                Logger.MethodExit();
                return request.GetResponse() as HttpWebResponse;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error In PostForm: {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        private byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            try
            {
                Logger.MethodEntry();
                using Stream formDataStream = new MemoryStream();
                var needsClrf = false;

                foreach (var param in postParameters)
                {
                    // Thanks to feedback from comment-ers, add a CRLF to allow multiple parameters to be added.
                    // Skip it on the first parameter, add it to subsequent parameters.
                    if (needsClrf)
                        formDataStream.Write(Encoding.GetBytes("\r\n"), 0, Encoding.GetByteCount("\r\n"));

                    needsClrf = true;

                    if (param.Value is FileParameter fileToUpload)
                    {
                        // Add just the first part of this param, since we will write the file data directly to the Stream

                        var header =
                            $"--------------------------{boundary}\r\nContent-Disposition: form-data; name=\"{param.Key}\"; filename=\"{fileToUpload.FileName ?? param.Key}\"\r\nContent-Type: {fileToUpload.ContentType ?? "application/octet-stream"}\r\n\r\n";

                        formDataStream.Write(Encoding.GetBytes(header), 0, Encoding.GetByteCount(header));

                        // Write the file data directly to the Stream, rather than serializing it to a string.
                        formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                    }
                }

                // Add the end of the request.  Start with a newline
                var footer = "\r\n--------------------------" + boundary + "--\r\n";
                formDataStream.Write(Encoding.GetBytes(footer), 0, Encoding.GetByteCount(footer));

                // Dump the Stream into a byte[]
                formDataStream.Position = 0;
                var formData = new byte[formDataStream.Length];
                // ReSharper disable once MustUseReturnValue
                formDataStream.Read(formData, 0, formData.Length);
                formDataStream.Close();
                Logger.MethodExit();
                return formData;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error In GetMultipartFormData: {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        public class FileParameter
        {
            public FileParameter(byte[] file, string filename, string contentType)
            {
                File = file;
                FileName = filename;
                ContentType = contentType;
            }

            public byte[] File { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
        }

        #endregion
    }
}