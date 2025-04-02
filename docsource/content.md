## Overview

TODO Overview is a required section

## Requirements

### 🔐 Setting Up API User and Access on A10 Thunder

This section explains how to configure an API user and enable API (AXAPI) access on an A10 Thunder device using the CLI.

#### ✅ Prerequisites

- Admin credentials for the A10 Thunder device  
- SSH access to the device  
- Familiarity with A10 CLI commands

---

#### 🧑‍💻 Step 1: Create an API User

1. SSH into the A10 Thunder device:

   ```bash
   ssh admin@<DEVICE_IP>
   ```

2. Enter configuration mode:

   ```bash
   configure terminal
   ```

3. Create a user account for API access:

   ```bash
   username <api_user> password <secure_password>
   ```

4. Assign admin privileges:

   ```bash
   username <api_user> privilege 15
   ```

5. (Optional) Assign a role if using Role-Based Access Control (RBAC):

   ```bash
   username <api_user> role <role_name>
   ```

6. Save the configuration:

   ```bash
   write memory
   ```

---

#### 🌐 Step 2: Enable and Verify API Access

A10 Thunder supports AXAPI, a REST-based API. Follow these steps to confirm access:

1. Ensure the management interface allows API traffic (if access-lists are used):

   ```bash
   ip access-list standard mgmt
     permit <MGMT_SUBNET> <SUBNET_MASK>
   exit

   interface management
     access-list mgmt
   ```

2. (Optional) Bind SSL cert for secure access:

   ```bash
   slb ssl-cert <CERT_NAME>
     key <KEY_FILE>
     certificate <CERT_FILE>
   ```

3. Authenticate using AXAPI v3 (example using `curl`):

   ```bash
   curl -k -X POST https://<DEVICE_IP>/axapi/v3/auth \
     -H "Content-Type: application/json" \
     -d '{"credentials": {"username": "<api_user>", "password": "<password>"}}'
   ```

   A successful response will include an `authresponse` with an authorization token.

---

#### 🔁 Step 3: Use the API Token

Use the returned token for authorized API calls:

```bash
curl -k -X GET https://<DEVICE_IP>/axapi/v3/system/resource-usage \
  -H "Authorization: A10 <AUTH_TOKEN>"
```

---

#### 📌 Notes

- You can also create users via the GUI:  
  **System → Admin → Users**

- Roles (for RBAC) can be managed under:  
  **System → Admin → Role**

- Always use HTTPS and avoid hardcoding credentials in scripts

---

#### 📚 Resources

- [AXAPI Documentation](https://support.a10networks.com/)
- [A10 Thunder CLI Reference Guide](https://docs.a10networks.com/)


## Post Installation

TODO Post Installation is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info

## Discovery

TODO Discovery is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info

