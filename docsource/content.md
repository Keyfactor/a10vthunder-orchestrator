## Overview

## 🔍 Difference Between Management Certificates and SSL Certificates on A10 Thunder

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
TC5|Setup for Unbound Renew Scenario|shared|**Alias**:<br>&lt;random&gt;<br>**Overwrite**:<br>false|Cert Installed|True|![](images/TC5.gif)  
TC5a|Renew Unbound Cert Without Overwrite|shared|**Alias**:<br>&lt;same&gt;<br>**Overwrite**:<br>false|Warning Shown: Overwrite Flag Needed|True|![](images/TC5a.gif)  
TC5b|Renew Unbound Cert With Overwrite|shared|**Alias**:<br>&lt;same&gt;<br>**Overwrite**:<br>true|Cert Overwritten Successfully|True|![](images/TC5b.gif)  
TC5c|Renew Bound Cert With Overwrite|shared|**Alias**:<br>&lt;entered-name&gt;<br>**Overwrite**:<br>true|Cert Renamed and Rebound|True|![](images/TC5c.gif)  
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
TC12|Attempt Renew/Repl Without Overwrite|/home/ec2-user|**Alias**:<br>&lt;same&gt;<br>**Overwrite**:<br>false|Fail: Overwrite Flag Missing|True|![](images/TC12.gif)  
TC13|Remove Mgmt Bound Cert|/home/ec2-user|**Alias**:<br>&lt;same&gt;<br>**Overwrite**:<br>false|Cert Removed, Binding Left Intact|True|![](images/TC13.gif)  

</details>