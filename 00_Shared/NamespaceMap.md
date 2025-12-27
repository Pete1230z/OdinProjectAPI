# Namespace Map and Architectural Intent

This document describes the intended namespace structure for the OdinProjectAPI
solution. It exists as architectural guidance only.

At this stage of the project, namespaces do not strictly mirror folder structure.
Folders are used for human organization and execution order, while namespaces
remain stable to avoid unnecessary refactors during active development.

---

## Current Namespace Usage

The following namespaces are currently in use:

- `OdinProjectAPI.Configuration`
- `OdinProjectAPI.GraphQL`
- `OdinProjectAPI.WegSubnav`

These namespaces are intentionally coarse-grained and may be refined later.

---

## Intended Logical Namespace Layers

Over time, the project may converge toward the following conceptual layers:

### Configuration
```text
OdinProjectAPI.Configuration

Purpose:

Configuration DTOs

Binding targets for appsettings.json
```

### Transport / Protocol
```text
OdinProjectAPI.Transport.GraphQL

Purpose:

GraphQL-over-HTTP transport

Raw API response DTOs
```

### Ingestion (Metadata)
```text
OdinProjectAPI.Ingestion.DotCms

Purpose:

DotCMS subnavigation ingestion

Raw metadata DTOs

One-time reference data fetch
```

### Processing (Categories)
```text
OdinProjectAPI.Categories

Purpose:

Normalized category models

Category normalization

Category cache access
```

### Querying (Content)
```text
OdinProjectAPI.Querying.WegCards

Purpose:

Lucene query construction

WEG card retrieval

Read-only repositories
```

### Output
```text
OdinProjectAPI.Output

Purpose:

Image parsing

Report generation

Export formatting (Excel, PDF, etc.)
```
