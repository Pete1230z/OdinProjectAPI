This file exists to document definitions.

---

## Class Types (C#)

### Regular Class
A standard class that can be instantiated and inherited.

- Can contain fields, properties, methods, constructors
- Can be inherited unless marked `sealed`

```csharp
public class UserService
{
    public void Execute() { }
}

Sealed Class

A class that cannot be inherited.

    Prevents other classes from deriving from it

    Common for DTOs and configuration classes

    Can improve performance slightly

public sealed class OdinSettingsDTO
{
    public string? GraphQLEndPoint { get; set; }
}

Abstract Class

A class that cannot be instantiated directly.

    Intended to be inherited

    Can contain abstract members (no implementation)

    Can contain concrete members

public abstract class BaseService
{
    public abstract void Run();
}

Static Class

A class that cannot be instantiated or inherited.

    Contains only static members

    Used for utility or helper functionality

public static class ConfigurationHelpers
{
    public static void Validate() { }
}

Partial Class

A class whose definition is split across multiple files.

    All parts combined at compile time

    Common with code generation

public partial class UserModel
{
    public string Name { get; set; }
}

Generic Class

A class defined with type parameters.

    Allows reuse with different data types

    Enforced at compile time

public class Repository<T>
{
    public T Get() => default!;
}

Record Class

A reference type optimized for immutable data.

    Value-based equality

    Common for DTOs and data models

public record UserRecord(string Name, int Id);

Nested Class

A class declared inside another class.

    Scope limited to containing class

    Used for logical grouping

public class Outer
{
    public class Inner { }
}

Object-Oriented Relationships (C#)
Inheritance

A relationship where one class derives from another class.

    Used to extend or specialize behavior

    Child class automatically has access to accessible members of the base class

    Established at compile time

    Expresses an “is-a” relationship

public class BaseService
{
    public void Log() { }
}

public class UserService : BaseService
{
    public void Execute() { }
}

Object Lifecycle (C#)
Instantiation

The act of creating a runtime instance of a class.

    Allocates memory for the object

    Calls the constructor

    Happens at runtime

    Produces an object that can be used

var service = new UserService();

Object (Instance)

A runtime instance of a class.

    Created using new

    Exists in memory at runtime

var settings = new OdinSettings();

Class Members (C#)
Property

A named member of a class that exposes a value through accessors.

    Has a type

    Has a name

    Has one or more accessors (get, set)

    Commonly used for configuration and data models

public string? ForceStructureAPI { get; set; }

Field

A variable declared directly inside a class.

    Stores data directly

    Typically private

    Less flexible than properties

    Has a type

    Has a name

private string _baseUrl;

Method

A block of code inside a class that performs an action.

    Contains executable logic

    Can accept parameters

    Can return a value or void

public void LoadConfiguration() { }

Constructor

A special method used to create and initialize an object.

    Same name as the class

    Runs when new is called

    No return type

public OdinSettings() { }

Type System (C#)
Type

Defines what kind of data a variable, property, or parameter can hold.

    Determines valid operations

    Enforced at compile time

Reference Type

    Stored on the heap

    Passed by reference

Examples:

class
string

Value Type

    Copied by value

    Stored directly

Examples:

int
bool
struct

Nullable

Indicates a value may be null.

    Used with reference types

    Important for configuration binding

string?

Null

Represents the absence of a value.

    Valid state unless explicitly prevented

    Must be checked before use

if (value == null)
{
}

Encapsulation and Visibility (C#)
Access Modifiers

Control visibility of classes and members.

    public — Accessible from anywhere

    private — Accessible only within the containing class

    internal — Accessible within the same assembly

Program Structure (.NET)
Namespace

A logical grouping of related classes.

    Prevents name collisions

    Organizes code

namespace OdinProjectAPI;

Program

The entry point of a .NET application.

    Execution starts here

    Typically located in Program.cs

Application Configuration (.NET)
Configuration

A system for supplying values to an application at runtime.

    External to compiled code

    Environment-specific

Configuration Binding

Maps configuration values to C# objects by convention.

    Property name ↔ configuration key

    Class structure ↔ JSON structure

Data and Integration Concepts
JSON

A text-based data format for structured data.

    Uses key–value pairs

    Human-readable

{
  "Odin": {
    "GraphQLEndPoint": "..."
  }
}

DTO (Data Transfer Object)

A class used only to carry data.

    Properties only

    No behavior

    Common for configuration and API responses

public class OdinSettingsDto
{
    public string? GraphQLEndPoint { get; set; }
    public string? OutputFolder { get; set; }
}

API (Application Programming Interface)

A defined contract that allows software systems to communicate.

    Exposes data and functionality

    Accessed via HTTP requests

    Returns structured responses (JSON)


ODIN FORCE STRUCTURE API (NON-STANDARD / ODIN-SPECIFIC)
An ODIN-specific REST API for managing and retrieving military force structure data.

- Used to create, modify, and delete force structures (authenticated)
- Allows public read-only GET requests for approved data
- Schema is hierarchical and organization-centric
- Not an industry-standard API

Key Concepts:
- Organization: Top-level entity (exercise countries, insurgent groups)
- top_unit: Foreign key referencing the root unit of a force structure
- Unit hierarchy: Recursive structure of units under a top unit
- Template: Foreign key defining standardized unit composition

Base URL:
https://odin.tradoc.army.mil/FSAPI/help/

Example Endpoints:
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


DIS ENUMERATION API (SEMI-STANDARD / DOMAIN-SPECIFIC)
An ODIN-hosted API that exposes DIS (Distributed Interactive Simulation) enumeration data.

- Stores SISO DIS enumeration codes
- Used for simulation and modeling systems
- Domain-specific, not general-purpose
- Based on DIS standards but exposed via a custom ODIN API

Use Cases:
- Assigning enumeration codes to force structure assets
- Assigning enumeration properties to WEG assets

Base URL:
https://odin.tradoc.army.mil/DISAPI/help/

Key Endpoints:
- Enumerations:
  https://odin.tradoc.army.mil/DISAPI/enumeration

- Countries:
  https://odin.tradoc.army.mil/DISAPI/enumeration/countries

- Kinds:
  https://odin.tradoc.army.mil/DISAPI/enumeration/kinds

- Domains:
  https://odin.tradoc.army.mil/DISAPI/enumeration/domains

Notes:
- Entries represent Kinds, Domains, or Categories
- Relationships expressed via foreign keys in the enumeration table


ODIN CONTENT API (BUILT ON STANDARD TECHNOLOGIES)
A content retrieval API backed by dotCMS.

- Uses industry-standard technologies
- ODIN-specific schemas and content types
- Read-only access does not require authentication

Underlying Technologies (STANDARD):
- REST
- GraphQL
- Lucene query syntax
- dotCMS content platform

As of Version:
ODIN 3.0.0+

Retrieval Methods:
- GraphQL API
- REST Content API

GraphQL Endpoint:
https://odin.tradoc.army.mil/api/v1/graphql

REST Content API:
https://www.dotcms.com/docs/latest/content-api-retrieval-and-querying

Important Characteristics:
- Content organized by Content Types
- Content Types define fields and queryable properties
- Indexed fields are searchable
- JSON fields require deserialization
- GraphQL introspection is disabled in ODIN

GraphQL Playground:
https://www.graphqlbin.com/v2/new

CONTENT TYPE
A schema definition that describes a type of content stored in dotCMS.

- Defines fields and data types
- Determines which fields are queryable
- Instances represent actual content records

Notes:
- Indexed fields can be searched via Lucene
- JSON fields contain structured data
- Content Types are ODIN-specific even though dotCMS is standard

