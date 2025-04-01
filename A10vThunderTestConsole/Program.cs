// Copyright 2023 Keyfactor
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Keyfactor.Orchestrators.Extensions;
using Keyfactor.Orchestrators.Extensions.Interfaces;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace A10vThunderTestConsole
{
    internal class Program
    {
        public static string UserName { get; set; }
        public static string Password { get; set; }
        public static string CaseName { get; set; }
        public static string CertAlias { get; set; }
        public static string ClientMachine { get; set; }
        public static string StorePath { get; set; }
        public static string Overwrite { get; set; }
        public static string ManagementType { get; set; }
        public static string CertificateContent { get; set; }
        public static string OrchToScpServerIp { get; set; }
        public static string A10ToScpServerIp { get; set; }
        public static string ScpPort { get; set; }
        public static string ScpUserName { get; set; }
        public static string ScpPassword { get; set; }
        public static string StoreType { get; set; }


        private static async Task Main(string[] args)
        {
            Thread.Sleep(20000);
            var arguments = ParseArgs(args);

            CaseName = GetValue(arguments, "-casename", "Enter The Case Name Inventory or Management");
            UserName = GetValue(arguments, "-user", "Enter User Name");
            Password = GetValue(arguments, "-password", "Enter The Password");
            StorePath = GetValue(arguments, "-storepath", "Enter Store Path");
            StoreType = GetValue(arguments, "-storetype", "Enter Store Type");
            if (StoreType.ToLower() == "mgmt")
            {
                OrchToScpServerIp = GetValue(arguments, "-orchtoscpserverip", "Enter Orch To Scp Server Ip");
                A10ToScpServerIp = GetValue(arguments, "-a10toscpserverip", "Enter A10 to Scp Server Ip");
                ScpPort = GetValue(arguments, "-scpport", "Enter Scp Port");
                ScpUserName = GetValue(arguments, "-scpusername", "Enter Scp User Name");
                ScpPassword = GetValue(arguments, "-scppassword", "Enter Scp Password");
            }
            ClientMachine = GetValue(arguments, "-clientmachine", "Enter Client Machine");


            Console.WriteLine("Running");

            switch (CaseName?.ToUpper())
            {
                case "INVENTORY":
                    RunInventory(arguments);
                    break;

                case "MANAGEMENT":
                    ManagementType = GetValue(arguments, "-managementtype", "Select Management Type Add or Remove");
                    if (ManagementType?.ToUpper() == "ADD")
                        RunManagementAdd(arguments);
                    else if (ManagementType?.ToUpper() == "REMOVE")
                        RunManagementRemove(arguments);
                    break;
            }
        }

        private static Dictionary<string, string> ParseArgs(string[] args)
        {
            var result = new Dictionary<string, string>();
            foreach (var arg in args)
            {
                var parts = arg.Split('=', 2);
                if (parts.Length == 2)
                    result[parts[0].ToLower()] = parts[1];
            }
            return result;
        }

        private static string GetValue(Dictionary<string, string> args, string key, string prompt)
        {
            return args.TryGetValue(key.ToLower(), out var value) ? value : Prompt(prompt);
        }

        private static string Prompt(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
        }

        private static void RunInventory(Dictionary<string, string> args)
        {
            StoreType = GetValue(args, "-storetype", "Enter Cert Alias");

            Console.WriteLine("Running Inventory");
            var config = GetInventoryJobConfiguration(StoreType);
            Console.WriteLine("Got Inventory Config");

            var secretResolver = MockSecrets(config.ServerUsername, config.ServerPassword);

            JobResult result;

            if (StoreType.ToLower() == "ssl")
            {
                var inv = new Keyfactor.Extensions.Orchestrator.ThunderSsl.Jobs.Inventory(secretResolver);
                result = inv.ProcessJob(config, GetItems);
            }
            else
            {
                var inv = new Keyfactor.Extensions.Orchestrator.ThunderMgmt.Jobs.Inventory(secretResolver);
                result = inv.ProcessJob(config, GetItems);
            }

            Console.WriteLine("Inventory Response:");
            Console.WriteLine(JsonConvert.SerializeObject(result));
            Console.ReadLine();
        }

        private static void RunManagementAdd(Dictionary<string, string> args)
        {
            CertAlias = GetValue(args, "-certalias", "Enter Cert Alias");
            Overwrite = GetValue(args, "-overwrite", "Overwrite (True or False)?");
            StoreType = GetValue(args, "-storetype", "Enter Store Type");

            Console.WriteLine("Generating Cert from KF API...");
            var client = new KeyfactorClient();
            CertificateContent = client.EnrollCertificate($"www.{CertAlias}.com").Result.CertificateInformation.Pkcs12Blob;

            var config = GetManagementJobConfiguration(StoreType);
            var secretResolver = MockSecrets(config.ServerUsername, config.ServerPassword);

            JobResult result;

            if (StoreType.ToLower() == "ssl")
            {
                var mgmt = new Keyfactor.Extensions.Orchestrator.ThunderSsl.Jobs.Management(secretResolver);
                result = mgmt.ProcessJob(config);
            }
            else
            {
                var mgmt = new Keyfactor.Extensions.Orchestrator.ThunderMgmt.Jobs.Management(secretResolver);
                result = mgmt.ProcessJob(config);
            }
            Console.WriteLine(JsonConvert.SerializeObject(result));
            Console.ReadLine();
        }

        private static void RunManagementRemove(Dictionary<string, string> args)
        {
            CertAlias = GetValue(args, "-certalias", "Enter Cert Alias");
            StoreType = GetValue(args, "-storetype", "Enter Store Type");


            var config = GetRemoveJobConfiguration(StoreType);
            var secretResolver = MockSecrets(config.ServerUsername, config.ServerPassword);
            JobResult result;

            if (StoreType.ToLower() == "ssl")
            {
                var mgmt = new Keyfactor.Extensions.Orchestrator.ThunderSsl.Jobs.Management(secretResolver);
                result = mgmt.ProcessJob(config);
            }
            else
            {
                var mgmt = new Keyfactor.Extensions.Orchestrator.ThunderMgmt.Jobs.Management(secretResolver);
                result = mgmt.ProcessJob(config);
            }
            Thread.Sleep(5000);
            Console.WriteLine(JsonConvert.SerializeObject(result));
            Console.ReadLine();
        }

        private static IPAMSecretResolver MockSecrets(string username, string password)
        {
            var mock = new Mock<IPAMSecretResolver>();
            mock.Setup(m => m.Resolve(It.Is<string>(s => s == username))).Returns(username);
            mock.Setup(m => m.Resolve(It.Is<string>(s => s == password))).Returns(password);
            return mock.Object;
        }

        public static bool GetItems(IEnumerable<CurrentInventoryItem> items) => true;

        private static InventoryJobConfiguration GetInventoryJobConfiguration(string storeType)
        {
            string fileContent = string.Empty;

            if (storeType.ToLower() == "ssl")
            {
                fileContent = File.ReadAllText($"A10SslInventory.json")
                    .Replace("UserNameGoesHere", UserName)
                    .Replace("PasswordGoesHere", Password)
                    .Replace("ClientMachineGoesHere", ClientMachine)
                    .Replace("PartitionNameGoesHere", StorePath);
            }
            else
            {
                fileContent = File.ReadAllText($"A10MgmtInventory.json")
                .Replace("UserNameGoesHere", UserName)
                .Replace("PasswordGoesHere", Password)
                .Replace("ClientMachineGoesHere", ClientMachine)
                .Replace("ScpPathGoesHere", StorePath)
                .Replace("OrchToScpServerIpGoesHere", OrchToScpServerIp)
                .Replace("A10ToScpServerIpGoesHere", A10ToScpServerIp)
                .Replace("ScpPortGoesHere",ScpPort)
                .Replace("ScpUserGoesHere", ScpUserName)
                .Replace("ScpPwdGoesHere",ScpPassword);
            }



            return JsonConvert.DeserializeObject<InventoryJobConfiguration>(JObject.Parse(fileContent).ToString());
        }

        private static ManagementJobConfiguration GetManagementJobConfiguration(string storeType)
        {
            var overwriteValue = Overwrite?.ToUpper() == "TRUE" ? "true" : "false";

            string fileContent;
            if (storeType.ToLower() == "ssl")
            {
                fileContent = File.ReadAllText($"A10SslManagement.json")
                .Replace("UserNameGoesHere", UserName)
                .Replace("PasswordGoesHere", Password)
                .Replace("PartitionNameGoesHere", StorePath)
                .Replace("AliasGoesHere", CertAlias)
                .Replace("ClientMachineGoesHere", ClientMachine)
                .Replace("\"Overwrite\": false", $"\"Overwrite\": {overwriteValue}")
                .Replace("CertificateContentGoesHere", CertificateContent);
            }
            else
            {
                fileContent = File.ReadAllText($"A10MgmtManagement.json")
                .Replace("UserNameGoesHere", UserName)
                .Replace("PasswordGoesHere", Password)
                .Replace("PartitionNameGoesHere", StorePath)
                .Replace("AliasGoesHere", CertAlias)
                .Replace("ClientMachineGoesHere", ClientMachine)
                .Replace("\"Overwrite\": false", $"\"Overwrite\": {overwriteValue}")
                .Replace("ScpPathGoesHere", StorePath)
                .Replace("OrchToScpServerIpGoesHere", OrchToScpServerIp)
                .Replace("A10ToScpServerIpGoesHere", A10ToScpServerIp)
                .Replace("ScpPortGoesHere", ScpPort)
                .Replace("ScpUserGoesHere", ScpUserName)
                .Replace("ScpPwdGoesHere", ScpPassword)
                .Replace("CertificateContentGoesHere", CertificateContent);
            }

            return JsonConvert.DeserializeObject<ManagementJobConfiguration>(JObject.Parse(fileContent).ToString());
        }

        private static ManagementJobConfiguration GetRemoveJobConfiguration(string storeType)
        {

            string fileContent = string.Empty;

            if (storeType.ToLower() == "ssl")
            {
                fileContent = File.ReadAllText($"A10SsllManagementRemove.json")
                 .Replace("UserNameGoesHere", UserName)
                 .Replace("PasswordGoesHere", Password)
                 .Replace("PartitionNameGoesHere", StorePath)
                 .Replace("AliasGoesHere", CertAlias)
                 .Replace("ClientMachineGoesHere", ClientMachine);
            }
            else
            {
                fileContent = File.ReadAllText($"A10MgmtManagementRemove.json")
                 .Replace("UserNameGoesHere", UserName)
                 .Replace("PasswordGoesHere", Password)
                 .Replace("PartitionNameGoesHere", StorePath)
                 .Replace("AliasGoesHere", CertAlias)
                 .Replace("ScpPathGoesHere", StorePath)
                 .Replace("OrchToScpServerIpGoesHere", OrchToScpServerIp)
                 .Replace("A10ToScpServerIpGoesHere", A10ToScpServerIp)
                 .Replace("ScpPortGoesHere", ScpPort)
                 .Replace("ScpUserGoesHere", ScpUserName)
                 .Replace("ScpPwdGoesHere", ScpPassword)
                 .Replace("ClientMachineGoesHere", ClientMachine);
            }



            return JsonConvert.DeserializeObject<ManagementJobConfiguration>(fileContent);
        }
    }
}
