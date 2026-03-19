# WarranTea — Warranties Made Easy

A full-stack warranty tracking application with AI-powered features including OCR receipt scanning, semantic product search, and an AI warranty assistant. Built with ASP.NET Core 10 and Nuxt 4, deployed on Azure.

**Live Demo:**

- Frontend: https://agreeable-rock-06b51d41e.1.azurestaticapps.net
- API: https://warrenteaapi.azurewebsites.net

---

## Architecture

```
┌─────────────────────┐       ┌─────────────────────┐
│   Nuxt 4 Frontend   │──────▶│  ASP.NET Core API   │
│  Azure Static Web   │ HTTPS │  Azure App Service   │
│       Apps           │◀──────│                      │
└─────────────────────┘       └──────────┬───────────┘
                                         │
                    ┌────────────────────┼────────────────────┐
                    │                    │                    │
            ┌───────▼──────┐   ┌────────▼───────┐   ┌───────▼──────┐
            │  Azure SQL   │   │  Azure Blob    │   │  Azure AI    │
            │  Database    │   │  Storage       │   │  Services    │
            └──────────────┘   └────────────────┘   └──────────────┘
```

---

## Azure Services

| Service                   | Purpose                                                                                             |
| ------------------------- | --------------------------------------------------------------------------------------------------- |
| **Azure App Service**     | Hosts the ASP.NET Core API                                                                          |
| **Azure Static Web Apps** | Hosts the Nuxt frontend                                                                             |
| **Azure SQL Database**    | Primary relational database (EF Core + Identity)                                                    |
| **Azure Blob Storage**    | Receipt images, warranty documents, and OCR extracted text files                                    |
| **Azure AI Search**       | Hybrid semantic + vector search over product warranties (HNSW, cosine similarity, 1024-dim vectors) |
| **Azure AI Vision**       | Multimodal embeddings for text and image vectors, plus OCR text extraction from receipt images      |
| **Azure OpenAI**          | AI chat assistant for warranty-related Q&A (GPT-based)                                              |

---

## Features

### User Warranty Management

- Create, view, edit, and delete personal warranties
- Upload receipt images with automatic OCR text extraction
- Upload warranty documents
- Filter warranties by search query, brand, and status
- Automatic status computation (active, expiring soon, expired)

### AI-Powered OCR

- Upload a receipt photo and the system extracts warranty fields (product name, brand, purchase date, expiration date, coverage type) using Azure AI Vision OCR and regex parsing

### Semantic Product Search

- Hybrid text + vector search across 71 pre-loaded product warranties
- Image-based search — upload a product photo to find matching warranties
- Brand and category filtering with semantic ranking

### AI Warranty Assistant

- Chat with an AI assistant about warranty questions
- Conversation history support
- Powered by Azure OpenAI

### Admin Product Warranty Management

- Full CRUD for the product warranty catalog
- Re-index all product warranties into Azure AI Search

### Authentication

- Cookie-based authentication via ASP.NET Identity
- Role-based authorization (User, Admin)
- Cross-domain cookie support (SameSite=None, Secure, HttpOnly)

---

## Tech Stack

### Backend

| Technology             | Version |
| ---------------------- | ------- |
| ASP.NET Core           | 10.0    |
| Entity Framework Core  | 10.0.4  |
| ASP.NET Identity       | 10.0.4  |
| Azure.Search.Documents | 11.7.0  |
| Azure.Storage.Blobs    | 12.27.0 |
| C# / .NET              | 10      |

### Frontend

| Technology | Version |
| ---------- | ------- |
| Nuxt       | 4.3.1   |
| Vue        | 3.5.30  |
| Vuetify    | 3.12.2  |
| TypeScript | —       |
| Vitest     | 4.1.0   |

---

## API Endpoints

### Auth — `api/auth`

| Method | Route                | Auth       | Description              |
| ------ | -------------------- | ---------- | ------------------------ |
| POST   | `/api/auth/register` | Public     | Register a new user      |
| POST   | `/api/auth/login`    | Public     | Login (sets auth cookie) |
| POST   | `/api/auth/logout`   | Public     | Logout (clears cookie)   |
| GET    | `/api/auth/me`       | Authorized | Get current user info    |

