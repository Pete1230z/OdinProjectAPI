# Odin WEG GraphQL Filtering and Reporting Pipeline

## Overview

This project is a C# (.NET) console application that retrieves **Worldwide Equipment Guide (WEG)**
data from the ODIN platform, allows structured filtering using category metadata, and prepares
the data for report generation.

The application is designed as a **data pipeline**, where each stage produces output that feeds
the next stage. Responsibilities are separated across files to keep the codebase understandable
and extensible.

---

## High-Level Capabilities

- Ingests and normalizes WEG category metadata from DotCMS
- Caches category reference data locally to avoid repeated API calls
- Produces backend dropdown data for Domain and Weapon System Type selection
- Converts user selections into deterministic Lucene queries
- Executes filtered GraphQL queries against ODIN
- Returns strongly-typed WEG card results (name, origin, images, etc.)
- Validates results via console output (current phase)

---

## Runtime Flow (End-to-End)

1. Load configuration from `appsettings.json`
2. Fetch and normalize WEG category reference data
3. Cache normalized category tree to disk
4. Load cached categories and build dropdown options
5. Capture filter selections into a criteria object
6. Build a Lucene query from the criteria
7. Execute a filtered GraphQL query using the Lucene string
8. Deserialize and output filtered WEG card results

---

## Key Files and Responsibilities

### Program.cs
**Role:** Application entry point and orchestration

- Loads and validates configuration
- Initializes shared services (`HttpClient`, `GraphQLTransportClient`)
- Executes the application pipeline in order
- Prints console output for validation and debugging

*Program.cs does not contain business logic.*  
It coordinates calls to the other components.

---

### Configuration (`OdinProjectAPI.Configuration`)
**Role:** Configuration binding

- Defines DTOs used to bind `appsettings.json`
- Provides strongly-typed access to ODIN endpoints and output settings
- Consumed during application startup

**Primary file:**
- `AppSettingsModels.cs`

---

## GraphQL Pipeline (ODIN Content API)

### GraphQLTransportClient.cs
**Role:** Low-level GraphQL transport

- Executes GraphQL-over-HTTP requests against ODIN
- Supports parameterized queries with variables
- Returns raw JSON responses
- Centralizes HTTP and serialization concerns

All GraphQL execution flows through this class.

---

### GraphQLRawDTOs.cs
**Role:** GraphQL response modeling (raw transport DTOs)

- Define how ODIN GraphQL responses are deserialized
- Mirror the GraphQL response shape
- Contain no business logic

These DTOs are consumed by query repositories and `Program.cs`.

---

## WEG Image Handling

### WegImageParser.cs
**Role:** Image JSON parsing and URL construction

- Parses the `images` field returned by WEG cards (JSON string)
- Converts relative image paths into absolute URLs
- Used during validation and future report generation

---

## WEG Subnavigation Pipeline (DotCMS)

### WegSubnavFetcher.cs
**Role:** DotCMS WEG category ingestion and caching

- Fetches the WEG subnavigation tree from DotCMS
- Writes raw reference data to `weg-subnav-raw.json`
- Normalizes the category tree
- Writes a cached, normalized version to `weg-categories.json`

This is the source of all dropdown reference data.

---

### WegSubnavRawDTOs.cs
**Role:** Raw DotCMS modeling (subnav response DTOs)

- Mirror the exact structure returned by the DotCMS subnav endpoint
- Used only during inspection and normalization
- Not used by filtering or query logic

These exist solely to deserialize the raw reference data.

---

## Category Processing (Normalized Models)

### WegCategoryNormalizer.cs
**Role:** Raw → normalized transformation

- Converts DotCMS-specific structures into application-friendly models
- Enforces required fields (category variables)
- Produces a clean hierarchical tree

---

### WegCategoryModels.cs
**Role:** Normalized category models

- Represent categories in a predictable, stable format
- Used by dropdown logic, filtering, and later reporting
- Serialized to disk as `weg-categories.json`

These models are safe to use throughout the application.

---

### WegCategoryRepository.cs
**Role:** Cached category access and tree navigation

- Loads the normalized category cache from disk
- Finds nodes by category variable
- Produces dropdown-ready `{ Label, Value }` options

This bridges cached reference data to user selection logic.

---

## Filtering and Querying (WEG Cards)

### WegFilterCriteria.cs
**Role:** Filter selection contract

- Stores selected Domain, Weapon System Type, and future filters
- Serves as the single input to the Lucene query builder

This keeps filter state explicit and centralized.

---

### LuceneQueryBuilder.cs
**Role:** Server-side filter construction

- Converts `WegFilterCriteria` into a Lucene query string
- Enforces deterministic output
- Encapsulates Lucene syntax rules required by ODIN

The generated query feeds directly into GraphQL requests.

---

### WegCardQueryRepository.cs
**Role:** Filtered WEG card retrieval (query boundary)

- Executes GraphQL queries using Lucene filters
- Deserializes results into strongly-typed `WegCardItem` lists
- Returns filtered WEG card data to the caller

This is the boundary between filtering logic and WEG content retrieval.
