# a10vThunder

A10 vThunder AnyAgent allows an organization to inventory and deploy certificates in any domain that the appliance services. The AnyAgent deploys the appropriate files (.cer, .pem) within the defined directories and also performs and Inventory on the Items.

#### Integration status: Production - Ready for use in production environments.

## About the Keyfactor Universal Orchestrator Capability

This repository contains a Universal Orchestrator Capability which is a plugin to the Keyfactor Universal Orchestrator. Within the Keyfactor Platform, Orchestrators are used to manage “certificate stores” &mdash; collections of certificates and roots of trust that are found within and used by various applications.

The Universal Orchestrator is part of the Keyfactor software distribution and is available via the Keyfactor customer portal. For general instructions on installing Capabilities, see the “Keyfactor Command Orchestrator Installation and Configuration Guide” section of the Keyfactor documentation. For configuration details of this specific Capability, see below in this readme.

The Universal Orchestrator is the successor to the Windows Orchestrator. This Capability plugin only works with the Universal Orchestrator and does not work with the Windows Orchestrator.



## Support for a10vThunder

a10vThunder is supported by Keyfactor for Keyfactor customers. If you have a support issue, please open a support ticket with your Keyfactor representative.

###### To report a problem or suggest a new feature, use the **[Issues](../../issues)** tab. If you want to contribute actual bug fixes or proposed enhancements, use the **[Pull requests](../../pulls)** tab.
___



---




## Platform Specific Notes

The minimum version of the Universal Orchestrator Framework needed to run this version of the extension is 

The Keyfactor Universal Orchestrator may be installed on either Windows or Linux based platforms. The certificate operations supported by a capability may vary based what platform the capability is installed on. The table below indicates what capabilities are supported based on which platform the encompassing Universal Orchestrator is running.
| Operation | Win | Linux |
|-----|-----|------|
|Supports Management Add|&check; |&check; |
|Supports Management Remove|&check; |&check; |
|Supports Create Store|  |  |
|Supports Discovery|  |  |
|Supports Renrollment|  |  |
|Supports Inventory|&check; |&check; |



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
Basic |Custom Capability|Unchecked
Basic |Job Types	|Inventory, Add, and Remove are the supported job types. 
Basic |Needs Server	|Must be checked
Basic |Blueprint Allowed	|checked
Basic |Requires Store Password	|Determines if a store password is required when configuring an individual store.  This must be unchecked.
Basic |Supports Entry Password	|Determined if an individual entry within a store can have a password.  This must be unchecked.
Advanced |Store Path Type| Determines how the user will enter the store path when setting up the cert store.  Freeform
Advanced |Supports Custom Alias	|Determines if an individual entry within a store can have a custom Alias.  This must be Required
Advanced |Private Key Handling |Determines how the orchestrator deals with private keys.  Optional
Advanced |PFX Password Style |Determines password style for the PFX Password. Default
Custom Fields|protocol|Name:protocol Display Name:Protocol Type:Multiple Choice (http,https) Default Value:https Required:True
Custom Fields|allowInvalidCert|Name:allowInvalidCert Display Name:Allow Invalid Cert Type:Bool Default Value:false Required:True
Entry Parameters|N/A| There are no Entry Parameters

**Basic Settings:**

![](Media/Images/CertStoreType-Basic.gif)

**Advanced Settings:**

![](Media/Images/CertStoreType-Advanced.gif)

**Custom Fields:**

![](Media/Images/CertStoreType-CustomFields.gif)

**Entry Params:**

![](Media/Images/CertStoreType-EntryParameters.gif)

