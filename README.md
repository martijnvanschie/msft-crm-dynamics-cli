# Microsoft Dynamics CLI

A small CLI application that allows users to interact with Microsoft Dynamics 365 from the command line.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Authentication](#authentication)
- [Commands](#commands)
  - [Account Commands](#account-commands)
  - [Opportunity Commands](#opportunity-commands)
- [Output Formats](#output-formats)
- [Examples](#examples)
- [Troubleshooting](#troubleshooting)

## Prerequisites

- .NET 9.0 Runtime
- Microsoft Dynamics 365 instance
- Azure AD App Registration (Public Client) with Dynamics CRM permissions
- Microsoft account with access to your Dynamics 365 organization

## Installation

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd msft-crm-dynamics-cli
   ```

2. Build the solution:
   ```bash
   cd cli
   dotnet build
   ```

3. Publish the CLI (optional):
   ```bash
   .\publish-cli.ps1
   ```

## Configuration

### Application Settings

Edit `appsettings.json` to configure the Dynamics instance URL:

```json
{
  "Settings": {
    "DynamicsUrl": "https://your-instance.crm.dynamics.com"
  }
}
```

### Logging

Logs are written to:
- Console (warnings and errors only)
- File: `c:\TEMP\.logs\microsoft-dynamics-cli-log-<month>.txt`

Log levels can be configured in `appsettings.json` under the `Serilog` section.

## Authentication

The CLI uses interactive Azure AD user authentication. When you run a command for the first time, a browser window will open asking you to sign in with your Microsoft credentials. The CLI will then use your user identity to query Dynamics 365.

### Required Environment Variables

You must set the following environment variables before using the CLI:

```bash
# PowerShell
$env:PARTNER_CENTER_TENANT_ID="your-tenant-id"
$env:PARTNER_CENTER_CLIENT_ID="your-app-registration-client-id"

# Command Prompt
set PARTNER_CENTER_TENANT_ID=your-tenant-id
set PARTNER_CENTER_CLIENT_ID=your-app-registration-client-id
```

For persistent configuration, add these to your system environment variables.

### Token Caching

After your first successful login, the authentication token is cached locally at:
```
%LocalAppData%\PartnerCenterCli\partner_center_cli.cache
```

This means you won't need to sign in again until the token expires. The CLI will:
1. First attempt to use a cached token (silent authentication)
2. If the cache is invalid or expired, prompt you to sign in interactively

### Azure AD App Registration

The Client ID must correspond to an Azure AD App Registration that has:
- **Redirect URI**: `http://localhost` (Public client/native)
- **API Permissions**: Dynamics CRM `user_impersonation` scope
- **Authentication**: Allow public client flows enabled

### User Permissions

Since the CLI runs queries under your user identity, you'll only be able to access data that your Dynamics 365 user account has permissions to view.

## Commands

### Global Options

- `--debug` - Display diagnostic information about the executable path and environment

### Account Commands

#### `account search`

Search for accounts by name in your Dynamics 365 instance.

**Usage:**
```bash
mdcli account search --name <NAME> [OPTIONS]
```

**Options:**
- `-n|--name <NAME>` **(required)** - The name or partial name of the account to search for
- `-t|--top <COUNT>` - Maximum number of results to return (default: 10)
- `-c|--contains` - Use 'contains' search instead of 'starts with' (default is starts with)

**Examples:**
```bash
# Search for accounts starting with "Contoso"
mdcli account search --name Contoso

# Search for accounts containing "Corp" (anywhere in the name)
mdcli account search -n Corp --contains

# Limit results to top 5 accounts
mdcli account search -n Microsoft --top 5
```

**Output:**
Displays a formatted table with:
- Account Name
- Owner
- Territory
- Relationship Type
- Account ID

---

### Opportunity Commands

#### `opportunity search`

Search for opportunities by name.

**Usage:**
```bash
mdcli opportunity search --name <NAME> [OPTIONS]
```

**Options:**
- `-n|--name <NAME>` **(required)** - The name or partial name of the opportunity to search for
- `-t|--top <COUNT>` - Maximum number of results to return (default: 20)
- `-c|--contains` - Use 'contains' search instead of 'starts with'
- `--include-closed` - Include closed opportunities in the results
- `-j|--json` - Output raw JSON response

**Examples:**
```bash
# Search for open opportunities starting with "Contoso"
mdcli opportunity search --name Contoso

# Search for all opportunities (including closed) containing "Cloud"
mdcli opportunity search -n Cloud --contains --include-closed

# Get top 5 results
mdcli opportunity search -n Microsoft --top 5

# Output as JSON
mdcli opportunity search -n Contoso --json
```

**Output:**
- By default: Formatted table with opportunity details
- With `--json`: Raw JSON response

---

#### `opportunity by-account`

Get all opportunities for a specific account using the account's GUID.

**Usage:**
```bash
mdcli opportunity by-account <ACCOUNT_ID> [OPTIONS]
```

**Arguments:**
- `<ACCOUNT_ID>` **(required)** - The GUID of the account

**Options:**
- `-c|--include-closed` - Include closed opportunities in the results
- `-j|--json` - Output raw JSON response

**Examples:**
```bash
# Get open opportunities for an account
mdcli opportunity by-account 12345678-1234-1234-1234-123456789abc

# Get all opportunities (including closed)
mdcli opportunity by-account 12345678-1234-1234-1234-123456789abc --include-closed

# Output as JSON
mdcli opportunity by-account 12345678-1234-1234-1234-123456789abc --json
```

---

#### `opportunity by-account-name`

Get opportunities for an account by searching by the account name. If multiple accounts match, you'll be prompted to select one.

**Usage:**
```bash
mdcli opportunity by-account-name <ACCOUNT_NAME> [OPTIONS]
```

**Arguments:**
- `<ACCOUNT_NAME>` **(required)** - The name or partial name of the account

**Options:**
- `--contains` - Use 'contains' search instead of 'starts with'
- `-c|--include-closed` - Include closed opportunities in the results
- `-j|--json` - Output raw JSON response

**Examples:**
```bash
# Get opportunities for an account starting with "Contoso"
mdcli opportunity by-account-name Contoso

# Search for account containing "Corp" and get all opportunities
mdcli opportunity by-account-name Corp --contains --include-closed

# Get opportunities for "Microsoft" account
mdcli opportunity by-account-name Microsoft
```

**Behavior:**
- If exactly one account matches: Automatically retrieves opportunities for that account
- If multiple accounts match: Displays a selection prompt to choose the desired account
- If no accounts match: Displays an error message

---

## Output Formats

### Table Format (Default)

The CLI uses color-coded, formatted tables for easy reading:
- **Green** - Success messages and headers
- **Yellow** - Column headers and warnings
- **Red** - Error messages
- **Dim** - N/A values and metadata

### JSON Format

Use the `--json` or `-j` flag to output raw JSON data for programmatic processing.

## Examples

### Complete Workflow Examples

**Find an account and its opportunities:**
```bash
# Step 1: Search for the account
mdcli account search --name "Contoso"

# Step 2: Copy the Account ID from the results
# Step 3: Get opportunities for that account
mdcli opportunity by-account 12345678-1234-1234-1234-123456789abc
```

**Quick search by account name:**
```bash
# One command to find account and get its opportunities
mdcli opportunity by-account-name "Contoso Corporation"
```

**Export data for analysis:**
```bash
# Export opportunities as JSON for processing
mdcli opportunity search --name "Q1" --json > opportunities.json
```

## Troubleshooting

### Authentication Errors

**Error:** `PARTNER_CENTER_TENANT_ID environment variable not set`

**Solution:** Ensure both required environment variables are set:
```bash
$env:PARTNER_CENTER_TENANT_ID="your-tenant-id"
$env:PARTNER_CENTER_CLIENT_ID="your-client-id"
```

**Error:** Browser doesn't open for login

**Solution:** 
1. Ensure you're running the CLI from an environment that can open a browser
2. Manually navigate to the authentication URL if provided
3. Check that the redirect URI (`http://localhost`) is configured in your Azure AD App Registration

**Error:** Login succeeds but queries fail

**Solution:**
1. Verify your user account has appropriate permissions in Dynamics 365
2. Check that the Azure AD App has the `user_impersonation` permission for Dynamics CRM
3. Ensure admin consent has been granted for the API permissions

### HTTP Errors

**Error:** `HTTP Error: Unauthorized`

**Solution:** Verify that:
1. You've successfully signed in (check the token cache exists)
2. Your user account has the correct permissions in Dynamics 365
3. The Azure AD App has `user_impersonation` permission granted
4. The Dynamics URL in `appsettings.json` is correct
5. Your token hasn't expired (try signing in again)

### No Results Found

If searches return no results:
1. Try using the `--contains` flag for broader searches
2. Verify you have access to the data in Dynamics 365
3. Check that the Dynamics instance URL is correct
4. Review the log files for detailed error messages

### Debug Mode

Run with `--debug` to see diagnostic information:
```bash
mdcli --debug
```

This displays:
- Process path
- Executable directory
- Base directory
- Environment name

## Additional Resources

- [Spectre.Console Documentation](https://spectreconsole.net/)
- [Microsoft Dynamics 365 Web API Reference](https://docs.microsoft.com/en-us/dynamics365/customer-engagement/web-api/)
- [Azure AD App Registration Guide](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app)
- [MSAL.NET Authentication](https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-overview)