### Warranties — `api/warranties`

| Method | Route                                 | Auth       | Description                         |
| ------ | ------------------------------------- | ---------- | ----------------------------------- |
| GET    | `/api/warranties`                     | Authorized | List user's warranties (filterable) |
| GET    | `/api/warranties/{id}`                | Authorized | Get warranty by ID                  |
| POST   | `/api/warranties`                     | Authorized | Create warranty                     |
| PUT    | `/api/warranties/{id}`                | Authorized | Update warranty                     |
| DELETE | `/api/warranties/{id}`                | Authorized | Delete warranty                     |
| POST   | `/api/warranties/{id}/upload-receipt` | Authorized | Upload receipt (auto-OCR)           |
| GET    | `/api/warranties/{id}/receipt`        | Authorized | Download receipt file               |
| GET    | `/api/warranties/{id}/extracted-text` | Authorized | Download OCR text                   |
| POST   | `/api/warranties/{id}/upload-doc`     | Authorized | Upload warranty document            |

### Products — `api/products`

| Method | Route                | Auth   | Description                 |
| ------ | -------------------- | ------ | --------------------------- |
| GET    | `/api/products`      | Public | List all products           |
| GET    | `/api/products/{id}` | Public | Get product with warranties |

### Product Search — `api/products/search`

| Method | Route                        | Auth   | Description                 |
| ------ | ---------------------------- | ------ | --------------------------- |
| GET    | `/api/products/search`       | Public | Hybrid text + vector search |
| POST   | `/api/products/search/image` | Public | Search by image upload      |

### OCR — `api/ocr`

| Method | Route                     | Auth       | Description                        |
| ------ | ------------------------- | ---------- | ---------------------------------- |
| POST   | `/api/ocr/parse-warranty` | Authorized | Extract warranty fields from image |

### AI — `api/ai`

| Method | Route                    | Auth       | Description                     |
| ------ | ------------------------ | ---------- | ------------------------------- |
| POST   | `/api/ai/warranty-query` | Authorized | Chat with AI warranty assistant |

### Admin — `api/admin/product-warranties`

| Method | Route                                   | Auth  | Description                 |
| ------ | --------------------------------------- | ----- | --------------------------- |
| GET    | `/api/admin/product-warranties`         | Admin | List all product warranties |
| GET    | `/api/admin/product-warranties/{id}`    | Admin | Get product warranty        |
| POST   | `/api/admin/product-warranties`         | Admin | Create product warranty     |
| PUT    | `/api/admin/product-warranties/{id}`    | Admin | Update product warranty     |
| DELETE | `/api/admin/product-warranties/{id}`    | Admin | Delete product warranty     |
| POST   | `/api/admin/product-warranties/reindex` | Admin | Re-index search index       |

### Health — `api/health`

| Method | Route         | Auth   | Description                                  |
| ------ | ------------- | ------ | -------------------------------------------- |
| GET    | `/api/health` | Public | Database connection status and record counts |

---

## Frontend Pages

| Route                       | Description                                      |
| --------------------------- | ------------------------------------------------ |
| `/`                         | Landing page                                     |
| `/login`                    | Login form                                       |
| `/register`                 | Registration form                                |
| `/dashboard`                | User warranty dashboard with stats and filtering |
| `/warranties/add`           | Add warranty with OCR receipt scanning           |
| `/warranties/search`        | Search personal warranties                       |
| `/warranties/[id]`          | Warranty detail view                             |
| `/products/search`          | Semantic product warranty search (text + image)  |
| `/ai`                       | AI warranty assistant chat                       |
| `/admin/product-warranties` | Admin product warranty management                |

---

## Data Model

### Product

