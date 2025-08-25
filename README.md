<h1 align="center" style="border-bottom: none">
    a10vThunder Universal Orchestrator Extension
</h1>

<p align="center">
  <!-- Badges -->
<img src="https://img.shields.io/badge/integration_status-production-3D1973?style=flat-square" alt="Integration Status: production" />
<a href="https://github.com/Keyfactor/a10vthunder-orchestrator/releases"><img src="https://img.shields.io/github/v/release/Keyfactor/a10vthunder-orchestrator?style=flat-square" alt="Release" /></a>
<img src="https://img.shields.io/github/issues/Keyfactor/a10vthunder-orchestrator?style=flat-square" alt="Issues" />
<img src="https://img.shields.io/github/downloads/Keyfactor/a10vthunder-orchestrator/total?style=flat-square&label=downloads&color=28B905" alt="GitHub Downloads (all assets, all releases)" />
</p>

<p align="center">
  <!-- TOC -->
  <a href="#support">
    <b>Support</b>
  </a>
  ·
  <a href="#installation">
    <b>Installation</b>
  </a>
  ·
  <a href="#license">
    <b>License</b>
  </a>
  ·
  <a href="https://github.com/orgs/Keyfactor/repositories?q=orchestrator">
    <b>Related Integrations</b>
  </a>
</p>

## Overview

The Keyfactor A10 vThunder Universal Orchestrator Extension facilitates the management of SSL certificates on A10 Networks' vThunder appliances. It offers two primary certificate store types:

1. **ThunderSsl**: Manages SSL certificates for securing traffic handled by the device, such as in SSL offloading, SSL intercept, and reverse proxy configurations.
2. **ThunderMgmt**: Manages certificates securing HTTPS access to the A10 management interface (GUI/API).

- **A10 Thunder Version**: Tested with A10 Thunder v6.0.5-P5 (Build 51).
- **Automated Certificate Deployment**: Streamlines the deployment of SSL certificates to the A10 vThunder appliance.
- **Certificate Inventory Management**: Provides visibility into existing certificates on the appliance.
- **Integration with Keyfactor Command**: Enables centralized certificate lifecycle management.

The a10vThunder Universal Orchestrator extension implements 2 Certificate Store Types. Depending on your use case, you may elect to use one, or both of these Certificate Store Types. Descriptions of each are provided below.

