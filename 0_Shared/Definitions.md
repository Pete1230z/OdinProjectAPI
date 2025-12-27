<h1>Project Definitions and Concepts</h1>

<p>
This document defines core C# language concepts and ODIN-specific APIs used throughout the project.
It exists as a reference to clarify terminology, design choices, and architectural decisions.
</p>

<hr/>

<h2>Class Types (C#)</h2>

<h3>Regular Class</h3>
<p>A standard class that can be instantiated and inherited.</p>
<ul>
  <li>Can contain fields, properties, methods, constructors</li>
  <li>Can be inherited unless marked <code>sealed</code></li>
</ul>
<pre>
public class UserService
{
    public void Execute() { }
}
</pre>

<hr/>

<h3>Sealed Class</h3>
<p>A class that cannot be inherited.</p>
<ul>
  <li>Prevents other classes from deriving from it</li>
  <li>Common for DTOs and configuration classes</li>
  <li>Can improve performance slightly</li>
</ul>
<pre>
public sealed class OdinSettingsDTO
{
    public string? GraphQLEndPoint { get; set; }
}
</pre>

<hr/>

<h3>Abstract Class</h3>
<p>A class that cannot be instantiated directly.</p>
<ul>
  <li>Intended to be inherited</li>
  <li>Can contain abstract members (no implementation)</li>
  <li>Can contain concrete members</li>
</ul>
<pre>
public abstract class BaseService
{
    public abstract void Run();
}
</pre>

<hr/>

<h3>Static Class</h3>
<p>A class that cannot be instantiated or inherited.</p>
<ul>
  <li>Contains only static members</li>
  <li>Used for utility or helper functionality</li>
</ul>
<pre>
public static class ConfigurationHelpers
{
    public static void Validate() { }
}
</pre>

<hr/>

<h3>Partial Class</h3>
<p>A class whose definition is split across multiple files.</p>
<ul>
  <li>All parts combined at compile time</li>
  <li>Common with code generation</li>
</ul>
<pre>
public partial class UserModel
{
    public string Name { get; set; }
}
</pre>

<hr/>

<h3>Generic Class</h3>
<p>A class defined with type parameters.</p>
<ul>
  <li>Allows reuse with different data types</li>
  <li>Enforced at compile time</li>
</ul>
<pre>
public class Repository&lt;T&gt;
{
    public T Get() =&gt; default!;
}
</pre>

<hr/>

<h3>Record Class</h3>
<p>A reference type optimized for immutable data.</p>
<ul>
  <li>Value-based equality</li>
  <li>Common for DTOs and data models</li>
</ul>
<pre>
public record UserRecord(string Name, int Id);
</pre>

<hr/>

<h3>Nested Class</h3>
<p>A class declared inside another class.</p>
<ul>
  <li>Scope limited to containing class</li>
  <li>Used for logical grouping</li>
</ul>
<pre>
public class Outer
{
    public class Inner { }
}
</pre>

<hr/>

<h2>Core OOP Concepts</h2>

<h3>Inheritance</h3>
<p>A relationship where one class derives from another.</p>
<ul>
  <li>Used to extend or specialize behavior</li>
  <li>Expresses an “is-a” relationship</li>
</ul>
<pre>
public class BaseService
{
    public void Log() { }
}

public class UserService : BaseService
{
    public void Execute() { }
}
</pre>

<hr/>

<h3>Instantiation</h3>
<p>The act of creating a runtime instance of a class.</p>
<pre>
var service = new UserService();
</pre>

<hr/>

<h3>Property</h3>
<p>A named member that exposes a value through accessors.</p>
<pre>
public string? ForceStructureAPI { get; set; }
</pre>

<hr/>

<h3>Field</h3>
<p>A variable declared directly inside a class.</p>
<pre>
private string _baseUrl;
</pre>

<hr/>

<h3>Method</h3>
<p>A block of code inside a class that performs an action.</p>
<pre>
public void LoadConfiguration() { }
</pre>

<hr/>

<h3>Constructor</h3>
<p>A special method used to create and initialize an object.</p>
<pre>
public OdinSettings() { }
</pre>

<hr/>

<h2>Types and Nullability</h2>

<h3>Object (Instance)</h3>
<p>A runtime instance of a class.</p>
<pre>
var settings = new OdinSettings();
</pre>

<h3>Type</h3>
<ul>
  <li><strong>Reference Types:</strong> class, string (stored on heap)</li>
  <li><strong>Value Types:</strong> int, bool, struct (copied by value)</li>
</ul>

<h3>Nullable</h3>
<p>Indicates a value may be null.</p>
<pre>
string?
</pre>

<h3>Null</h3>
<p>Represents the absence of a value.</p>
<pre>
if (value == null)
</pre>

<hr/>

<h2>Namespaces and Access</h2>

<h3>Access Modifiers</h3>
<ul>
  <li><code>public</code> – accessible everywhere</li>
  <li><code>private</c
