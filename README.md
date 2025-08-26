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

### Build
```bash
# from repo root
 dotnet build src/VSS/VSS.csproj -c Release
```

### License
Licensed under the MIT License. See `LICENSE` for details. 