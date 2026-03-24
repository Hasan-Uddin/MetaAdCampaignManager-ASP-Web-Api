<div align="center">

# 📢 Meta Ad Campaign Manager — Web API

A **production-ready ASP.NET Core Web API** that integrates with the **Meta (Facebook) Graph API** to manage ad campaigns, ad sets, ads, lead-gen forms, and incoming leads — all through a clean, well-structured REST API.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-9.0-512BD4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/en-us/ef/core/)
[![Meta Graph API](https://img.shields.io/badge/Meta%20Graph%20API-v25.0-1877F2?logo=meta&logoColor=white)](https://developers.facebook.com/docs/graph-api/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

</div>

---

## 📖 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [Project Structure](#-project-structure)
- [Prerequisites](#-prerequisites)
- [Getting Started](#-getting-started)
- [Configuration](#%EF%B8%8F-configuration)
- [Database Setup & Migrations](#-database-setup--migrations)
- [API Endpoints](#-api-endpoints)
- [Testing the API](#-testing-the-api)
- [Docker](#-docker)
- [License](#-license)

---

## 🌟 Overview

**Meta Ad Campaign Manager** is a backend web API that acts as a bridge between your application and the **Meta (Facebook) Marketing Platform**. It allows you to:

- Authenticate users via **Meta OAuth** (Facebook Login)
- Fetch and manage **Campaigns**, **Ad Sets**, and **Ads** from Meta Ad Accounts
- Create and manage **Lead Generation Forms** on Facebook Pages
- Receive **real-time lead data** via Meta Webhooks
- Save reusable **Form Templates** for quick form creation

---

## ✨ Features

| Feature | Description |
|---|---|
| 🔐 **Authentication** | JWT-based auth + Meta OAuth (Facebook Login) |
| 📊 **Campaign Management** | Sync and view campaigns from Meta Ad Accounts |
| 📝 **Lead Gen Forms** | Create, manage, and template lead-gen forms via Graph API |
| 📩 **Webhook Integration** | Receive real-time lead submissions from Meta |
| 📋 **Form Templates** | CRUD operations for reusable form templates |
| 📧 **Email Notifications** | SMTP-based email service for lead notifications |
| 🏥 **Health Checks** | Built-in health monitoring endpoint |
| 📜 **Structured Logging** | Serilog with console + Seq sink support |
| 🐳 **Docker Support** | Containerized deployment with Docker Compose |

---

## 🛠 Tech Stack

| Technology | Purpose |
|---|---|
| **ASP.NET Core 9** | Web API framework |
| **Entity Framework Core** | ORM / Data access |
| **SQLite** (Dev) / **PostgreSQL** (Prod) | Database |
| **Meta Graph API v25.0** | Facebook/Meta integration |
| **Serilog** | Structured logging |
| **JWT Bearer** | Authentication tokens |
| **Docker & Docker Compose** | Containerization |
| **Seq** | Log aggregation (optional) |

---

## 🏗 Architecture

This project follows **Clean Architecture** principles, with clearly separated layers:

```
┌──────────────────────────────────────────────┐
│                  Web.Api                     │  ← Presentation (Endpoints, Middleware)
├──────────────────────────────────────────────┤
│                Application                   │  ← Use Cases (Commands, Queries, Handlers)
├──────────────────────────────────────────────┤
│                  Domain                      │  ← Entities, Domain Events, Business Rules
├──────────────────────────────────────────────┤
│               Infrastructure                 │  ← EF Core, Meta API Client, Email, Auth
├──────────────────────────────────────────────┤
│               SharedKernel                   │  ← Result type, Entity base, Error handling
└──────────────────────────────────────────────┘
```

> **Dependency Rule**: Inner layers never depend on outer layers. `Domain` has zero external dependencies.

---

## 📁 Project Structure

```
MetaCampaignManager/
├── src/
│   ├── Domain/                  # Core domain entities & business rules
│   │   ├── Ads/                 # Ad entity
│   │   ├── AdSets/              # Ad Set entity
│   │   ├── Campaigns/           # Campaign entity
│   │   ├── Forms/               # Lead Gen Form entity
│   │   ├── FormQuestions/       # Form Question entity
│   │   ├── FormTemplates/       # Form Template entity
│   │   ├── Leads/               # Lead entity + domain events
│   │   ├── MetaSettings/        # Meta API settings per user
│   │   └── Users/               # User entity + factory methods
│   │
│   ├── Application/             # Application layer (CQRS)
│   │   ├── Abstractions/        # Interfaces (Messaging, Auth)
│   │   └── Features/            # Commands & Queries by feature
│   │       ├── Auth/            # Meta OAuth command handlers
│   │       ├── Meta/            # Campaigns, Ads, Forms, Leads, etc.
│   │       └── Users/           # Register, Login, GetMe, etc.
│   │
│   ├── Infrastructure/          # External concerns
│   │   ├── Migrations/          # EF Core migration files
│   │   ├── Persistence/         # DbContext, repositories, configs
│   │   └── Services/            # Meta API client, Email, JWT, etc.
│   │
│   ├── SharedKernel/            # Shared primitives
│   │   ├── Entity.cs            # Base entity with domain events
│   │   ├── Result.cs            # Railway-oriented Result type
│   │   ├── Error.cs             # Typed error definitions
│   │   └── ...
│   │
│   └── Web.Api/                 # ASP.NET Core host
│       ├── Endpoints/           # Minimal API endpoint definitions
│       │   ├── Auth/            # Meta OAuth login & callback
│       │   ├── Meta/            # Campaigns, Ads, Forms, Leads, Webhooks
│       │   ├── MetaSettings/    # Meta settings management
│       │   └── Users/           # Register, Login, Profile
│       ├── Extensions/          # Swagger, Migration, DI extensions
│       ├── Infrastructure/      # Auth middleware, custom results
│       ├── Middleware/           # Request context logging
│       ├── Program.cs           # Application entry point
│       └── appsettings.json     # Configuration
│
├── tests/                       # Test projects
├── docker-compose.yml           # Docker orchestration
└── MetaCampaignManager.sln      # Solution file
```

---

## 📋 Prerequisites

| Tool | Version | Required |
|---|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | 9.0+ | ✅ |
| [Git](https://git-scm.com/) | Latest | ✅ |
| [Docker](https://www.docker.com/) | Latest | Optional |
| [Postman](https://www.postman.com/) | Latest | Optional |
| [Meta Developer Account](https://developers.facebook.com/) | — | ✅ (for Meta API) |

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/<your-username>/MetaAdCampaignManager-ASP-Web-Api.git
cd MetaAdCampaignManager-ASP-Web-Api
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Configure the Application

Copy and update the settings file (see [Configuration](#%EF%B8%8F-configuration) section below):

```bash
# The default appsettings.json works for local development with SQLite
# Update Meta API credentials with your own
```

### 4. Apply Database Migrations

```bash
dotnet ef database update \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Web.Api/Web.Api.csproj
```

### 5. Run the Application

```bash
dotnet run --project src/Web.Api/Web.Api.csproj
```

The API will be available at:
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`
- **Swagger UI**: `https://localhost:5001/swagger` *(Development only)*

---

## ⚙️ Configuration

All settings are in `src/Web.Api/appsettings.json`. Update the following sections with your own values:

### Connection String

```json
"ConnectionStrings": {
  "Database": "Data Source=Data/MetaAdCompaigns.db;Cache=Shared;Mode=ReadWriteCreate"
}
```

> For production, switch to PostgreSQL via Docker Compose.

### Meta Graph API

```json
"MetaApi": {
  "BaseUrl": "https://graph.facebook.com/v25.0/",
  "AppId": "<YOUR_META_APP_ID>",
  "AppSecret": "<YOUR_META_APP_SECRET>",
  "WebhookVerifyToken": "<YOUR_CUSTOM_VERIFY_TOKEN>"
}
```

### JWT Settings

```json
"Jwt": {
  "Secret": "<YOUR_JWT_SECRET>",
  "Issuer": "calendly-clone",
  "Audience": "developers",
  "ExpirationInMinutes": 200
}
```

### Email (SMTP)

```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "Username": "<YOUR_EMAIL>",
  "Password": "<YOUR_APP_PASSWORD>",
  "FromEmail": "<YOUR_FROM_EMAIL>",
  "FromName": "Meta Campaign Manager"
}
```

### CORS (Allowed Origins)

```json
"AllowedOrigins": [
  "https://localhost:4200",
  "http://localhost:4200",
  "https://localhost:3000"
]
```

> **Tip**: For sensitive values, use [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) in development or environment variables in production.

---

## 🗃 Database Setup & Migrations

This project uses **Entity Framework Core** with a **Code-First** approach.

### Apply Migrations (Update Database)

```bash
dotnet ef database update \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Web.Api/Web.Api.csproj
```

### Create a New Migration

```bash
dotnet ef migrations add <MigrationName> \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Web.Api/Web.Api.csproj
```

### Rollback to a Previous Migration

```bash
dotnet ef database update <PreviousMigrationName> \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Web.Api/Web.Api.csproj
```

### Remove the Most Recent Migration

```bash
dotnet ef migrations remove \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Web.Api/Web.Api.csproj
```

> **Note**: In **Development** mode, migrations are applied automatically on startup via `app.ApplyMigrations()`.

---

## 📡 API Endpoints

### 🔐 Authentication

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| `POST` | `/users/register` | Register a new user | ❌ |
| `POST` | `/users/login` | Login and get JWT token | ❌ |
| `GET` | `/meta/auth/login` | Redirect to Meta OAuth login | ❌ |
| `GET` | `/meta/auth/callback` | Meta OAuth callback (sets JWT cookie) | ❌ |

### 👤 Users

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| `GET` | `/api/users/me` | Get current authenticated user | ✅ |
| `GET` | `/users/{userId}` | Get user by ID | ✅ |
| `GET` | `/api/users/{email}` | Get user by email | ✅ |

### 📊 Campaigns, Ad Sets & Ads

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| `GET` | `/meta/campaigns?adAccountId={id}` | Get campaigns for an ad account | ❌ |
| `GET` | `/meta/campaigns/{campaignId}/adsets` | Get ad sets for a campaign | ❌ |
| `GET` | `/meta/adsets/{adSetId}/ads` | Get ads for an ad set | ❌ |

### 📝 Lead Generation Forms

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| `GET` | `/meta/pages/{pageId}/forms` | Get all forms for a page | ❌ |
| `GET` | `/meta/forms/{formId}` | Get a specific form by ID | ❌ |
| `POST` | `/meta/pages/{pageId}/forms` | Create a new lead gen form | ❌ |
| `POST` | `/meta/pages/{pageId}/forms/form-template` | Create form from template | ❌ |
| `DELETE` | `/meta/forms/{formId}` | Delete a form | ❌ |

### 📋 Form Templates

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| `GET` | `/meta/form-templates` | Get all form templates | ❌ |
| `GET` | `/meta/form-templates/{templateId}` | Get template by ID | ❌ |
| `POST` | `/meta/form-templates` | Create a new form template | ❌ |
| `PUT` | `/meta/form-templates/{templateId}` | Update a form template | ❌ |
| `DELETE` | `/meta/form-templates/{templateId}` | Delete a form template | ❌ |

### 📩 Leads & Webhooks

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| `GET` | `/meta/forms/{formId}/leads` | Get leads for a form | ❌ |
| `GET` | `/meta/webhook` | Meta webhook verification (GET) | ❌ |
| `POST` | `/meta/webhook` | Receive webhook events from Meta | ❌ |

### 🏥 Health Check

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/health` | Health check status |

---

## 🧪 Testing the API

### Using Swagger UI

Navigate to `https://localhost:5001/swagger` in your browser when running in **Development** mode.

### Using curl

#### Get Current User (Authenticated)

```bash
curl -X GET https://localhost:5001/api/users/me \
  -H "Authorization: Bearer <YOUR_JWT_TOKEN>"
```

**Response** `200 OK`:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  "name": "John Doe"
}
```

#### Get Campaigns

```bash
curl -X GET "https://localhost:5001/meta/campaigns?adAccountId=act_123456789" \
  -H "Authorization: Bearer <YOUR_JWT_TOKEN>"
```

**Response** `200 OK`:
```json
[
  {
    "id": "120212345678",
    "name": "Summer Sale 2025",
    "status": "ACTIVE",
    "objective": "LEAD_GENERATION",
    "budgetRemaining": 5000,
    "createdAt": "2025-06-01T00:00:00Z"
  }
]
```

#### Create a Form Template

```bash
curl -X POST https://localhost:5001/meta/form-templates \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Contact Form",
    "description": "Basic contact information form",
    "questions": [
      { "key": "email", "type": "EMAIL", "label": "Email Address" },
      { "key": "full_name", "type": "FULL_NAME", "label": "Full Name" },
      { "key": "phone_number", "type": "PHONE", "label": "Phone" }
    ],
    "isDefault": true
  }'
```

**Response** `201 Created`:
```json
"a1b2c3d4-e5f6-7890-abcd-ef1234567890"
```

#### Create a Lead Gen Form on a Facebook Page

```bash
curl -X POST https://localhost:5001/meta/pages/123456789/forms \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Summer Campaign Form",
    "locale": "en_US",
    "privacyPolicy": {
      "url": "https://example.com/privacy",
      "linkText": "Privacy Policy"
    },
    "questions": [
      { "key": "email", "type": "EMAIL", "label": "Email" },
      { "key": "full_name", "type": "FULL_NAME", "label": "Name" }
    ],
    "followUpActionUrl": "https://example.com/thank-you"
  }'
```

**Response** `201 Created`:
```json
"987654321"
```

### Using Postman

1. Import the API base URL: `https://localhost:5001`
2. Create a request to `POST /users/register` or `POST /users/login`
3. Copy the JWT token from the login response
4. Set the `Authorization` header to `Bearer <token>` for protected endpoints
5. Explore the Meta endpoints to manage campaigns, forms, and leads

---

## 🐳 Docker

### Run with Docker Compose

```bash
docker-compose up -d
```

This starts three services:

| Service | Port | Description |
|---|---|---|
| `web-api` | `5000`, `5001` | The ASP.NET Core Web API |
| `postgres` | `5432` | PostgreSQL 17 database |
| `seq` | `8081` | Seq log viewer dashboard |

### Stop Services

```bash
docker-compose down
```

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

<div align="center">

**Built with ❤️ using ASP.NET Core & Meta Graph API**

</div>
