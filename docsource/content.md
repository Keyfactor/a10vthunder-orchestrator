## Overview

A Keyfactor Universal Orchestrator extension for managing SSL/TLS certificates on A10 Networks vThunder load balancers through direct API integration and SCP-based management interface certificate deployment.

The A10 vThunder Orchestrator provides automated certificate lifecycle management for A10 Networks vThunder appliances. It implements two distinct certificate store types:

1. **ThunderSsl**: Direct API-based management of SSL certificates for load balancing and application delivery
2. **ThunderMgmt**: SCP-based management of certificates for the A10 management interface (GUI/API access)

### Key Features

- **Direct SSL Certificate Management**: Native A10 API integration for SSL certificate deployment and management
- **Template-Aware Operations**: Intelligent handling of certificates bound to SSL templates and virtual services
- **Multi-API Version Support**: Automatic detection and support for A10 API v4 and v6
- **Partition Support**: Full support for A10 partitions for multi-tenant deployments
- **Certificate Inventory**: Comprehensive discovery and inventory of existing certificates
- **Management Interface Certificates**: SCP-based deployment for A10 management interface certificates
- **PAM Integration**: Support for Privileged Access Management systems
- **Advanced Certificate Replacement**: Zero-downtime certificate replacement with automatic template rebinding

### Architecture

#### ThunderSsl Store Type
Uses A10's native REST API (AXAPI) for direct certificate management:
- Certificates are uploaded directly to the A10 appliance
- Supports both certificate-only and certificate-with-private-key operations
- Automatically detects and handles template bindings and virtual service configurations
- Implements intelligent certificate replacement to avoid service disruption

#### ThunderMgmt Store Type
Uses SCP (Secure Copy Protocol) for management interface certificates:
- Orchestrator uploads certificates to an intermediate Linux SCP server
- A10 appliance retrieves certificates from the SCP server via API calls
- Designed for management interface SSL certificate deployment

#### A10 vThunder Requirements
- A10 vThunder appliance with AXAPI support
- API versions 4.x or 6.x supported (automatically detected)
- Valid user account with certificate management privileges
- For ThunderMgmt: SSH/SCP access enabled

#### Required A10 Permissions
The orchestrator requires an A10 user account with permissions to:
- Access AXAPI (REST API)
- Manage SSL certificates and private keys
- Read/write SSL templates (server-ssl and client-ssl)
- Query and modify virtual services
- Write configuration to memory
- Set active partitions
- For ThunderMgmt: SSH/SCP file operations

## Certificate Alias Management

Understanding how aliases work is crucial for proper certificate management across both store types.

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

#### ThunderMgmt Aliases

In the ThunderMgmt store type, the **alias** determines the filename for certificates stored on the SCP server:

- **Certificate File**: `{alias}.crt` on the SCP server
- **Private Key File**: `{alias}.key` on the SCP server  
- **A10 API Reference**: The A10 management interface loads certificates using SCP URLs pointing to these files

##### Example ThunderMgmt Usage
```
Alias: "mgmt-interface-cert"
→ SCP Server Files: 
  - /home/scpuser/mgmt-interface-cert.crt
  - /home/scpuser/mgmt-interface-cert.key
→ A10 API Call: 
  - Certificate URL: scp://scpuser:pass@192.168.1.100:/home/scpuser/mgmt-interface-cert.crt
  - Key URL: scp://scpuser:pass@192.168.1.100:/home/scpuser/mgmt-interface-cert.key
```

##### ThunderMgmt File Management

The orchestrator handles file operations as follows:

1. **Add Operation**: 
   - Uploads `{alias}.crt` and `{alias}.key` to SCP server
   - Calls A10 API to load certificate from SCP URLs
   - A10 device pulls files directly from SCP server

2. **Remove Operation**:
   - Deletes `{alias}.crt` and `{alias}.key` from SCP server
   - Does not modify A10 management interface configuration

3. **Replace Operation** (with Overwrite=true):
   - Overwrites existing `{alias}.crt` and `{alias}.key` files
   - Calls A10 API to reload certificate from same SCP URLs

#### Alias Best Practices

##### For ThunderSsl
- Use descriptive names that indicate purpose: `web-frontend-ssl`, `api-backend-tls`
- Avoid special characters that might conflict with A10 naming rules
- Consider including environment indicators: `prod-web-cert`, `stage-api-cert`
- Remember that renaming will append timestamps for template-bound certificates

##### For ThunderMgmt  
- Use names that clearly identify the management purpose: `mgmt-interface-2025`
- Ensure filenames are valid for both SCP server filesystem and A10 API calls
- Consider including renewal dates: `mgmt-cert-jan2025`

##### Character Limitations
- **Maximum Length**: 240 characters (enforced by orchestrator)
- **Recommended Characters**: Letters, numbers, hyphens, underscores
- **Avoid**: Special characters that might cause issues in API calls or file operations

#### Troubleshooting Alias Issues

##### ThunderSsl Common Issues
- **Template Update Failures**: Verify templates exist and are accessible
- **Long Alias Names**: Orchestrator will truncate to fit timestamp if needed
- **Special Characters**: May cause API call failures

##### ThunderMgmt Common Issues  
- **File Path Issues**: Ensure SCP user has access to the target directory
- **Invalid Filenames**: Some characters may not be valid for filesystem operations
- **URL Encoding**: Special characters in aliases may require URL encoding in SCP URLs

## API Integration Details

### AXAPI Endpoints Used

- **Authentication**: `/axapi/v3/auth` or `/axapi/v4/auth`
- **SSL Certificates**: `/axapi/v3/slb/ssl-cert` or `/axapi/v4/slb/ssl-cert`
- **Private Keys**: `/axapi/v3/slb/ssl-key` or `/axapi/v4/slb/ssl-key`
- **SSL Templates**: `/axapi/v3/slb/template/server-ssl` and `/axapi/v3/slb/template/client-ssl`
- **Virtual Services**: `/axapi/v3/slb/virtual-server`
- **Partitions**: `/axapi/v3/active-partition`
- **Memory Operations**: `/axapi/v3/write/memory`

### Certificate Format Support

- **PKCS#12**: Full support for certificates with private keys
- **PEM**: Individual certificate files
- **Certificate Chains**: Automatic handling of certificate chains
- **Private Key Extraction**: Secure extraction and separate storage

### Advanced Features

#### Partition Support

The orchestrator fully supports A10 partitions:
- Set active partition before operations
- Isolate certificate operations to specific partitions
- Support multi-tenant deployments

#### Template Management

Intelligent SSL template handling:
- Detection of server-ssl and client-ssl template usage
- Atomic template updates during certificate replacement
- Preservation of template configurations

#### Virtual Service Coordination

Advanced virtual service management:
- Mapping of templates to virtual service ports
- Coordinated unbinding and rebinding operations
- Support for multiple template types on single ports

#### Alias Management

Sophisticated alias handling:
- Automatic timestamp generation for replacements
- 240-character limit compliance
- Duplicate alias detection and resolution


