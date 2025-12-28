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
```

### Sealed Class

A class that cannot be inherited.

    Prevents other classes from deriving from it

    Common for DTOs and configuration classes

    Can improve performance slightly

```csharp
public sealed class OdinSettingsDTO
{
    public string? GraphQLEndPoint { get; set; }
}
```

### Abstract Class

A class that cannot be instantiated directly.

    Intended to be inherited

    Can contain abstract members (no implementation)

    Can contain concrete members

```csharp
public abstract class BaseService
{
    public abstract void Run();
}
```

### Static Class

A class that cannot be instantiated or inherited.

    Contains only static members

    Used for utility or helper functionality

```csharp
public static class ConfigurationHelpers
{
    public static void Validate() { }
}
```

### Partial Class

A class whose definition is split across multiple files.

    All parts combined at compile time

    Common with code generation

```csharp
public partial class UserModel
{
    public string Name { get; set; }
}
```

### Generic Class

A class defined with type parameters.

    Allows reuse with different data types

    Enforced at compile time

```csharp
public class Repository<T>
{
    public T Get() => default!;
}
```

### Record Class

A reference type optimized for immutable data.

    Value-based equality

    Common for DTOs and data models

```csharp
public record UserRecord(string Name, int Id);
```

### Nested Class

A class declared inside another class.

    Scope limited to containing class

    Used for logical grouping

```csharp
public class Outer
{
    public class Inner { }
}
```
---

## Object-Oriented Relationships (C#)

### Inheritance

A relationship where one class derives from another class.

    Used to extend or specialize behavior

    Child class automatically has access to accessible members of the base class

    Established at compile time

    Expresses an “is-a” relationship

```csharp
public class BaseService
{
    public void Log() { }
}

public class UserService : BaseService
{
    public void Execute() { }
}
```

---

## Object Lifecycle (C#)

### Instantiation

The act of creating a runtime instance of a class.

    Allocates memory for the object

    Calls the constructor

    Happens at runtime

    Produces an object that can be used

```csharp
var service = new UserService();
```

### Object (Instance)

A runtime instance of a class.

    Created using new

    Exists in memory at runtime

```csharp
var settings = new OdinSettings();
```
---

## Class Members (C#)

### Property

A named member of a class that exposes a value through accessors.

    Has a type

    Has a name

    Has one or more accessors (get, set)

    Commonly used for configuration and data models

```csharp
public string? ForceStructureAPI { get; set; }
```

### Field

A variable declared directly inside a class.

    Stores data directly

    Typically private

    Less flexible than properties

    Has a type

    Has a name

```csharp
private string _baseUrl;
```

### Method

A block of code inside a class that performs an action.

    Contains executable logic

    Can accept parameters

    Can return a value or void

```csharp
public void LoadConfiguration() { }
```

### Constructor

A special method used to create and initialize an object.

    Same name as the class

    Runs when new is called

    No return type

```csharp
public OdinSettings() { }
```
---

## Type System (C#)

### Type

Defines what kind of data a variable, property, or parameter can hold.

    Determines valid operations

    Enforced at compile time

#### Reference Type: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/reference-types

    Stored on the heap

    Passed by reference

Examples:

class
string

#### Value Type: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-types

    Copied by value

    Stored directly

Examples:

int
bool
struct

### Nullable

Indicates a value may be null.

    Used with reference types

    Important for configuration binding

string?

### Null

Represents the absence of a value.

    Valid state unless explicitly prevented

    Must be checked before use

```csharp
if (value == null)
{
}
```
---

## Encapsulation and Visibility (C#)

### Access Modifiers

Control visibility of classes and members.

    public — Accessible from anywhere

    private — Accessible only within the containing class

    internal — Accessible within the same assembly

## Program Structure (.NET)

### Namespace

A logical grouping of related classes.

    Prevents name collisions

    Organizes code

```csharp
namespace OdinProjectAPI;
```

### Program

The entry point of a .NET application.

    Execution starts here

    Typically located in Program.cs

## Application Configuration (.NET)

### Configuration

A system for supplying values to an application at runtime.

    External to compiled code

    Environment-specific

### Configuration Binding

Maps configuration values to C# objects by convention.

    Property name ↔ configuration key

    Class structure ↔ JSON structure

## Data and Integration Concepts

### JSON

A text-based data format for structured data.

    Uses key–value pairs

    Human-readable

```json
{
  "Odin": {
    "GraphQLEndPoint": "..."
  }
}
```

### DTO (Data Transfer Object)

A class used only to carry data.

    Properties only

    No behavior

    Common for configuration and API responses

```csharp
public class OdinSettingsDto
{
    public string? GraphQLEndPoint { get; set; }
    public string? OutputFolder { get; set; }
}
```

## API (Application Programming Interface)

A defined contract that allows software systems to communicate.

    Exposes data and functionality

    Accessed via HTTP requests

    Returns structured responses (JSON)