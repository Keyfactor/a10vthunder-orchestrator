## Overview

### 🔒 SSL Certificates

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

#### ThunderSsl Aliases

In the ThunderSsl store type, the **alias** directly corresponds to the certificate and private key names stored on the A10 appliance:

- **Certificate Name**: The alias becomes the SSL certificate identifier in A10's certificate store
- **Private Key Name**: The same alias is used for the associated private key
- **Template References**: SSL templates reference certificates by this exact alias name
- **API Operations**: All A10 API calls use this alias to identify the certificate/key pair

##### Example ThunderSsl Usage
```
Alias: "webserver-prod-2025"
→ A10 Certificate: "webserver-prod-2025"  
→ A10 Private Key: "webserver-prod-2025"
→ Template Reference: server-ssl template uses cert "webserver-prod-2025"
```

##### Alias Renaming for Template-Bound Certificates

When replacing a certificate that's bound to SSL templates, the orchestrator uses an intelligent renaming strategy:

1. **Timestamp Generation**: Creates a Unix timestamp (10 digits)
2. **Alias Pattern Matching**: 
   - If alias contains existing timestamp: `webserver-prod_1640995200` → `webserver-prod_1672531200`
   - If no timestamp found: `webserver-prod` → `webserver-prod_1672531200`
3. **Length Validation**: Ensures final alias stays within A10's 240-character limit
4. **Template Updates**: All SSL templates are updated to reference the new timestamped alias
5. **Cleanup**: Original certificate is removed after successful template updates

##### Replacement Workflow Example
```
Original: "api-gateway-cert"
Step 1: Generate new alias → "api-gateway-cert_1672531200"  
Step 2: Upload certificate with new alias
Step 3: Update server-ssl templates: cert "api-gateway-cert" → "api-gateway-cert_1672531200"
Step 4: Update client-ssl templates: cert "api-gateway-cert" → "api-gateway-cert_1672531200"  
Step 5: Remove old certificate "api-gateway-cert"
Step 6: Rebind templates to virtual services
```



##### Alias Best Practices
- Use descriptive names that indicate purpose: `web-frontend-ssl`, `api-backend-tls`
- Avoid special characters that might conflict with A10 naming rules
- Consider including environment indicators: `prod-web-cert`, `stage-api-cert`
- Remember that renaming will append timestamps for template-bound certificates



##### Character Limitations
- **Maximum Length**: 240 characters (enforced by orchestrator)
- **Recommended Characters**: Letters, numbers, hyphens, underscores
- **Avoid**: Special characters that might cause issues in API calls or file operations

#### Troubleshooting Alias Issues

##### ThunderSsl Common Issues
- **Template Update Failures**: Verify templates exist and are accessible
- **Long Alias Names**: Orchestrator will truncate to fit timestamp if needed
- **Special Characters**: May cause API call failures


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

## Certificate Store Configuration

### ⚙️ Configuration Fields

| Name              | Display Name                  | Description                                                  | Type   | Required |
|-------------------|-------------------------------|--------------------------------------------------------------|--------|----------|
| allowInvalidCert  | Allow Invalid Cert on A10 API | If true, allows self-signed/untrusted certs for A10 API access | Bool   | ✅ (default: true) |

