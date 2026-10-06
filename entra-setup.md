# Microsoft Entra ID Authentication & Authorization Setup

This document outlines the architecture, configuration steps, and verification procedures for securing the Web API using Microsoft Entra ID with OAuth 2.0 Authorization Code Flow + PKCE.

---

## 1. Architecture Overview

- **Identity Provider (IdP):** Microsoft Entra ID
- **Protocol:** OAuth 2.0 Authorization Code Flow with PKCE (Proof Key for Code Exchange)
- **API (Resource Server):** Protected ASP.NET Core Web API validating JWT bearer tokens
- **Client Application:** Postman (testing) / React SPA (production public client)
- **Token Format:** Entra ID Access Token (v2.0)

```text
[ Client (Postman / SPA) ]
          │
          │ 1. Redirect to Entra (/authorize) with code_challenge
          ▼
[ Entra ID Authorization Server ]
          │
          │ 2. Authenticate User (Alice / Bob)
          │ 3. Return temporary authorization code
          ▼
[ Client (Postman / SPA) ]
          │
          │ 4. POST code + code_verifier to Entra (/token)
          ▼
[ Entra ID Token Endpoint ]
          │
          │ 5. Validate PKCE match & issue Access Token
          │    (contains aud, iss, scp, roles)
          ▼
[ Client (Postman / SPA) ]
          │
          │ 6. HTTP Request: Authorization: Bearer <access_token>
          ▼
[ .NET Web API ]
          └── Validates Signature, Issuer, Audience, Scopes, and Roles
```

---

## 2. Configuration Inventory & Environment Values

| Configuration Key | Description | Configured Value / Example |
| :--- | :--- | :--- |
| **Tenant ID** | Directory (tenant) ID | `<Tenant-ID>` |
| **API Client ID** | Application (client) ID of API | `<API-Client-ID>` |
| **API App ID URI** | Resource Identifier / Namespace URI | `api://<API-Client-ID>` |
| **API Delegated Scope** | Scope exposed by API | `access_as_user` |
| **Full Scope String** | Qualified Scope requested by clients | `api://<API-Client-ID>/access_as_user` |
| **Client App ID** | Application (client) ID of Postman/SPA | `<Client-App-ID>` |
| **Redirect URI (Postman)** | Authorized OAuth callback | `https://oauth.pstmn.io/v1/callback` |
| **Authorize Endpoint** | Interactive user login URL | `https://login.microsoftonline.com/<Tenant-ID>/oauth2/v2.0/authorize` |
| **Token Endpoint** | Direct token exchange URL | `https://login.microsoftonline.com/<Tenant-ID>/oauth2/v2.0/token` |

---

## 3. Step-by-Step Portal Configuration

### Step 1: Register the API (`entra-auth-lab-api`)

1. In Entra ID, navigate to **App registrations** > **New registration**.
2. Set Name: `entra-auth-lab-api`.
3. Set Account Type: **Accounts in this organizational directory only (Single tenant)**.
4. Leave Redirect URI blank and click **Register**.
5. Go to **Manifest**:
   - Set `"requestedAccessTokenVersion": 2`.
   - Click **Save**.
6. Go to **Expose an API**:
   - Set **Application ID URI** to `api://<API-Client-ID>`.
   - Click **+ Add a scope**:
     - Scope name: `access_as_user`
     - Who can consent: **Admins and users**
     - Display name: `Access entra-auth-lab API`
     - Description: `Allows the application to access the API on behalf of the signed-in user.`
     - State: **Enabled**
7. Go to **App roles**:
   - Create Role 1:
     - Display name: `API Admin`
     - Allowed member types: **Users/Groups**
     - Value: `Admin`
     - Description: `Full administrative access`
   - Create Role 2:
     - Display name: `API Customer`
     - Allowed member types: **Users/Groups**
     - Value: `Customer`
     - Description: `Standard customer read/write access`

---

### Step 2: Create Test Users & Assign Roles