- [A10 Thunder Ssl Certificates](#ThunderSsl)

- [A10 Thunder Management Certificates](#ThunderMgmt)


## Compatibility

This integration is compatible with Keyfactor Universal Orchestrator version 10.1 and later.

## Support
The a10vThunder Universal Orchestrator extension is supported by Keyfactor. If you require support for any issues or have feature request, please open a support ticket by either contacting your Keyfactor representative or via the Keyfactor Support Portal at https://support.keyfactor.com.

> If you want to contribute bug fixes or additional enhancements, use the **[Pull requests](../../pulls)** tab.

## Requirements & Prerequisites

Before installing the a10vThunder Universal Orchestrator extension, we recommend that you install [kfutil](https://github.com/Keyfactor/kfutil). Kfutil is a command-line tool that simplifies the process of creating store types, installing extensions, and instantiating certificate stores in Keyfactor Command.



## Certificate Store Types

To use the a10vThunder Universal Orchestrator extension, you **must** create the Certificate Store Types required for your use-case. This only needs to happen _once_ per Keyfactor Command instance.

The a10vThunder Universal Orchestrator extension implements 2 Certificate Store Types. Depending on your use case, you may elect to use one, or both of these Certificate Store Types.

### ThunderSsl

<details><summary>Click to expand details</summary>


#### 🔒 SSL Certificates

**Purpose:**  
Used for securing traffic that passes through the device (i.e., traffic handled by SLB/ADC features).

**Usage Context:**  
- SSL Offloading
- SSL Intercept (Decryption/Encryption)
- Reverse proxy configurations

**Configured In:**  
- **GUI:** `ADC → Ssl Management


**Example:**  
If the A10 is acting as an SSL offloader for a backend web server, the **SSL Certificate** is used to terminate client HTTPS sessions.




#### A10 Thunder Ssl Certificates Requirements

#### Creating a User for API Access on A10 vThunder

This guide explains how to create a user on A10 vThunder for API (AXAPI) access with appropriate privileges.

##### Step-by-Step Instructions

1. **Enter configuration mode:**
   ```bash
   configure terminal
   ```

2. **Create the user and set a password:**
   ```bash
   admin apiuser password yourStrongPassword
   ```

   Replace `apiuser` with the desired username, and `yourStrongPassword` with a secure password.

3. **Assign necessary privileges:**
   ```bash
   privilege read
   privilege write
   privilege partition-enable-disable
   privilege partition-read
   privilege partition-write
   ```

   These privileges grant the user:
   - Global read and write access
   - Per-partition read and write access
   - Permission to enable or disable partitions

4. **(Optional) Enable external health monitor privilege (if needed):**
   ```bash
   privilege hm
   ```

5. **Exit user configuration:**
   ```bash
   exit
   ```

##### Notes

- This user will now be able to authenticate and perform actions via A10's AXAPI (v2/v3) interface.
- Role-Based Access (RBA) and partition assignment can further fine-tune access control.

##### Example Login via AXAPI

Example using `curl` for AXAPI v3 login:
```bash
curl -X POST https://<vThunder-IP>/axapi/v3/auth \
  -d '{"credentials":{"username":"apiuser","password":"yourStrongPassword"}}' \
  -H "Content-Type: application/json"
```



#### Supported Operations

| Operation    | Is Supported                                                                                                           |
|--------------|------------------------------------------------------------------------------------------------------------------------|
| Add          | ✅ Checked        |
| Remove       | ✅ Checked     |
| Discovery    | 🔲 Unchecked  |
| Reenrollment | ✅ Checked |
| Create       | 🔲 Unchecked     |

#### Store Type Creation

##### Using kfutil:
`kfutil` is a custom CLI for the Keyfactor Command API and can be used to create certificate store types.
For more information on [kfutil](https://github.com/Keyfactor/kfutil) check out the [docs](https://github.com/Keyfactor/kfutil?tab=readme-ov-file#quickstart)
   <details><summary>Click to expand ThunderSsl kfutil details</summary>

   ##### Using online definition from GitHub:
   This will reach out to GitHub and pull the latest store-type definition
   ```shell
   # A10 Thunder Ssl Certificates
   kfutil store-types create ThunderSsl
   ```

   ##### Offline creation using integration-manifest file:
   If required, it is possible to create store types from the [integration-manifest.json](./integration-manifest.json) included in this repo.
   You would first download the [integration-manifest.json](./integration-manifest.json) and then run the following command
   in your offline environment.
   ```shell
   kfutil store-types create --from-file integration-manifest.json
   ```
   </details>


#### Manual Creation
Below are instructions on how to create the ThunderSsl store type manually in
the Keyfactor Command Portal
   <details><summary>Click to expand manual ThunderSsl details</summary>

   Create a store type called `ThunderSsl` with the attributes in the tables below:

   ##### Basic Tab
   | Attribute | Value | Description |
   | --------- | ----- | ----- |
   | Name | A10 Thunder Ssl Certificates | Display name for the store type (may be customized) |
   | Short Name | ThunderSsl | Short display name for the store type |
   | Capability | ThunderSsl | Store type name orchestrator will register with. Check the box to allow entry of value |
   | Supports Add | ✅ Checked | Check the box. Indicates that the Store Type supports Management Add |
   | Supports Remove | ✅ Checked | Check the box. Indicates that the Store Type supports Management Remove |
   | Supports Discovery | 🔲 Unchecked |  Indicates that the Store Type supports Discovery |
   | Supports Reenrollment | ✅ Checked |  Indicates that the Store Type supports Reenrollment |
   | Supports Create | 🔲 Unchecked |  Indicates that the Store Type supports store creation |
   | Needs Server | ✅ Checked | Determines if a target server name is required when creating store |
   | Blueprint Allowed | 🔲 Unchecked | Determines if store type may be included in an Orchestrator blueprint |
   | Uses PowerShell | 🔲 Unchecked | Determines if underlying implementation is PowerShell |
   | Requires Store Password | 🔲 Unchecked | Enables users to optionally specify a store password when defining a Certificate Store. |
   | Supports Entry Password | 🔲 Unchecked | Determines if an individual entry within a store can have a password. |

   The Basic tab should look like this:

   ![ThunderSsl Basic Tab](docsource/images/ThunderSsl-basic-store-type-dialog.png)

   ##### Advanced Tab
   | Attribute | Value | Description |
   | --------- | ----- | ----- |
   | Supports Custom Alias | Forbidden | Determines if an individual entry within a store can have a custom Alias. |
   | Private Key Handling | Optional | This determines if Keyfactor can send the private key associated with a certificate to the store. Required because IIS certificates without private keys would be invalid. |
   | PFX Password Style | Default | 'Default' - PFX password is randomly generated, 'Custom' - PFX password may be specified when the enrollment job is created (Requires the Allow Custom Password application setting to be enabled.) |

   The Advanced tab should look like this:

   ![ThunderSsl Advanced Tab](docsource/images/ThunderSsl-advanced-store-type-dialog.png)

   > For Keyfactor **Command versions 24.4 and later**, a Certificate Format dropdown is available with PFX and PEM options. Ensure that **PFX** is selected, as this determines the format of new and renewed certificates sent to the Orchestrator during a Management job. Currently, all Keyfactor-supported Orchestrator extensions support only PFX.

   ##### Custom Fields Tab
   Custom fields operate at the certificate store level and are used to control how the orchestrator connects to the remote target server containing the certificate store to be managed. The following custom fields should be added to the store type:

   | Name | Display Name | Description | Type | Default Value/Options | Required |
   | ---- | ------------ | ---- | --------------------- | -------- | ----------- |
   | allowInvalidCert | Allow Invalid Cert on A10 Management API |  | Bool | true | ✅ Checked |

   The Custom Fields tab should look like this:

   ![ThunderSsl Custom Fields Tab](docsource/images/ThunderSsl-custom-fields-store-type-dialog.png)

   </details>
</details>

### ThunderMgmt

<details><summary>Click to expand details</summary>


#### 🔐 Management Certificates

**Purpose:**  
Used to secure HTTPS access to the A10 management interface (GUI/API).

**Usage Context:**  
- AXAPI (API access over HTTPS)
- Web GUI login
- Any administrative HTTPS session

**Configured In:**  
- **GUI:** `System → Settings → Certificate`

**Example:**  
When a user logs into the GUI via `https://<device_ip>`, the certificate presented is the **Management Certificate**.




#### A10 Thunder Management Certificates Requirements

#### A10 Certificate Management Orchestrator Extension

This orchestrator extension automates the process of uploading, inventorying, and deploying SSL certificates from a Linux SCP server to an A10 vThunder device. Due to A10 API limitations, certificates must be pulled from the SCP server directly by the A10 device itself.

---

##### 📌 How It Works

1. **The orchestrator** connects to a Linux server via SCP to inventory available certificates.
2. It stores relevant metadata and pushes new certificates and keys to the SCP server.
3. It then instructs the **A10 device** to retrieve the certificate and private key from the Linux server using API calls.
4. The A10 device loads the certificate and key directly from the SCP server for use on its **management interface**.

---

##### 📡 API Call Example (From A10 Device)

```http
POST /axapi/v3/web-service/secure/certificate
```

**Payload:**
```json
{
  "certificate": {
    "load": 1,
    "file-url": "scp://ec2-user:dda@172.31.93.107:/home/ec2-user/26125.crt"
  }
}
```

> A similar call is made for loading the private key onto the A10 device using a separate AXAPI endpoint.

- The A10 device **must have access** to the SCP server via the specified IP (`A10ToScpServerIp`).
- Ensure the certificate and key file paths are correct and accessible to the SCP user.

---

##### 🔐 Linux Server Requirements

###### User Access
- The SCP user (`ScpUserName`, e.g., `ec2-user`) must:
  - Have SSH/SCP access.
  - Authenticate with a password.
  - Have **read and write** permissions in the SCP location.

> New certificates and **private keys** are generated by Keyfactor and uploaded to this location by the orchestrator. Therefore, write access is essential.

###### SCP Directory Permissions
- Ensure the directory (e.g., `/home/ec2-user/`) is:
  - Writable by the orchestrator (to upload new certs/keys).
  - Readable by both the orchestrator and the A10 device (via SCP).

---

##### 🔄 Alternate Design Consideration

It may be possible to use the A10 device itself as the SCP target location if it supports read/write SCP operations **outside the CLI context**. However, A10 devices typically restrict file access through CLI or API mechanisms only, and not through standard SCP server operations. This limitation is why a separate Linux SCP server is currently required.

---

##### 🔓 Network and Port Requirements

| Source             | Destination         | Port | Protocol | Purpose                       |
|--------------------|---------------------|------|----------|-------------------------------|
| Orchestrator       | Linux SCP Server    | 22   | TCP      | Inventory and upload via SCP  |
| A10 Device         | Linux SCP Server    | 22   | TCP      | Cert and key retrieval via SCP|
| Orchestrator/Admin | A10 Device (API)    | 443  | HTTPS    | API calls to load certificate |

---

##### ✅ Summary

This extension coordinates certificate and private key delivery by using SCP as a bridge between orchestrator logic and A10's strict API requirements. It ensures secure and automated deployment for the management interface certificates with minimal manual intervention.



#### Supported Operations

| Operation    | Is Supported                                                                                                           |
|--------------|------------------------------------------------------------------------------------------------------------------------|
| Add          | ✅ Checked        |
| Remove       | ✅ Checked     |
| Discovery    | 🔲 Unchecked  |
| Reenrollment | ✅ Checked |
| Create       | 🔲 Unchecked     |

#### Store Type Creation

##### Using kfutil:
`kfutil` is a custom CLI for the Keyfactor Command API and can be used to create certificate store types.
For more information on [kfutil](https://github.com/Keyfactor/kfutil) check out the [docs](https://github.com/Keyfactor/kfutil?tab=readme-ov-file#quickstart)
   <details><summary>Click to expand ThunderMgmt kfutil details</summary>

   ##### Using online definition from GitHub:
   This will reach out to GitHub and pull the latest store-type definition
   ```shell
   # A10 Thunder Management Certificates
   kfutil store-types create ThunderMgmt
   ```

   ##### Offline creation using integration-manifest file:
   If required, it is possible to create store types from the [integration-manifest.json](./integration-manifest.json) included in this repo.
   You would first download the [integration-manifest.json](./integration-manifest.json) and then run the following command
   in your offline environment.
   ```shell
   kfutil store-types create --from-file integration-manifest.json
   ```
   </details>


#### Manual Creation
Below are instructions on how to create the ThunderMgmt store type manually in
the Keyfactor Command Portal
   <details><summary>Click to expand manual ThunderMgmt details</summary>

   Create a store type called `ThunderMgmt` with the attributes in the tables below:

   ##### Basic Tab
   | Attribute | Value | Description |
   | --------- | ----- | ----- |
   | Name | A10 Thunder Management Certificates | Display name for the store type (may be customized) |
   | Short Name | ThunderMgmt | Short display name for the store type |
   | Capability | ThunderMgmt | Store type name orchestrator will register with. Check the box to allow entry of value |
   | Supports Add | ✅ Checked | Check the box. Indicates that the Store Type supports Management Add |
   | Supports Remove | ✅ Checked | Check the box. Indicates that the Store Type supports Management Remove |
   | Supports Discovery | 🔲 Unchecked |  Indicates that the Store Type supports Discovery |
   | Supports Reenrollment | ✅ Checked |  Indicates that the Store Type supports Reenrollment |
   | Supports Create | 🔲 Unchecked |  Indicates that the Store Type supports store creation |
   | Needs Server | ✅ Checked | Determines if a target server name is required when creating store |
   | Blueprint Allowed | 🔲 Unchecked | Determines if store type may be included in an Orchestrator blueprint |
   | Uses PowerShell | 🔲 Unchecked | Determines if underlying implementation is PowerShell |
   | Requires Store Password | 🔲 Unchecked | Enables users to optionally specify a store password when defining a Certificate Store. |
   | Supports Entry Password | 🔲 Unchecked | Determines if an individual entry within a store can have a password. |

   The Basic tab should look like this:

   ![ThunderMgmt Basic Tab](docsource/images/ThunderMgmt-basic-store-type-dialog.png)

   ##### Advanced Tab
   | Attribute | Value | Description |
   | --------- | ----- | ----- |
   | Supports Custom Alias | Forbidden | Determines if an individual entry within a store can have a custom Alias. |
   | Private Key Handling | Required | This determines if Keyfactor can send the private key associated with a certificate to the store. Required because IIS certificates without private keys would be invalid. |
   | PFX Password Style | Default | 'Default' - PFX password is randomly generated, 'Custom' - PFX password may be specified when the enrollment job is created (Requires the Allow Custom Password application setting to be enabled.) |

   The Advanced tab should look like this:

   ![ThunderMgmt Advanced Tab](docsource/images/ThunderMgmt-advanced-store-type-dialog.png)

   > For Keyfactor **Command versions 24.4 and later**, a Certificate Format dropdown is available with PFX and PEM options. Ensure that **PFX** is selected, as this determines the format of new and renewed certificates sent to the Orchestrator during a Management job. Currently, all Keyfactor-supported Orchestrator extensions support only PFX.

   ##### Custom Fields Tab
   Custom fields operate at the certificate store level and are used to control how the orchestrator connects to the remote target server containing the certificate store to be managed. The following custom fields should be added to the store type:

   | Name | Display Name | Description | Type | Default Value/Options | Required |
   | ---- | ------------ | ---- | --------------------- | -------- | ----------- |
   | OrchToScpServerIp | Orch To Scp Server Ip |  | String |  | ✅ Checked |
   | ScpPort | Port Used For Scp |  | String |  | ✅ Checked |
   | ScpUserName | UserName Used For Scp |  | Secret |  | ✅ Checked |
   | ScpPassword | Password Used For Scp |  | Secret |  | ✅ Checked |
   | A10ToScpServerIp | A10 Device To Scp Server Ip |  | String |  | ✅ Checked |
   | allowInvalidCert | Allow Invalid Cert on A10 Management API |  | Bool | true | ✅ Checked |

   The Custom Fields tab should look like this:

   ![ThunderMgmt Custom Fields Tab](docsource/images/ThunderMgmt-custom-fields-store-type-dialog.png)

   </details>
</details>


## Installation

1. **Download the latest a10vThunder Universal Orchestrator extension from GitHub.**

    Navigate to the [a10vThunder Universal Orchestrator extension GitHub version page](https://github.com/Keyfactor/a10vthunder-orchestrator/releases/latest). Refer to the compatibility matrix below to determine whether the `net6.0` or `net8.0` asset should be downloaded. Then, click the corresponding asset to download the zip archive.

   | Universal Orchestrator Version | Latest .NET version installed on the Universal Orchestrator server | `rollForward` condition in `Orchestrator.runtimeconfig.json` | `a10vthunder-orchestrator` .NET version to download |
   | --------- | ----------- | ----------- | ----------- |
   | Older than `11.0.0` | | | `net6.0` |
   | Between `11.0.0` and `11.5.1` (inclusive) | `net6.0` | | `net6.0` |
   | Between `11.0.0` and `11.5.1` (inclusive) | `net8.0` | `Disable` | `net6.0` |
   | Between `11.0.0` and `11.5.1` (inclusive) | `net8.0` | `LatestMajor` | `net8.0` |
   | `11.6` _and_ newer | `net8.0` | | `net8.0` |

    Unzip the archive containing extension assemblies to a known location.

    > **Note** If you don't see an asset with a corresponding .NET version, you should always assume that it was compiled for `net6.0`.

2. **Locate the Universal Orchestrator extensions directory.**

    * **Default on Windows** - `C:\Program Files\Keyfactor\Keyfactor Orchestrator\extensions`
    * **Default on Linux** - `/opt/keyfactor/orchestrator/extensions`

3. **Create a new directory for the a10vThunder Universal Orchestrator extension inside the extensions directory.**

    Create a new directory called `a10vthunder-orchestrator`.
    > The directory name does not need to match any names used elsewhere; it just has to be unique within the extensions directory.

4. **Copy the contents of the downloaded and unzipped assemblies from __step 2__ to the `a10vthunder-orchestrator` directory.**

5. **Restart the Universal Orchestrator service.**

    Refer to [Starting/Restarting the Universal Orchestrator service](https://software.keyfactor.com/Core-OnPrem/Current/Content/InstallingAgents/NetCoreOrchestrator/StarttheService.htm).


6. **(optional) PAM Integration**

    The a10vThunder Universal Orchestrator extension is compatible with all supported Keyfactor PAM extensions to resolve PAM-eligible secrets. PAM extensions running on Universal Orchestrators enable secure retrieval of secrets from a connected PAM provider.

    To configure a PAM provider, [reference the Keyfactor Integration Catalog](https://keyfactor.github.io/integrations-catalog/content/pam) to select an extension and follow the associated instructions to install it on the Universal Orchestrator (remote).


> The above installation steps can be supplemented by the [official Command documentation](https://software.keyfactor.com/Core-OnPrem/Current/Content/InstallingAgents/NetCoreOrchestrator/CustomExtensions.htm?Highlight=extensions).



## Defining Certificate Stores

The a10vThunder Universal Orchestrator extension implements 2 Certificate Store Types, each of which implements different functionality. Refer to the individual instructions below for each Certificate Store Type that you deemed necessary for your use case from the installation section.

<details><summary>A10 Thunder Ssl Certificates (ThunderSsl)</summary>

### ⚙️ Configuration Fields

| Name              | Display Name                  | Description                                                  | Type   | Required |
|-------------------|-------------------------------|--------------------------------------------------------------|--------|----------|
| allowInvalidCert  | Allow Invalid Cert on A10 API | If true, allows self-signed/untrusted certs for A10 API access | Bool   | ✅ (default: true) |


### Store Creation

#### Manually with the Command UI

<details><summary>Click to expand details</summary>

1. **Navigate to the _Certificate Stores_ page in Keyfactor Command.**

    Log into Keyfactor Command, toggle the _Locations_ dropdown, and click _Certificate Stores_.

2. **Add a Certificate Store.**

    Click the Add button to add a new Certificate Store. Use the table below to populate the **Attributes** in the **Add** form.

   | Attribute | Description                                             |
   | --------- |---------------------------------------------------------|
   | Category | Select "A10 Thunder Ssl Certificates" or the customized certificate store name from the previous step. |
   | Container | Optional container to associate certificate store with. |
   | Client Machine |  |
   | Store Path |  |
   | Orchestrator | Select an approved orchestrator capable of managing `ThunderSsl` certificates. Specifically, one with the `ThunderSsl` capability. |
   | allowInvalidCert |  |

</details>



#### Using kfutil CLI

<details><summary>Click to expand details</summary>

1. **Generate a CSV template for the ThunderSsl certificate store**

    ```shell
    kfutil stores import generate-template --store-type-name ThunderSsl --outpath ThunderSsl.csv
    ```
2. **Populate the generated CSV file**

    Open the CSV file, and reference the table below to populate parameters for each **Attribute**.

   | Attribute | Description |
   | --------- | ----------- |
   | Category | Select "A10 Thunder Ssl Certificates" or the customized certificate store name from the previous step. |
   | Container | Optional container to associate certificate store with. |
   | Client Machine |  |
   | Store Path |  |
   | Orchestrator | Select an approved orchestrator capable of managing `ThunderSsl` certificates. Specifically, one with the `ThunderSsl` capability. |
   | Properties.allowInvalidCert |  |

3. **Import the CSV file to create the certificate stores**

    ```shell
    kfutil stores import csv --store-type-name ThunderSsl --file ThunderSsl.csv
    ```

</details>


#### PAM Provider Eligible Fields
<details><summary>Attributes eligible for retrieval by a PAM Provider on the Universal Orchestrator</summary>

If a PAM provider was installed _on the Universal Orchestrator_ in the [Installation](#Installation) section, the following parameters can be configured for retrieval _on the Universal Orchestrator_.

   | Attribute | Description |
   | --------- | ----------- |
   | ServerUsername | Username to use when connecting to server |
   | ServerPassword | Password to use when connecting to server |

Please refer to the **Universal Orchestrator (remote)** usage section ([PAM providers on the Keyfactor Integration Catalog](https://keyfactor.github.io/integrations-catalog/content/pam)) for your selected PAM provider for instructions on how to load attributes orchestrator-side.
> Any secret can be rendered by a PAM provider _installed on the Keyfactor Command server_. The above parameters are specific to attributes that can be fetched by an installed PAM provider running on the Universal Orchestrator server itself.

</details>


> The content in this section can be supplemented by the [official Command documentation](https://software.keyfactor.com/Core-OnPrem/Current/Content/ReferenceGuide/Certificate%20Stores.htm?Highlight=certificate%20store).


</details>

<details><summary>A10 Thunder Management Certificates (ThunderMgmt)</summary>

### ⚙️ Configuration Fields

| Name              | Display Name                  | Description                                                  | Type   | Required |
|-------------------|-------------------------------|--------------------------------------------------------------|--------|----------|
| OrchToScpServerIp | Orch To Scp Server IP         | IP from the orchestrator to the SCP Linux server             | String | ✅        |
| ScpPort           | Port Used For SCP             | Port used to connect to the SCP server                       | String | ✅        |
| ScpUserName       | Username Used For SCP         | Username for SCP access on the Linux server                  | Secret | ✅        |
| ScpPassword       | Password Used For SCP         | Password for SCP access on the Linux server                  | Secret | ✅        |
| A10ToScpServerIp  | A10 Device To SCP Server IP   | IP used by the A10 device to reach the SCP server (can be private) | String | ✅   |
| allowInvalidCert  | Allow Invalid Cert on A10 API | If true, allows self-signed/untrusted certs for A10 API access | Bool   | ✅ (default: true) |


### Store Creation

#### Manually with the Command UI

<details><summary>Click to expand details</summary>

1. **Navigate to the _Certificate Stores_ page in Keyfactor Command.**

    Log into Keyfactor Command, toggle the _Locations_ dropdown, and click _Certificate Stores_.

2. **Add a Certificate Store.**

    Click the Add button to add a new Certificate Store. Use the table below to populate the **Attributes** in the **Add** form.

   | Attribute | Description                                             |
   | --------- |---------------------------------------------------------|
   | Category | Select "A10 Thunder Management Certificates" or the customized certificate store name from the previous step. |
   | Container | Optional container to associate certificate store with. |
   | Client Machine |  |
   | Store Path |  |
   | Orchestrator | Select an approved orchestrator capable of managing `ThunderMgmt` certificates. Specifically, one with the `ThunderMgmt` capability. |
   | OrchToScpServerIp |  |
   | ScpPort |  |
   | ScpUserName |  |
   | ScpPassword |  |
   | A10ToScpServerIp |  |
   | allowInvalidCert |  |

</details>



#### Using kfutil CLI

<details><summary>Click to expand details</summary>

1. **Generate a CSV template for the ThunderMgmt certificate store**

    ```shell
    kfutil stores import generate-template --store-type-name ThunderMgmt --outpath ThunderMgmt.csv
    ```
2. **Populate the generated CSV file**

    Open the CSV file, and reference the table below to populate parameters for each **Attribute**.

   | Attribute | Description |
   | --------- | ----------- |
   | Category | Select "A10 Thunder Management Certificates" or the customized certificate store name from the previous step. |
   | Container | Optional container to associate certificate store with. |
   | Client Machine |  |
   | Store Path |  |
   | Orchestrator | Select an approved orchestrator capable of managing `ThunderMgmt` certificates. Specifically, one with the `ThunderMgmt` capability. |
   | Properties.OrchToScpServerIp |  |
   | Properties.ScpPort |  |
   | Properties.ScpUserName |  |
   | Properties.ScpPassword |  |
   | Properties.A10ToScpServerIp |  |
   | Properties.allowInvalidCert |  |

3. **Import the CSV file to create the certificate stores**

    ```shell
    kfutil stores import csv --store-type-name ThunderMgmt --file ThunderMgmt.csv
    ```

</details>


#### PAM Provider Eligible Fields
<details><summary>Attributes eligible for retrieval by a PAM Provider on the Universal Orchestrator</summary>

If a PAM provider was installed _on the Universal Orchestrator_ in the [Installation](#Installation) section, the following parameters can be configured for retrieval _on the Universal Orchestrator_.

   | Attribute | Description |
   | --------- | ----------- |
   | ServerUsername | Username to use when connecting to server |
   | ServerPassword | Password to use when connecting to server |
   | ScpUserName |  |
   | ScpPassword |  |

Please refer to the **Universal Orchestrator (remote)** usage section ([PAM providers on the Keyfactor Integration Catalog](https://keyfactor.github.io/integrations-catalog/content/pam)) for your selected PAM provider for instructions on how to load attributes orchestrator-side.
> Any secret can be rendered by a PAM provider _installed on the Keyfactor Command server_. The above parameters are specific to attributes that can be fetched by an installed PAM provider running on the Universal Orchestrator server itself.

</details>


> The content in this section can be supplemented by the [official Command documentation](https://software.keyfactor.com/Core-OnPrem/Current/Content/ReferenceGuide/Certificate%20Stores.htm?Highlight=certificate%20store).


</details>



## Test Cases
<details>  
<summary>Management & SSL Certificate Operations</summary>  

Case Number|Case Name|Store Path|Enrollment Params|Expected Results|Passed|Screenshots  
-------|----------|------------------|--------------------|----------------------------|----|--------  
TC1|Add Unbound Certificate|shared|**Alias**:<br>&lt;random&gt;<br>**Overwrite**:<br>false|Cert and Chain Added|True|![](images/TC1.gif)  
TC2|Remove Unbound Certificate|shared|**Alias**:<br>&lt;same-random&gt;<br>**Overwrite**:<br>false|Cert Removed Successfully|True|![](images/TC2.gif)  
TC3|Add Certificate to New Partition|keyfactor2|**Alias**:<br>&lt;random&gt;<br>**Overwrite**:<br>false|Cert Added to Partition|True|![](images/TC3.gif)  
TC4|Remove Cert from Partition|keyfactor2|**Alias**:<br>&lt;same-random&gt;<br>**Overwrite**:<br>false|Cert Removed from Partition|True|![](images/TC4.gif)  
TC5|Renew Bound Cert With Overwrite|shared|**Alias**:<br>&lt;entered-name&gt;<br>**Overwrite**:<br>true|Cert Renamed and Rebound|True|![](images/TC5.gif)  
TC6|Renew Bound Cert in Different Partition|keyfactor2|**Alias**:<br>&lt;entered-name&gt;<br>**Overwrite**:<br>true|Cert Renamed and Rebound|True|![](images/TC6.gif)  
TC7|Attempt to Remove Bound Cert (Not Allowed)|keyfactor2|**Alias**:<br>&lt;entered-name&gt;<br>**Overwrite**:<br>true|Error: Bound Cert Cannot Be Removed|True|![](images/TC7.gif)  

</details>

<details>  
<summary>Inventory Tests</summary>  

Case Number|Case Name|Store Path|Enrollment Params|Expected Results|Passed|Screenshots  
-------|----------|------------------|--------------------|----------------------------|----|--------  
TC8|Inventory From Partition|keyfactor2|—|Partition Certs Listed|True|![](images/TC8.gif)  
TC9|Inventory From Shared Location|shared|—|Shared Certs Listed|True|![](images/TC9.gif)  
TC14|Inventory Management Certs from SCP|/home/ec2-user|—|Mgmt Certs Retrieved from SCP|True|![](images/TC14.gif)  

</details>

<details>  
<summary>Management Port Binding</summary>  

Case Number|Case Name|Store Path|Enrollment Params|Expected Results|Passed|Screenshots  
-------|----------|------------------|--------------------|----------------------------|----|--------  
TC10|Add New Cert and Bind to Mgmt Port|/home/ec2-user|**Alias**:<br>&lt;random&gt;<br>**Overwrite**:<br>true|Cert Installed and Bound|True|![](images/TC10.gif)  
TC11|Renew and Rebind Cert to Mgmt Port|/home/ec2-user|**Alias**:<br>&lt;same&gt;<br>**Overwrite**:<br>true|Cert Renewed and Bound Again|True|![](images/TC11.gif)  
TC13|Remove Mgmt Bound Cert|/home/ec2-user|**Alias**:<br>&lt;same&gt;<br>**Overwrite**:<br>false|Cert Removed, Binding Left Intact|True|![](images/TC13.gif)  

</details>


## License

Apache License 2.0, see [LICENSE](LICENSE).

## Related Integrations

See all [Keyfactor Universal Orchestrator extensions](https://github.com/orgs/Keyfactor/repositories?q=orchestrator).