20 seeded products across electronics, appliances, and gaming (e.g., MacBook Pro 16", Galaxy S25 Ultra, Dyson V15 Detect, LG C4 OLED TV, PlayStation 5 Pro).

### Product Warranty

71 seeded warranty options (3–5 per product) covering Manufacturer, Extended, Accidental, Theft & Loss, International, and specialty coverage types.

### User Warranty

User-created warranties linked to products, with receipt/document storage and automatic status tracking.

### Seeded Users

| Email               | Role  | Password |
| ------------------- | ----- | -------- |
| admin@warrantea.com | Admin | Admin1!  |
| alice@warrantea.com | User  | Alice1!  |
| bob@warrantea.com   | User  | Bobob1!  |
| carol@warrantea.com | User  | Carol1!  |

---

## Testing

**93 total unit tests** (55 backend + 38 frontend)

### Backend — xUnit + Moq + EF Core InMemory

| Test File                   | Tests | Coverage                                             |
| --------------------------- | ----- | ---------------------------------------------------- |
| WarrantyServiceTests        | 22    | CRUD, status computation, filtering, blob operations |
| ProductWarrantyServiceTests | 13    | CRUD, reindex, search integration                    |
| AiServiceTests              | 8     | AI calls, error handling, timeouts                   |
| OcrServiceTests             | 7     | OCR parsing, regex extraction                        |
| ProductServiceTests         | 5     | Product retrieval                                    |

### Frontend — Vitest + happy-dom + Vue Test Utils

| Test File               | Tests | Coverage                        |
| ----------------------- | ----- | ------------------------------- |
| types.test.ts           | 8     | TypeScript interface validation |
| warrantyService.test.ts | 6     | API calls, FormData, OCR        |
| adminService.test.ts    | 6     | CRUD, reindex                   |
| productService.test.ts  | 6     | Search, duration formatting     |
| useAuth.test.ts         | 6     | Cookie auth, roles, login       |
| useAiChat.test.ts       | 5     | Chat history, error handling    |
| auth.test.ts            | 1     | Auth headers                    |

---

## CI/CD

Two GitHub Actions workflows deploy on push to `main`:

- **API Workflow** — Builds .NET solution, runs backend tests, publishes `WarranTeaApi.csproj`, deploys to Azure App Service via OIDC
- **Frontend Workflow** — Installs dependencies, runs frontend tests, generates static site with `npm run generate`, deploys to Azure Static Web Apps

---

## Local Development

### Prerequisites

- .NET 10 SDK
- Node.js 18+
- SQL Server LocalDB (or update the connection string)

### Backend

```bash
cd WarranTeaApi
# Copy appsettings.template.json to appsettings.json and fill in your Azure keys
dotnet run
```

The API starts at `http://localhost:5000`.

### Frontend

```bash
cd WarranTeaFrontEnd
npm install
npm run dev
```

The frontend starts at `http://localhost:3000` with API calls proxied to `localhost:5000`.

### Run Tests

```bash
# Backend
dotnet test WarranTeaApi.Tests

# Frontend
cd WarranTeaFrontEnd
npx vitest run
```

---

## Configuration

The API reads configuration from `appsettings.json` locally or Azure App Service environment variables in production:

| Key                                   | Description                                       |
| ------------------------------------- | ------------------------------------------------- |
| `ConnectionStrings:DefaultConnection` | SQL Server connection string                      |
| `AzureAISearch:Endpoint`              | Azure AI Search service endpoint                  |
| `AzureAISearch:AdminApiKey`           | Azure AI Search admin key                         |
| `AzureAISearch:IndexName`             | Search index name (default: `warranties`)         |
| `AzureAIVision:Endpoint`              | Azure AI Vision endpoint                          |
| `AzureAIVision:ApiKey`                | Azure AI Vision API key                           |
| `AzureBlob:ConnectionString`          | Azure Blob Storage connection string              |
| `AzureBlob:ContainerName`             | Blob container name (default: `files`)            |
| `AzureOpenAI:Endpoint`                | Azure OpenAI endpoint (including deployment path) |
| `AzureOpenAI:ApiKey`                  | Azure OpenAI API key                              |

In Azure App Service, use double underscores as section separators (e.g., `AzureAISearch__Endpoint`).

- Upload warranty image
- OCR + AI extraction of structured data:
  - Product name
  - Brand
  - Purchase date
  - Warranty duration
  - Serial number
  - Store

- Convert image to searchable PDF
- Store structured warranty in database
- List/search warranties per user
- Manual product warranty lookup

### Future Enhancements

- QR code receipt scanning
- Email receipt parsing
- Manufacturer API integrations
- Warranty expiration notifications
- Family/shared accounts
- Chrome extension for online purchases
- Mobile app version

---

## 3. Database Design

### User

```
User
-----
Id (GUID)
ExternalAuthId (string)
Email
CreatedAt
```

### Product

```
Product
-------
Id (GUID)
Brand
ModelName
Category
UPC (nullable)
CreatedAt
```

### Warranty

```
Warranty
--------
Id (GUID)
UserId (FK)
ProductId (FK)
PurchaseDate
ExpirationDate
WarrantyDurationMonths
StoreName
SerialNumber
PdfBlobUrl
OriginalImageBlobUrl
AIExtractConfidenceScore
CreatedAt
```

### WarrantyDocument (Optional, Flexible Design)

```
WarrantyDocument
----------------
Id
WarrantyId (FK)
BlobUrl
DocumentType (OriginalImage, PDF, Receipt)
UploadedAt
```

### ProductWarrantyInfo (Lookup Cache)

```
ProductWarrantyInfo
-------------------
Id
ProductId (FK)
WarrantyDurationMonths
CoverageDescription
SourceUrl
LastVerifiedAt
```

---

## 4. AI Integration (Azure Foundry)

### Use Case 1: OCR + Structured Extraction

Pipeline:

1. User uploads image
2. Backend sends image to Azure Document Intelligence / Vision OCR
3. Extract text
4. Send text to LLM via Azure Foundry
5. Prompt for structured JSON output
6. Deserialize into C# model
7. Store in database

Important Design Considerations:

- Require JSON-only output
- Store AI confidence score
- Allow manual user edits
- Display extracted raw text for transparency

### Use Case 2: Product Warranty Lookup

Workflow:

1. User searches product
2. Check local cache (ProductWarrantyInfo)
3. If not found:
   - Call Azure AI agent
   - Retrieve warranty terms
   - Return structured JSON

4. Store in cache table

Must include disclaimer:

> Warranty information is AI-generated and may not be legally accurate.

---

## 5. Authentication Design

Recommended Approach:

- Use Azure Static Web Apps built-in auth
- Entra ID / GitHub / Google provider
- Pass JWT to backend
- Backend validates token

User table stores only:

- ExternalAuthId
- Email

No password storage required.

---

## 6. Blob Storage Organization

Suggested structure:

```
/users/{userId}/warranties/{warrantyId}/original.jpg
/users/{userId}/warranties/{warrantyId}/warranty.pdf
```

Store only Blob URLs in database.

---

## 7. Key Design Decisions

### Multiple Warranties per Product

Allow multiple warranties per product per user.

### AI Failure Handling

- Editable warranty form
- Store AI confidence
- Show extracted text

### Normalization

Normalize products instead of storing product names directly in Warranty.

---

## 8. Scalability Considerations

### Infrastructure Scaling

- Blob Storage for large files
- SQL for structured data
- Stateless API layer
- Horizontal scaling via App Service or Containers

### Background Processing (Future)

- Azure Service Bus
- Azure Queue Storage
- Background worker for:
  - PDF generation
  - AI extraction
  - Notifications

### Caching Strategy

- Cache AI lookup results in ProductWarrantyInfo
- Reduce repeated AI calls
- Add expiration/refresh logic

### Event-Driven Expansion

- Emit events on warranty creation
- Trigger:
  - Email reminders
  - Analytics updates
  - AI summary generation

---

## 9. Risks & Constraints

- Warranty data inconsistency
- AI extraction inaccuracies
- Manufacturer site scraping reliability
- Legal accuracy concerns

Mitigations:

- Manual editing
- Transparency
- Confidence scoring
- Disclaimers

---

## 10. Standout Features

- Warranty expiration dashboard
- Email notifications (SendGrid)
- Category filtering
- AI plain-English warranty summary
- Multi-document uploads
- Receipt + warranty matching

---

## 11. Overall Architectural Strengths

- Clean separation of concerns
- Cloud-native storage model
- AI integration is purposeful
- Scalable file handling
- Secure external authentication
- Resume-worthy full-stack + AI project
