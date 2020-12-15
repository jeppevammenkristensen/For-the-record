# For-the-record
A code refactoring for Visual Studio to enable converting a record to a class

This is a tool lazy people like my self. It allows you to convert a record like this

```csharp
public record SomeRecord (int Id, string Name, int Number) { }
```

And the code refactoring will suggest either to **Convert to class**

```csharp
public class SomeRecord
{
    public int Id {get;set;}
    
    public string Name {get;set;}
    
    public int Number {get;set;}
    
}
```

Or **Convert to class (init properties from constructor)**

```csharp
public class SomeRecord
{
    public int Id {get;set;}
    
    public string Name {get;set;}
    
    public int Number {get;set;}
    
    public SomeRecord(int id, string name, int number)
    {
      this.Id = id;
      this.Name = name;
      this.Number = number
    }

}
```