1. Navigate to **Users** > **+ New user** > **Create new user**:
   - User 1: `Alice Admin` (`alice@<tenant-domain>`)
   - User 2: `Bob Customer` (`bob@<tenant-domain>`)
   - Complete initial login once in an incognito window to clear the mandatory first-time password reset.
2. Navigate to **Enterprise applications** > select **`entra-auth-lab-api`** > **Users and groups**:
   - Click **+ Add user/group**:
     - User: `Alice Admin` > Role: `Admin`
   - Click **+ Add user/group**:
     - User: `Bob Customer` > Role: `Customer`

---

### Step 3: Register the Client (`entra-auth-lab-client`)

1. Navigate to **App registrations** > **New registration**.
2. Set Name: `entra-auth-lab-client`.
3. Set Account Type: **Single tenant**.
4. Set Redirect URI:
   - Platform: **Mobile and desktop applications (public client/native)**
     > **Note:** Do NOT pick SPA for Postman; Postman does not emit browser CORS origin headers.
   - Value: `https://oauth.pstmn.io/v1/callback`
5. Click **Register**.
6. Go to **API permissions**:
   - Click **+ Add a permission** > **APIs my organization uses**.
   - Search for and select `entra-auth-lab-api`.
   - Select **Delegated permissions** > check `access_as_user` > click **Add permissions**.
   - Click **Grant admin consent for `<Tenant>`** and confirm with **Yes**.
   - Ensure the Status column shows a green checkmark: **Granted for `<Tenant>`**.

---

## 4. Verification via Postman

### Postman Request Configuration

1. Open Postman, create a new request, and open the **Authorization** tab.
2. Configure the following fields:

| Field | Value |
| :--- | :--- |
| **Type** | `OAuth 2.0` |
| **Add authorization data to** | `Request Headers` |
| **Grant Type** | `Authorization Code (With PKCE)` |
| **Callback URL** | `https://oauth.pstmn.io/v1/callback` |
| **Authorize using browser** | Unchecked (native Postman view) or Checked |
| **Auth URL** | `https://login.microsoftonline.com/<Tenant-ID>/oauth2/v2.0/authorize` |
| **Access Token URL** | `https://login.microsoftonline.com/<Tenant-ID>/oauth2/v2.0/token` |
| **Client ID** | `<Client-App-ID>` |
| **Client Secret** | *(Leave empty)* |
| **Code Challenge Method** | `S256` |
| **Code Verifier** | *(Leave blank - auto-generated)* |
| **Scope** | `api://<API-Client-ID>/access_as_user openid profile` |
| **Client Authentication** | `Send client credentials in body` |

3. Click **Get New Access Token**.
4. Sign in as `alice@<tenant-domain>` in the pop-up window.
5. Click **Proceed** / **Use Token**.

---

## 5. Token Claims Reference (`jwt.ms`)

Decoded payload verification checklist:

```json
{
  "aud": "<API-Client-ID>",
  "iss": "https://login.microsoftonline.com/<Tenant-ID>/v2.0",
  "azp": "<Client-App-ID>",
  "name": "Alice Admin",
  "oid": "<User-Object-ID>",
  "preferred_username": "alice@<tenant-domain>",
  "roles": [
    "Admin"
  ],
  "scp": "access_as_user",
  "ver": "2.0"
}
```

### Claims Responsibility Matrix

| Claim | Key | API Validation Purpose |
| :--- | :--- | :--- |
| **Audience** | `aud` | Confirms the token was minted specifically for this API (ClientId). |
| **Issuer** | `iss` | Confirms the token was signed and issued by our specific Entra tenant. |
| **Authorized Party** | `azp` | Identifies the client app (Postman or SPA) that initiated the flow. |
| **User ID** | `oid` | Immutable user GUID used for DB queries, auditing, and foreign keys. |
| **Scope** | `scp` | Proves the client app was delegated `access_as_user` permissions. |
| **App Roles** | `roles` | Evaluated by ASP.NET Core `[Authorize(Roles = "Admin")]` policies. |