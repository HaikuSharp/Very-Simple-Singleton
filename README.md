## Very Simple Singleton (VSS)

A tiny, dependency-free helper for registering and managing simple singletons in .NET.

### Highlights
- **Minimal API**: register lazy factory, lazy instance, or concrete instance
- **Type-safe**: generic, per-type management
- **Lifecycle**: `IsRegistered`, `IsCreated`, `Unregister`, `Dispose`

### Quick Start
```csharp
using VSS;

// Register lazily via factory
Singleton.RegisterLazy(() => new MyService());

// Or register an existing instance
// Singleton.RegisterInstance(new MyService());

// Or register a provided Lazy<T>
// Singleton.RegisterLazy(new Lazy<MyService>(() => new MyService(), isThreadSafe: true));

// Check status
bool registered = Singleton.IsRegistered<MyService>();
bool created = Singleton.IsCreated<MyService>();

// Dispose (if IDisposable) and unregister
Singleton.Dispose<MyService>();

// Or just unregister without disposing
// Singleton.Unregister<MyService>();

public class MyService : IDisposable
{
    public void Dispose() { /* cleanup */ }
}
```

### Accessing the instance
VSS manages registration and lifecycle. Access patterns depend on how you registered:

- If you registered a provided `Lazy<T>` (recommended when you need direct access):
```csharp
Singleton.RegisterLazy(() => new MyService());

// Access instance via your Lazy
MyService instance = Singleton.GetInstance<MyService>();
```

- If you registered an existing instance:
```csharp
var service = new MyService();
Singleton.RegisterInstance(service);

// Access the same instance you registered
MyService instance = service;
```

- Create a small wrapper to centralize access:
```csharp
using VSS;

public static class MyServiceSingleton
{
    private static readonly Lazy<MyService> Lazy = new(() => new MyService(), true);

    public static void Register() => Singleton.RegisterLazy(Lazy);

    public static MyService Instance => Lazy.Value;
}

// Usage
MyServiceSingleton.Register();
var instance = MyServiceSingleton.Instance;
```

### License
Licensed under the MIT License. See `LICENSE` for details. 