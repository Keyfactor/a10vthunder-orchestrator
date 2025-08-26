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

### TEST CASES

Case Number|Case Name|Case Description|Store Path|Overwrite Flag|Alias Name|Expected Results|Passed
-----------|---------|----------------|----------|--------------|----------|----------------|--------------
1|Fresh Add SSL Certificate With Private Key|Will create new SSL certificate and private key on the vThunder appliance in shared partition|shared|true|WebServerSSL|The new WebServerSSL certificate and private key will be created in SSL certificate store on vThunder|True
1a|Replace SSL Cert with no overwrite flag|Should warn user that a cert cannot be replaced with the same name without overwrite flag|shared|false|WebServerSSL|Error message indicating overwrite flag must be used|True
1b|Replace SSL Cert with overwrite flag (unbound)|Will replace certificate and private key on vThunder for unbound certificate|shared|true|WebServerSSL|Certificate will be removed and re-added because it's not bound to templates|True
2|Add SSL Cert Without Private Key|This will create a certificate with no private key on vThunder|shared|false|PublicCertOnly|Only certificate will be added to vThunder SSL store with no private key|True
2a|Replace SSL Cert Without Private Key|This will replace a certificate with no private key on vThunder|shared|true|PublicCertOnly|Only certificate will be replaced on vThunder with no private key|True
2b|Replace SSL Cert Without Private Key no overwrite flag|Should warn user that a cert cannot be replaced with the same name without overwrite flag|shared|false|PublicCertOnly|Error message indicating overwrite flag must be used|True
3|Remove Unbound SSL Certificate and Private Key|Certificate and Private Key will be removed from A10|shared|N/A|WebServerSSL|Certificate and key will be removed from vThunder SSL store|True
3a|Remove SSL Certificate without Private Key|Certificate will be removed from A10|shared|N/A|PublicCertOnly|Certificate will be removed from vThunder SSL store|True

### Template-Bound Certificate Operations

Case Number|Case Name|Case Description|Store Path|Overwrite Flag|Alias Name|Expected Results|Passed
-----------|---------|----------------|----------|--------------|----------|----------------|--------------
4|Replace Server-SSL Template-Bound Certificate|Will create new timestamped certificate and update server-ssl templates|shared|true|APIGatewayCert|New certificate created with timestamp alias (APIGatewayCert_1672531200), server-ssl templates updated, virtual services rebound, old cert removed|True
4a|Replace Client-SSL Template-Bound Certificate|Will create new timestamped certificate and update client-ssl templates|shared|true|ClientAuthCert|New certificate created with timestamp alias (ClientAuthCert_1672531200), client-ssl templates updated, virtual services rebound, old cert removed|True
4b|Replace Multi-Template-Bound Certificate|Will create new timestamped certificate and update both server-ssl and client-ssl templates|shared|true|DualPurposeCert|New certificate created with timestamp, both template types updated with consistent alias, all virtual services rebound|True
4c|Attempt to Remove Template-Bound Certificate|Should fail with informative error about certificate being in use|shared|N/A|BoundServerCert|Error indicating certificate is bound to SSL templates and cannot be removed|True

### Partition Operations

Case Number|Case Name|Case Description|Store Path|Overwrite Flag|Alias Name|Expected Results|Passed
-----------|---------|----------------|----------|--------------|----------|----------------|--------------
5|Add SSL Certificate to Custom Partition|Certificate will be added to specified partition instead of shared|tenant-prod|false|TenantWebCert|Certificate added to "tenant-prod" partition, isolated from shared partition|True
5a|Remove SSL Certificate from Custom Partition|Certificate will be removed from specified partition|tenant-prod|N/A|TenantWebCert|Certificate removed from "tenant-prod" partition, shared partition unaffected|True
5b|Replace Certificate in Custom Partition with Template Binding|Certificate replacement with template updates in specific partition|tenant-prod|true|TenantAPICert|New timestamped certificate created in partition, partition-specific templates updated|True

### Inventory Operations

Case Number|Case Name|Case Description|Store Path|Overwrite Flag|Alias Name|Expected Results|Passed
-----------|---------|----------------|----------|--------------|----------|----------------|--------------
6|Inventory SSL Certificates from Shared Partition|Inventory of SSL certificates will be pulled from shared partition|shared|N/A|N/A|All SSL certificates in shared partition inventoried with private key flags and metadata|True
6a|Inventory SSL Certificates from Custom Partition|Inventory of SSL certificates will be pulled from specified partition|tenant-prod|N/A|N/A|All SSL certificates in "tenant-prod" partition inventoried, isolated from other partitions|True
6b|Inventory Mixed Certificate Types|Inventory should handle certificates with and without private keys|shared|N/A|N/A|Certificates with private keys marked as PrivateKeyEntry=true, certificates without marked as false|True

### API Version Compatibility

