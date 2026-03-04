# Warranty Tracker App – Architecture & Design Overview

## 1. High-Level Architecture

### Frontend

* **Nuxt 4 (Vue)**
* Hosted on **Azure Static Web Apps**
* Responsibilities:

  * Authentication handling
  * Uploading warranty images
  * Viewing/searching warranties
  * Product lookup interface

### Backend

* **ASP.NET Core Web API**
* **Entity Framework Core**
* Hosted on:

  * Azure App Service (simple)
  * Azure Container Apps (scalable option)
* Responsibilities:

  * JWT validation
  * File upload handling
  * Calling Azure AI services
  * Business logic
  * Database interaction

### Storage

* **Azure Blob Storage**

  * Original images
  * Generated PDFs
* **Azure SQL Database**

  * Structured metadata
  * Warranty records
  * Product warranty lookup data

---

## 2. Core Feature Breakdown

### MVP Features

* User authentication (external provider)
* Upload warranty image
* OCR + AI extraction of structured data:

  * Product name
  * Brand
  * Purchase date
  * Warranty duration
  * Serial number
  * Store
* Convert image to searchable PDF
* Store structured warranty in database
* List/search warranties per user
* Manual product warranty lookup

### Future Enhancements

* QR code receipt scanning
* Email receipt parsing
* Manufacturer API integrations
* Warranty expiration notifications
* Family/shared accounts
* Chrome extension for online purchases
* Mobile app version

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

* Require JSON-only output
* Store AI confidence score
* Allow manual user edits
* Display extracted raw text for transparency

### Use Case 2: Product Warranty Lookup

Workflow:

1. User searches product
2. Check local cache (ProductWarrantyInfo)
3. If not found:

   * Call Azure AI agent
   * Retrieve warranty terms
   * Return structured JSON
4. Store in cache table

Must include disclaimer:

> Warranty information is AI-generated and may not be legally accurate.

---

## 5. Authentication Design

Recommended Approach:

* Use Azure Static Web Apps built-in auth
* Entra ID / GitHub / Google provider
* Pass JWT to backend
* Backend validates token

User table stores only:

* ExternalAuthId
* Email

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

* Editable warranty form
* Store AI confidence
* Show extracted text

### Normalization

Normalize products instead of storing product names directly in Warranty.

---

## 8. Scalability Considerations

### Infrastructure Scaling

* Blob Storage for large files
* SQL for structured data
* Stateless API layer
* Horizontal scaling via App Service or Containers

### Background Processing (Future)

* Azure Service Bus
* Azure Queue Storage
* Background worker for:

  * PDF generation
  * AI extraction
  * Notifications

### Caching Strategy

* Cache AI lookup results in ProductWarrantyInfo
* Reduce repeated AI calls
* Add expiration/refresh logic

### Event-Driven Expansion

* Emit events on warranty creation
* Trigger:

  * Email reminders
  * Analytics updates
  * AI summary generation

---

## 9. Risks & Constraints

* Warranty data inconsistency
* AI extraction inaccuracies
* Manufacturer site scraping reliability
* Legal accuracy concerns

Mitigations:

* Manual editing
* Transparency
* Confidence scoring
* Disclaimers

---

## 10. Standout Features

* Warranty expiration dashboard
* Email notifications (SendGrid)
* Category filtering
* AI plain-English warranty summary
* Multi-document uploads
* Receipt + warranty matching

---

## 11. Overall Architectural Strengths

* Clean separation of concerns
* Cloud-native storage model
* AI integration is purposeful
* Scalable file handling
* Secure external authentication
* Resume-worthy full-stack + AI project
