# Planpipe

A production-ready C# solution for processing PDF construction plans, extracting features, deriving quantities, and comparing results with ground truth data.

## Architecture

This solution follows clean architecture principles with clear separation of concerns:

- **Planpipe.Core** - Domain models, enums, and interfaces
- **Planpipe.Infrastructure** - EF Core DbContext, PostgreSQL integration, repositories, and PDF text extraction
- **Planpipe.Application** - Business logic services for feature extraction, quantity derivation, confidence scoring, and ground truth comparison
- **Planpipe.Api** - ASP.NET Core Web API with RESTful endpoints
- **Planpipe.UnitTests** - xUnit unit tests with Moq
- **Planpipe.IntegrationTests** - xUnit integration tests with Testcontainers

## Technologies

- **.NET 8 LTS**
- **Entity Framework Core 8.0** with PostgreSQL (Npgsql)
- **PdfPig 0.1.8** for PDF text extraction
- **xUnit, Moq** for testing
- **Testcontainers.PostgreSql** for integration testing
- **Docker & Docker Compose** for containerization
- **GitHub Actions** for CI/CD

## Getting Started

### Prerequisites

- .NET 8 SDK
- Docker and Docker Compose
- PostgreSQL (or use Docker Compose)

### Running with Docker Compose

```bash
docker-compose up -d
```

This will start:
- PostgreSQL database on port 5432
- API service on port 8080

### Running Locally

1. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

2. **Update database connection string in `src/Planpipe.Api/appsettings.json`**

3. **Run migrations:**
   ```bash
   cd src/Planpipe.Infrastructure
   dotnet ef database update --startup-project ../Planpipe.Api/Planpipe.Api.csproj
   ```

4. **Run the API:**
   ```bash
   dotnet run --project src/Planpipe.Api/Planpipe.Api.csproj
   ```

### Running Tests

**Unit tests:**
```bash
dotnet test tests/Planpipe.UnitTests/Planpipe.UnitTests.csproj
```

**Integration tests** (requires Docker):
```bash
dotnet test tests/Planpipe.IntegrationTests/Planpipe.IntegrationTests.csproj
```

**All tests:**
```bash
dotnet test
```

## API Endpoints

### POST /api/runs
Upload and process a PDF file.

**Request:** multipart/form-data with PDF file
**Response:** ProcessingRun object with status and ID

### GET /api/runs
Retrieve all processing runs.

**Response:** List of ProcessingRun objects

### GET /api/runs/{id}
Get details of a specific processing run including features, quantities, and comparison results.

**Response:** ProcessingRun object with related data

### GET /api/runs/{id}/quantities
Get extracted quantities for a specific run.

**Response:** List of QuantityItem objects

### GET /api/runs/{id}/comparison
Get comparison results against ground truth for a specific run.

**Response:** List of ComparisonResult objects

## Pipeline Processing Flow

1. **PDF Upload** → Receives PDF file via API
2. **Text Extraction** → Extracts text from PDF using PdfPig
3. **Feature Extraction** → Uses regex patterns to extract:
   - Quantities (e.g., "150.5 m2", "25 m3")
   - Dimensions (e.g., "2400 x 1200 mm")
   - Item codes (e.g., "ABC1234")
4. **Quantity Derivation** → Groups and sums quantities by unit
5. **Confidence Scoring** → Calculates confidence based on corroborating features
6. **Ground Truth Comparison** → Compares extracted quantities with ground truth data
7. **Results Storage** → Saves all results to PostgreSQL database

## CI/CD

The project includes a GitHub Actions workflow (`.github/workflows/ci.yml`) that:
- Builds the solution
- Runs unit tests
- Runs integration tests with PostgreSQL service container

## Project Structure

```
Planpipe/
├── src/
│   ├── Planpipe.Core/              # Domain layer
│   │   ├── Models/
│   │   ├── Enums/
│   │   └── Interfaces/
│   ├── Planpipe.Infrastructure/    # Data access layer
│   │   ├── Data/
│   │   ├── Repositories/
│   │   ├── Services/
│   │   └── Migrations/
│   ├── Planpipe.Application/       # Business logic layer
│   │   └── Services/
│   └── Planpipe.Api/               # Presentation layer
│       └── Controllers/
├── tests/
│   ├── Planpipe.UnitTests/
│   └── Planpipe.IntegrationTests/
├── Dockerfile
├── docker-compose.yml
└── .github/workflows/ci.yml
```

## License

MIT
