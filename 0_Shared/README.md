<h1>Odin WEG GraphQL Filtering and Reporting Pipeline</h1>

<h2>Overview</h2>
<p>
This project is a C# (.NET) console application that retrieves Worldwide Equipment Guide (WEG) data from the
ODIN platform, allows structured filtering using category metadata, and prepares the data for report generation.
</p>
<p>
The application is designed as a <strong>data pipeline</strong>, where each stage produces output that feeds the next stage.
Responsibilities are separated across files to keep the codebase understandable and extensible.
</p>

<hr/>

<h2>High-Level Capabilities</h2>
<ul>
  <li>Ingests and normalizes WEG category metadata from DotCMS</li>
  <li>Caches category reference data locally to avoid repeated API calls</li>
  <li>Produces backend dropdown data for Domain and Weapon System Type selection</li>
  <li>Converts user selections into deterministic Lucene queries</li>
  <li>Executes filtered GraphQL queries against ODIN</li>
  <li>Returns strongly-typed WEG card results (name, origin, images, etc.)</li>
  <li>Validates results via console output (current phase)</li>
</ul>

<hr/>

<h2>Runtime Flow (End-to-End)</h2>
<ol>
  <li>Load configuration from <code>appsettings.json</code></li>
  <li>Fetch and normalize WEG category reference data</li>
  <li>Cache normalized category tree to disk</li>
  <li>Load cached categories and build dropdown options</li>
  <li>Capture filter selections into a criteria object</li>
  <li>Build a Lucene query from the criteria</li>
  <li>Execute a filtered GraphQL query using the Lucene string</li>
  <li>Deserialize and output filtered WEG card results</li>
</ol>

<hr/>

<h2>Key Files and Responsibilities</h2>

<h3>Program.cs</h3>
<p><strong>Role:</strong> Application entry point and orchestration</p>
<ul>
  <li>Loads and validates configuration</li>
  <li>Initializes shared services (<code>HttpClient</code>, <code>GraphQLClient</code>)</li>
  <li>Executes the application pipeline in order</li>
  <li>Prints console output for validation and debugging</li>
</ul>
<p>
<em>Program.cs does not contain business logic.</em> It coordinates calls to the other components.
</p>

<hr/>

<h3>Configuration (OdinProjectAPI.Configuration)</h3>
<p><strong>Role:</strong> Configuration binding</p>
<ul>
  <li>Defines DTOs used to bind <code>appsettings.json</code></li>
  <li>Provides strongly-typed access to ODIN endpoints and output settings</li>
  <li>Consumed during application startup</li>
</ul>

<hr/>

<h3>GraphQLClient.cs</h3>
<p><strong>Role:</strong> Low-level GraphQL transport</p>
<ul>
  <li>Executes GraphQL-over-HTTP requests against ODIN</li>
  <li>Supports parameterized queries with variables</li>
  <li>Returns raw JSON responses</li>
  <li>Centralizes HTTP and serialization concerns</li>
</ul>
<p>All GraphQL execution flows through this class.</p>

<hr/>

<h3>GraphQL DTOs (GraphQLResponse, WegCardItem, etc.)</h3>
<p><strong>Role:</strong> GraphQL response modeling</p>
<ul>
  <li>Define how ODIN GraphQL responses are deserialized</li>
  <li>Mirror the GraphQL response shape</li>
  <li>Contain no business logic</li>
</ul>
<p>These DTOs are consumed by query services and <code>Program.cs</code>.</p>

<hr/>

<h3>WebImageService.cs</h3>
<p><strong>Role:</strong> Image JSON handling</p>
<ul>
  <li>Parses the <code>images</code> field returned by WEG cards (JSON string)</li>
  <li>Converts relative image paths into absolute URLs</li>
  <li>Used during validation and future report generation</li>
</ul>

<hr/>

<h2>WEG Subnavigation Pipeline (DotCMS)</h2>

<h3>WegSubnavStructure.cs</h3>
<p><strong>Role:</strong> DotCMS WEG category ingestion and caching</p>
<ul>
  <li>Fetches the WEG subnavigation tree from DotCMS</li>
  <li>Writes raw reference data to <code>weg-subnav-raw.json</code></li>
  <li>Normalizes the category tree</li>
  <li>Writes a cached, normalized version to <code>weg-categories.json</code></li>
</ul>
<p>This is the source of all dropdown reference data.</p>

<hr/>

<h3>WEG Subnav DTOs</h3>
<p><strong>Role:</strong> Raw DotCMS modeling</p>
<ul>
  <li>Mirror the exact structure returned by the DotCMS subnav endpoint</li>
  <li>Used only during inspection and normalization</li>
  <li>Not used by filtering or query logic</li>
</ul>
<p>These exist solely to deserialize the raw reference data.</p>

<hr/>

<h3>WegCategoryNormalizer.cs</h3>
<p><strong>Role:</strong> Raw → normalized transformation</p>
<ul>
  <li>Converts DotCMS-specific structures into application-friendly models</li>
  <li>Enforces required fields (category variables)</li>
  <li>Produces a clean hierarchical tree</li>
</ul>

<hr/>

<h3>WegCategoryModels.cs</h3>
<p><strong>Role:</strong> Normalized category models</p>
<ul>
  <li>Represent categories in a predictable, stable format</li>
  <li>Used by dropdown logic, filtering, and later reporting</li>
  <li>Serialized to disk as <code>weg-categories.json</code></li>
</ul>
<p>These models are safe to use throughout the application.</p>

<hr/>

<h3>WegCategoryCacheReader.cs</h3>
<p><strong>Role:</strong> Cached category access</p>
<ul>
  <li>Loads the normalized category cache from disk</li>
  <li>Finds nodes by category variable</li>
  <li>Produces dropdown-ready <code>{ Label, Value }</code> options</li>
</ul>
<p>This bridges cached reference data to user selection logic.</p>

<hr/>

<h3>WegFilterCriteria.cs</h3>
<p><strong>Role:</strong> Filter selection contract</p>
<ul>
  <li>Stores selected Domain, Weapon System Type, and future filters</li>
  <li>Serves as the single input to the Lucene query builder</li>
</ul>
<p>This keeps filter state explicit and centralized.</p>

<hr/>

<h3>LuceneQueryBuilder.cs</h3>
<p><strong>Role:</strong> Server-side filter construction</p>
<ul>
  <li>Converts <code>WegFilterCriteria</code> into a Lucene query string</li>
  <li>Enforces deterministic output</li>
  <li>Encapsulates Lucene syntax rules required by ODIN</li>
</ul>
<p>The generated query feeds directly into GraphQL requests.</p>

<hr/>

<h3>WegCardQueryService.cs</h3>
<p><strong>Role:</strong> Filtered WEG card retrieval</p>
<ul>
  <li>Executes GraphQL queries using Lucene filters</li>
  <li>Deserializes results into strongly-typed <code>WegCardItem</code> lists</li>
  <li>Returns filtered WEG card data to the caller</li>
</ul>
<p>This is the boundary between filtering logic and WEG content retrieval.</p>