**2. Register the A10 vThunder Orchestrator with Keyfactor**
1. Stop the Keyfactor Universal Orchestrator Service for the orchestrator you plan to install this extension to run on.
2. In the Keyfactor Orchestrator installation folder (by convention usually C:\Program Files\Keyfactor\Keyfactor Orchestrator), find the "extensions" folder. Underneath that, create a new folder named "vThunderU". You may choose to use a different name, but then you must edit the manifest.json file downloaded from GitHub (Step 3) and modify based on Step 5 below.
3. Download the latest version of the A10 orchestrator extension from [GitHub](https://github.com/Keyfactor/a10vthunder-orchestrator).
4. Copy the contents of the download installation zip file to the folder created in Step 2.
5. (Optional) If you decided to name the folder created in Step 2 to something different than the suggested name (vThunderU), you will need to edit the manifest.json file. Modify "CertStores.{folder name}.Capability" to the folder name you created in Step 2.
6. Start the Keyfactor Universal Orchestrator Service.

Please reference the "Keyfactor Orchestrators Installation and Configuration Guide" obtainable from your Keyfactor contact/representative for more information regarding this step.

**3. Create a A10 vThunder Certificate Store within Keyfactor Command**
In Keyfactor Command create a new Certificate Store similar to the one below

![](Media/Images/CertStore1.gif)
![](Media/Images/CertStore2.gif)

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

**Adding New Certificate New Alias**

![](Media/Images/NewCertNewAlias.gif)

*** 

**Replace Cert With Same Alias**

![](Media/Images/ReplaceCertSameAlias.gif)

*** 

**Add Cert No Private Key**

![](Media/Images/AddPubCert.gif)

*** 

**Replace Cert No Private Key**

![](Media/Images/PubCertReplace.gif)

*** 

**Remove Cert No Private Key**

![](Media/Images/RemovePubCert.gif)

*** 

**Remove Cert and Private Key**

![](Media/Images/RemoveCertAndKey.gif)

*** 

**Certificate Inventory**

![](Media/Images/CertificateInventory.gif)

#### TEST CASES
Case Number|Case Name|Case Description|Overwrite Flag|Alias Name|Expected Results|Passed
------------|---------|----------------|--------------|----------|----------------|--------------
1|Fresh Add With Alias|Will create new certificate and private key on the vThunder appliance|true|KeyAndCertBTest|The new KeyAndCertBTest certificate and private key will be created in the ADC/SSL Cerificates area on vThunder.|True
1a|Replace Alias with no overwrite flag|Should warn user that a cert cannot be replaced with the same name without overwrite flag|false|KeyAndCertBTest|Error Saying Overwrite Flag Needs To Be Used|True
1b|Replace Alias with overwrite flag|Will create new certificate and private key on the vThunder appliance|true|KeyAndCertBTest|Cert will be replaced because overwrite flag was used|True
2|Add Cert Without Private Key|This will create a cert with no private key on vThunder|false|NewCertNoPk|Only Cert will be added to vThunder with no private key|True
2a|Replace Cert Without Private Key|This will Replace a cert with no private key on vThunder|true|NewCertNoPk|Only Cert will be replaced on vThunder with no private key|True
2b|Replace Cert Without Private Key no overwrite flag|Should warn user that a cert cannot be replaced with the same name without overwrite flag|false|NewCertNoPk|Error Saying Overwrite Flag Needs To Be Used|True
3|Remove Certificate and Private Key|Certificate and Private Key Will Be Removed from A10|N/A|KeyAndCertBTest|Cert and Key will be removed from vThunder and Keyfactor Store|True
3a|Remove Certificate without Private Key|Certificate Will Be Removed from A10|N/A|KeyAndCertBTest|Cert will be removed from vThunder and Keyfactor Store|True
4|Inventory Certificates with Private Key|Inventory of Certificates with private keys will be pulled from vThunder up to 125 tested|N/A|N/A|125 Certs will be inventoried, more should be supported but there is no paging in the API so limits apply|True
4a|Inventory Certificates without Private Key|Inventory of Certificates without private keys will be pulled from vThunder up to 125 tested|N/A|N/A|125 Certs will be inventoried, more should be supported but there is no paging in the API so limits apply|True



