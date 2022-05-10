using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

using Keyfactor.Logging;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Keyfactor.Extensions.Orchestrator.vThunder.api
{
    [SuppressMessage("ReSharper", "ConstantConditionalAccessQualifier")]
    public class ApiClient : IDisposable
    {
        private static readonly Encoding Encoding = Encoding.UTF8;
        public virtual string AuthenticationSignature { get; set; }
        public string BaseUrl { get; set; }
        public virtual bool AllowInvalidCert { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }


        #region Interface Implementation

        public void Dispose()
        {
            LogOff();
        }

        #endregion

        #region Constructors

        public ApiClient(string user, string pass, string baseUrl, bool allowInvalidCert)
        {
            BaseUrl = baseUrl;
            UserId = user;
            Password = pass;
            AllowInvalidCert = allowInvalidCert;
        }

        public ApiClient()
        {
        }

        #endregion

        #region Class Methods

        public virtual void Logon()
        {
            ILogger logger = LogHandler.GetClassLogger<Management>();

            var authRequest = new AuthRequest
                {Credentials = new Credentials {Username = UserId, Password = Password}};
            var strRequest = JsonConvert.SerializeObject(authRequest);
            try
            {
                var strResponse = ApiRequestString("POST", "/axapi/v3/auth", "POST", strRequest, false, false);
                var authSignatureResponse = JsonConvert.DeserializeObject<AuthSignatureResponse>(strResponse);
                AuthenticationSignature = authSignatureResponse.Response.Signature;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error Authenticating: {ex.Message}");
            }
        }

        public virtual bool LogOff()
        {
            try
            {
                ApiRequestString("POST", "/axapi/v3/logoff", "POST", "", false, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual void AddCertificate(SslCertificateRequest sslCertRequest, string importCertificate)
        {
            var certData = Encoding.ASCII.GetBytes(importCertificate);
            AddCertificate(sslCertRequest, certData);
        }

        public virtual void AddCertificate(SslCertificateRequest sslCertRequest, byte[] certData)
        {
            var strRequest = JsonConvert.SerializeObject(sslCertRequest);
            var requestArray = Encoding.ASCII.GetBytes(strRequest);

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

            using (var responseReader = new StreamReader(webResponse.GetResponseStream() ?? Stream.Null))
            {
                responseReader.ReadToEnd();
                webResponse.Close();
            }
        }

        public virtual bool AddPrivateKey(SslKeyRequest sslKeyRequest, string importCertificateKey)
        {
            var keyArray = Encoding.ASCII.GetBytes(importCertificateKey);
            AddPrivateKey(sslKeyRequest, keyArray);
            return true;
        }

        public virtual void AddPrivateKey(SslKeyRequest sslKeyRequest, byte[] keyArray)
        {
            var strRequest = JsonConvert.SerializeObject(sslKeyRequest);
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

            // Process response
            using (var responseReader = new StreamReader(webResponse.GetResponseStream() ?? Stream.Null))
            {
                responseReader.ReadToEnd();
                webResponse.Close();
            }
        }

        public virtual SslCollectionResponse GetCertificates(string certName = "")
        {
            var strResponse = ApiRequestString("GET",
                certName.Length == 0
                    ? "/axapi/v3/slb/ssl-cert/oper"
                    : $"/axapi/v3/slb/ssl-cert/oper?name={certName}", "GET", "", false, true);
            var sslColResponse = JsonConvert.DeserializeObject<SslCollectionResponse>(strResponse);
            return sslColResponse;
        }

        public virtual string GetCertificate(string certificateName)
        {
            var strResponse = ApiRequestString("GET", $"/axapi/v3/file/ssl-cert/{certificateName}", "GET", "", false,
                true);
            return strResponse;
        }

        public virtual void RemoveCertificate(DeleteCertBaseRequest deleteCertRoot)
        {
            ApiRequestString("POST", "/axapi/v3/pki/delete", "POST", JsonConvert.SerializeObject(deleteCertRoot),
                false, true);
        }

        public virtual HttpWebRequest CreateRequest(string baseUrl, string postUrl)
        {
            var objRequest = (HttpWebRequest) WebRequest.Create(BaseUrl + postUrl);
            return objRequest;
        }

        public virtual HttpWebResponse GetResponse(HttpWebRequest request)
        {
            return (HttpWebResponse) request.GetResponse();
        }

        public virtual string ApiRequestString(string strCall, string strPostUrl, string strMethod,
            string strQueryString,
            bool bWrite, bool bUseToken)
        {
            var objRequest = CreateRequest(BaseUrl, strPostUrl);
            objRequest.Method = strMethod;
            objRequest.ContentType = "application/json";
            if (bUseToken)
                objRequest.Headers?.Add("Authorization", "A10 " + AuthenticationSignature);

            if (!string.IsNullOrEmpty(strQueryString) && strMethod == "POST")
            {
                var postBytes = Encoding.UTF8.GetBytes(strQueryString);
                objRequest.ContentLength = postBytes.Length;
                //This is for testing on an Azure VM with an invalid certificate
                if (AllowInvalidCert)
                    ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
                using (var requestStream = objRequest.GetRequestStream())
                {
                    requestStream?.Write(postBytes, 0, postBytes.Length);
                    requestStream?.Close();
                }
            }

            //This is for testing on an Azure VM with an invalid certificate
            if (AllowInvalidCert)
                ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
            var objResponse = GetResponse(objRequest);

            using (var strReader = new StreamReader(objResponse.GetResponseStream() ?? Stream.Null))
            {
                var strResponse = strReader.ReadToEnd();
                return strResponse;
            }
        }

        public virtual HttpWebResponse MultipartFormDataPost(string postUrl, string userAgent,
            Dictionary<string, object> postParameters)
        {
            var boundary = $"{Guid.NewGuid():N}";
            var formDataBoundary = $"------------------------{boundary}";
            var contentType = "multipart/form-data; boundary=" + formDataBoundary;

            var formData = GetMultipartFormData(postParameters, boundary);

            return PostForm(postUrl, userAgent, contentType, formData);
        }

        protected virtual HttpWebResponse PostForm(string postUrl, string userAgent, string contentType,
            byte[] formData)
        {
            var request = CreateRequest(BaseUrl, postUrl);
            if (request == null) throw new NullReferenceException("request is not a http request");

            // Set up the request properties.
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.ContentLength = formData.Length;
            request.Headers.Add("Authorization", "A10 " + AuthenticationSignature);
            //This is for testing on an Azure VM with an invalid certificate
            if (AllowInvalidCert)
                ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
            // Send the form data to the request.
            using (var requestStream = request.GetRequestStream())
            {
                requestStream?.Write(formData, 0, formData.Length);
                requestStream?.Close();
            }

            //This is for testing on an Azure VM with an invalid certificate
            if (AllowInvalidCert)
                ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
            return request?.GetResponse() as HttpWebResponse;
        }

        protected virtual byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            byte[] formData;
            using (Stream formDataStream = new MemoryStream())
            {
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
                formData = new byte[formDataStream.Length];
                formDataStream.Read(formData, 0, formData.Length);
                formDataStream.Close();
            }

            return formData;
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