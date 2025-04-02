## Overview

TODO Overview is a required section

## Requirements


### Creating a User for API Access on A10 vThunder

This guide explains how to create a user on A10 vThunder for API (AXAPI) access with appropriate privileges.

#### Step-by-Step Instructions

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

#### Notes

- This user will now be able to authenticate and perform actions via A10's AXAPI (v2/v3) interface.
- Role-Based Access (RBA) and partition assignment can further fine-tune access control.

#### Example Login via AXAPI

Example using `curl` for AXAPI v3 login:
```bash
curl -X POST https://<vThunder-IP>/axapi/v3/auth \
  -d '{"credentials":{"username":"apiuser","password":"yourStrongPassword"}}' \
  -H "Content-Type: application/json"
```



## Post Installation

TODO Post Installation is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info

## Discovery

TODO Discovery is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info

