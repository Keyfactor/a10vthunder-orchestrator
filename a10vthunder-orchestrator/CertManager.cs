using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CSS.Common.Logging;
using Keyfactor.Extensions.Orchestrator.vThunder.api;
using Keyfactor.Platform.Extensions.Agents;
using Keyfactor.Platform.Extensions.Agents.Enums;

namespace Keyfactor.Extensions.Orchestrator.vThunder
{
    public class CertManager : LoggingClientBase
    {
        public virtual InventoryResult GetCert(ApiClient apiClient, string certName)
        {
            return InventoryResult(apiClient, certName);
        }

        public virtual InventoryResult GetCerts(ApiClient apiClient)
        {
            return InventoryResult(apiClient);
        }

        protected virtual SslCollectionResponse CertificateCollection { get; set; }

        protected virtual string CertificateDetails { get; set; }

        public virtual InventoryResult InventoryResult(ApiClient apiClient, string certName = "")
        {
            var result = new InventoryResult();
            var error = new AnyErrors {HasError = false};

            logger.LogTrace("GetCerts");


            CertificateCollection =
                certName == "" ? apiClient.GetCertificates() : apiClient.GetCertificates(certName);

            var inventoryItems = new List<AgentCertStoreInventoryItem>();

            logger.LogTrace("Start loop");

            if (CertificateCollection != null)
                foreach (var cc in CertificateCollection.SslCertificate.Oper.SslCertificates)
                    if (!string.IsNullOrEmpty(cc.Name))
                    {
                        logger.LogTrace($"Looping through Certificate Store files: {cc.Name}");

                        var privateKeyEntry = cc.Type == "certificate/key";

                        try
                        {
                            CertificateDetails = apiClient.GetCertificate(cc.Name);

                            //check this is a valid cert, if not fall to the errors
                            var cert = new X509Certificate2(Encoding.UTF8.GetBytes(CertificateDetails));

                            logger.LogTrace($"Add to list: {cc.Name}");
                            if (cert.Thumbprint != null)
                                inventoryItems.Add(
                                    new AgentCertStoreInventoryItem
                                    {
                                        Certificates = new[]
                                            {CertificateDetails},
                                        Alias = cc.Name,
                                        PrivateKeyEntry = privateKeyEntry,
                                        ItemStatus = AgentInventoryItemStatus.Unknown,
                                        UseChainLevel = true
                                    });
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Certificate not retrievable: Error on {cc.Name}: {ex.Message}");
                            error.ErrorMessage = ex.Message;
                            error.HasError = true;
                        }
                    }


            result.Errors = error;
            result.InventoryList = inventoryItems;

            return result;
        }
    }
}