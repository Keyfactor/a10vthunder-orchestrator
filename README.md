# a10vThunder

A10 vThunder AnyAgent allows an organization to inventory and deploy certificates in any domain that the appliance services. The AnyAgent deploys the appropriate files (.cer, .pem) within the defined directories and also performs and Inventory on the Items.

#### Integration status: 

## About the Keyfactor Universal Orchestrator Capability

This repository contains a Universal Orchestrator Capability which is a plugin to the Keyfactor Universal Orchestrator. Within the Keyfactor Platform, Orchestrators are used to manage “certificate stores” &mdash; collections of certificates and roots of trust that are found within and used by various applications.

The Universal Orchestrator is part of the Keyfactor software distribution and is available via the Keyfactor customer portal. For general instructions on installing Capabilities, see the “Keyfactor Command Orchestrator Installation and Configuration Guide” section of the Keyfactor documentation. For configuration details of this specific Capability, see below in this readme.

The Universal Orchestrator is the successor to the Windows Orchestrator. This Capability plugin only works with the Universal Orchestrator and does not work with the Windows Orchestrator.

---




---

**A10 Networks vThunder Orchestrator**

**Overview**

A10 vThunder AnyAgent allows an organization to inventory and deploy certificates in any domain that the appliance services. The AnyAgent deploys the appropriate files (.cer, .pem) within the defined directories and also performs and Inventory on the Items.

This agent implements three job types – Inventory, Management Add, and Management Remove. Below are the steps necessary to configure this AnyAgent.  It supports adding certificates with or without private keys.


**A10 vThunder Configuration**

1. Read up on [A10 Networks ADC](https://a10networks.optrics.com/downloads/datasheets/Thunder-Application-Delivery-Controller-ADC.pdf) and how it works.
2. A user account is needed with the appropriate permissions on vThunder to manage certificates.

**1. Create the New Certificate Store Type for the A10 vThunder Orchestrator**

In Keyfactor Command create a new Certificate Store Type similar to the one below:

#### STORE TYPE CONFIGURATION
SETTING TAB  |  CONFIG ELEMENT	| DESCRIPTION
------|-----------|------------------
Basic |Name	|Descriptive name for the Store Type.  A10 vThunder can be used.
Basic |Short Name	|The short name that identifies the registered functionality of the orchestrator. Must be vThunderU
Basic |Custom Capability|Un checked
Basic |Job Types	|Inventory, Add, and Remove are the supported job types. 
Basic |Needs Server	|Must be checked
Basic |Blueprint Allowed	|checked
Basic |Requires Store Password	|Determines if a store password is required when configuring an individual store.  This must be unchecked.
Basic |Supports Entry Password	|Determined if an individual entry within a store can have a password.  This must be unchecked.
Advanced |Store Path Type| Determines how the user will enter the store path when setting up the cert store.  Freeform
Advanced |Supports Custom Alias	|Determines if an individual entry within a store can have a custom Alias.  This must be Required
Advanced |Private Key Handling |Determines how the orchestrator deals with private keys.  Required
Advanced |PFX Password Style |Determines password style for the PFX Password. Default
Custom Fields|protocol|Name:protocol Display Name:Protocol Type:Multiple Choice (http,https) Default Value:https Required:True
Custom Fields|allowInvalidCert|Name:allowInvalidCert Display Name:Allow Invalid Cert Type:Bool Default Value:false Required:True
Entry Parameters|N/A| There are no Entry Parameters

**Basic Settings:**

![](images/CertStoreType-Basic.gif)

**Advanced Settings:**

![](images/CertStoreType-Advanced.gif)

**Custom Fields:**

![](images/CertStoreType-CustomFields.gif)

**Entry Params:**

![](images/CertStoreType-EntryParameters.gif)

**2. Register the A10 vThunder Orchestrator with Keyfactor**
See Keyfactor InstallingKeyfactorOrchestrators.pdf Documentation.  Get from your Keyfactor contact/representative.

**3. Create a A10 vThunder Certificate Store within Keyfactor Command**
In Keyfactor Command create a new Certificate Store similar to the one below

![](images/CertStore1.gif)
![](images/CertStore2.gif)

#### STORE CONFIGURATION 
CONFIG ELEMENT	|DESCRIPTION
----------------|---------------
Category	|The type of certificate store to be configured. Select category based on the display name configured above "VThunder Universal".
Container	|This is a logical grouping of like stores. This configuration is optional and does not impact the functionality of the store.
Client Machine	|The url to the vThunder api.  This file should the url and port of the vThunder api sample vThunder.test.com:1113.
Store Path	|This will be "cert".  This is not used but just hard code it as "cert".
Allow Invalid Cert|Only used for testing should be false in production.
Protocol| http is only used for testing should be https in production
Orchestrator	|This is the orchestrator server registered with the appropriate capabilities to manage this certificate store type. 
Inventory Schedule	|The interval that the system will use to report on what certificates are currently in the store. 
Use SSL	|This should be checked.
User	|This is the user name for the vThunder api to access the certficate management functionality.
Password |This is the password for the vThunder api to access the certficate management functionality.

*** 

#### Usage

**Adding New Certificate No Map Entry**

![](images/AddCertificateNoMapEntry.gif)

*** 

**Adding New Certificate With Map Entry**

![](images/AddCertificateWithMapEntry.gif)

*** 

**Replace Certficate With Map Entry**

![](images/ReplaceCertificateMapEntry.gif)

*** 

**Replace Certficate No Map Entry**

![](images/ReplaceCertificateNoMapEntry.gif)

*** 

**Replace Certficate With Map Entry**

![](images/ReplaceCertificateMapEntry.gif)

*** 

**Replace Certficate No Map Entry**

![](images/ReplaceCertificateNoMapEntry.gif)

***

**Remove Certificate Map Entry**

![](images/RemoveCertifcateMapEntry.gif)

*** 

**Remove Certficate No Map Entry**

![](images/RemoveCertificateNoMapEntry.gif)


#### TEST CASES
Case Number|Case Name|Case Description|Overwrite Flag|Alias Name|Expected Results|Passed
------------|---------|----------------|--------------|----------|----------------|--------------
1|Fresh Add With Alias|Will create new certificate and private key on the vThunder appliance|true|KeyAndCertBTest|The new KeyAndCertBTest certificate and private key will be created in the ADC/SSL Cerificates area on vThunder.|True
1a|Fresh Add With Alias|Will create new certificate and private key on the vThunder appliance|false|KeyAndCertBTest|Error Saying Overwrite Flag Needs To Be Used|True



