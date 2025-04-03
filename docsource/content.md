## Overview

The Keyfactor A10 vThunder Universal Orchestrator Extension facilitates the management of SSL certificates on A10 Networks' vThunder appliances. It offers two primary certificate store types:

1. **ThunderSsl**: Manages SSL certificates for securing traffic handled by the device, such as in SSL offloading, SSL intercept, and reverse proxy configurations.
2. **ThunderMgmt**: Manages certificates securing HTTPS access to the A10 management interface (GUI/API).

### Compatibility

- **A10 Thunder Version**: Tested with A10 Thunder v6.0.5-P5 (Build 51).

### Key Features

- **Automated Certificate Deployment**: Streamlines the deployment of SSL certificates to the A10 vThunder appliance.
- **Certificate Inventory Management**: Provides visibility into existing certificates on the appliance.
- **Integration with Keyfactor Command**: Enables centralized certificate lifecycle management.

### Requirements

- **A10 vThunder Appliance**: Running firmware version 6.0.5-P5 (Build 51).
- **Keyfactor Command Platform**: For orchestrating certificate management tasks.
- **Network Connectivity**: Ensures communication between Keyfactor Command and the A10 vThunder appliance.
- **Authentication Credentials**: Necessary for interacting with the A10 vThunder API.

In an A10 Thunder device, certificates are used in different contexts depending on their role. Two commonly used types are **Management Certificates** and **SSL Certificates**.

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