Case Number|Case Name|Case Description|Store Path|Overwrite Flag|Alias Name|Expected Results|Passed
-----------|---------|----------------|----------|--------------|----------|----------------|--------------
7|API v4 Detection and Template Operations|System should detect A10 software version 4.x and use appropriate API format for template updates|shared|true|V4TestCert|API v4 format detected and used for template updates, version info logged showing 4.x software|True
7a|API v6 Detection and Template Operations|System should detect A10 software version 6.x and use appropriate API format for template updates|shared|true|V6TestCert|API v6 format detected and used for template updates (default), version info logged|True

## ThunderMgmt Store Type Test Cases

### SCP Certificate Operations

Case Number|Case Name|Case Description|Store Path|Overwrite Flag|Alias Name|Expected Results|Passed
-----------|---------|----------------|----------|--------------|----------|----------------|--------------
8|Fresh Add Management Certificate via SCP|Will upload certificate files to SCP server and install on A10 management interface|/home/certuser|true|MgmtInterface2025|Files MgmtInterface2025.crt and MgmtInterface2025.key created on SCP server, A10 loads certificate via API|True
8a|Replace Management Certificate with overwrite flag|Will replace existing certificate files and reload on A10 management interface|/home/certuser|true|MgmtInterface2025|Existing files overwritten, A10 reloads certificate, 60-second delay observed, memory written|True
8b|Replace Management Certificate without overwrite flag|Should warn user that files cannot be replaced without overwrite flag|/home/certuser|false|MgmtInterface2025|Error indicating files exist and overwrite flag must be used|True
9|Add Management Cert Without Private Key|This will create certificate file only on SCP server|/home/certuser|false|MgmtCertOnly|Only .crt file will be created on SCP server, no .key file, A10 API called for certificate only|True
10|Remove Management Certificate Files|Certificate files will be removed from SCP server|/home/certuser|N/A|MgmtInterface2025|Both .crt and .key files deleted from SCP server, A10 management configuration unchanged|True

### SCP Server Connectivity and Error Handling

Case Number|Case Name|Case Description|Store Path|Overwrite Flag|Alias Name|Expected Results|Passed
-----------|---------|----------------|----------|--------------|----------|----------------|--------------
11|Inventory Management Certificates from SCP|Inventory of certificate files will be retrieved from SCP server directory|/home/certuser|N/A|N/A|All valid PEM certificates in SCP directory inventoried, invalid files skipped gracefully|True
11a|SCP Authentication Failure|Should handle SCP authentication errors gracefully|/home/certuser|N/A|TestCert|Clear authentication error message, operation fails safely, security not compromised|True
11b|SCP Network Connectivity Issues|Should handle network connectivity issues to SCP server|/home/unreachable|N/A|TestCert|Network timeout error captured, distinguishes from authentication errors, provides troubleshooting guidance|True
11c|Remote File Already Exists Check|Should properly detect existing files on SCP server before upload|/home/certuser|false|ExistingCert|File existence check works correctly, appropriate error when overwrite=false and file exists|True

## Cross-Store Integration Test Cases

Case Number|Case Name|Case Description|Store Types|Expected Results|Passed
-----------|---------|----------------|-----------|----------------|--------------
12|Concurrent SSL and Management Operations|Both store types should operate independently without interference|ThunderSsl + ThunderMgmt|SSL operations use AXAPI endpoints, Management operations use SCP+API, no conflicts or interference|True
12a|PAM Integration Across Store Types|PAM credential resolution should work for both store types|ThunderSsl + ThunderMgmt|All PAM-eligible fields resolved securely across both store types, different credential contexts maintained|True
12b|Mixed Partition and SCP Operations|Operations in SSL partitions should not affect SCP-based management operations|ThunderSsl (custom partition) + ThunderMgmt|SSL partition operations isolated from SCP operations, both succeed independently|True

## Error Recovery and Edge Case Test Cases

Case Number|Case Name|Case Description|Store Path|Overwrite Flag|Alias Name|Expected Results|Passed
-----------|---------|----------------|----------|--------------|----------|----------------|--------------
13|Template Update Failure Recovery|Should handle failures during template update phase and rollback virtual service bindings|shared|true|RecoveryTest|Original template bindings restored to virtual services, new certificate cleaned up if possible, detailed error logging|True
13a|Virtual Service Rebind Partial Failure|Should handle partial failures when rebinding templates to virtual services|shared|true|RebindTest|Failed bindings logged clearly, successful bindings remain, system state documented for manual recovery|True
13b|Long Alias Name Handling|Should handle alias names approaching character limits and truncate appropriately for timestamps|shared|true|VeryLongAliasNameThatExceedsNormalLimitsAndNeedsToBeHandledGracefullyByTheSystem|Alias truncated to fit timestamp within 240 character limit, operation succeeds|True
13c|A10 API Connection Loss During Operation|Should handle API connection failures during multi-step operations|shared|true|ConnectionTest|Operation fails gracefully, system state logged, rollback attempted where possible|True
