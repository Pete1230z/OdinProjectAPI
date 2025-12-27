# ODIN API Definitions and Architecture Overview

This document exists to explain how the ODIN APIs work, how they differ from one another,
and how they are intended to be consumed by applications.  
While ODIN leverages standard web technologies, the APIs themselves are **domain-specific**
and expose ODIN-defined schemas and behaviors.

---

## ODIN Force Structure API  
**(Non-Standard / ODIN-Specific)**

The ODIN Force Structure API is a custom REST API for managing and retrieving military
force structure data.

### Purpose
- Create, modify, and delete force structures (authenticated)
- Allow public, read-only GET access for approved force structure data
- Model hierarchical military organizations and unit compositions

### Characteristics
- Hierarchical, organization-centric schema
- Uses foreign keys to link related entities
- Not an industry-standard API

### Key Concepts
- **Organization**  
  Top-level entity representing exercise countries or insurgent groups

- **top_unit**  
  Foreign key referencing the root unit of a force structure

- **Unit Hierarchy**  
  Recursive tree of units beneath a top unit

- **Template**  
  Foreign key defining standardized unit composition


### Base URL:
https://odin.tradoc.army.mil/FSAPI/help/

#### Example Endpoints:
- Organizations:
  https://odin.tradoc.army.mil/FSAPI/organization

- Full force structure hierarchy:
  https://odin.tradoc.army.mil/FSAPI/unit_info/{unitId}/hierarchy/full

- Template details:
  https://odin.tradoc.army.mil/FSAPI/template/{templateId}

- Template hierarchy:
  https://odin.tradoc.army.mil/FSAPI/template/{templateId}/hierarchy

- Search units:
  https://odin.tradoc.army.mil/FSAPI/search/units?name=BDE

---

## DIS ENUMERATION API (SEMI-STANDARD / DOMAIN-SPECIFIC)
An ODIN-hosted API that exposes DIS (Distributed Interactive Simulation) enumeration data.

- Stores SISO DIS enumeration codes
- Used for simulation and modeling systems
- Domain-specific, not general-purpose
- Based on DIS standards but exposed via a custom ODIN API

Use Cases:
- Assigning enumeration codes to force structure assets
- Assigning enumeration properties to WEG assets

### Base URL:
https://odin.tradoc.army.mil/DISAPI/help/

### Key Endpoints:
- Enumerations:
  https://odin.tradoc.army.mil/DISAPI/enumeration

- Countries:
  https://odin.tradoc.army.mil/DISAPI/enumeration/countries

- Kinds:
  https://odin.tradoc.army.mil/DISAPI/enumeration/kinds

- Domains:
  https://odin.tradoc.army.mil/DISAPI/enumeration/domains

### Notes:
- Entries represent Kinds, Domains, or Categories
- Relationships expressed via foreign keys in the enumeration table

---

## ODIN Content API  
**(Built on Standard Technologies)**

The ODIN Content API provides read-only access to ODIN-managed content
and is backed by the **dotCMS** content platform.

### Purpose
- Retrieve content such as WEG, DATE, TP, and VOA data
- Expose content using standard web technologies with ODIN-specific schemas

### Characteristics
- Read-only access does not require authentication
- ODIN defines its own Content Types and fields
- GraphQL introspection is disabled

### Underlying Technologies (Standard)
- REST
- GraphQL
- Lucene query syntax
- dotCMS content platform

### As of Version
ODIN 3.0.0+

### Retrieval Methods
- GraphQL API
- REST Content API

### GraphQL Endpoint:
https://odin.tradoc.army.mil/api/v1/graphql

### REST Content API Documentation
https://www.dotcms.com/docs/latest/content-api-retrieval-and-querying

### Important Characteristics
- Content is organized by **Content Types**
- Content Types define:
  - Fields
  - Data types
  - Queryable properties
- Indexed fields are searchable using Lucene
- JSON fields require deserialization
- GraphQL schema discovery is not supported

### GraphQL Playground
https://www.graphqlbin.com/v2/new

---

## Content Type

A **Content Type** is a schema definition that describes a type of content stored in dotCMS.

### Characteristics
- Defines fields and their data types
- Determines which fields are queryable
- Instances represent actual content records

### Notes
- Indexed fields can be searched via Lucene
- JSON fields contain structured data and must be deserialized
- Content Types are **ODIN-specific**, even though dotCMS itself is a standard platform

---

## How the ODIN APIs Fit Together

- **Force Structure API**  
  Manages hierarchical military organization and unit data

- **DIS Enumeration API**  
  Provides standardized enumeration codes used to classify assets

- **Content API**  
  Delivers ODIN-authored content using standard retrieval technologies

Together, these APIs support ODIN’s mission by separating:
- **Structure** (Force Structure)
- **Classification** (DIS Enumerations)
- **Content** (dotCMS-backed Content API)

Each API serves a distinct role but is designed to be consumed together
by ODIN client applications.