# EFCoreHooks

This project provides a simple, attribute-based hooking system for all projects using all versions
of Microsoft's [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) 2.1 and above.

## Installation

Make sure you have [Nuget](http://docs.nuget.org/docs/start-here/installing-nuget) installed, the run
```
PM> Install-Package EFCoreHooks
```

## Usage

First you must edit your `ConfigureServices(IServiceCollection)` so the hooking system can have access
to the Dependency Injection Container

```c#
using EFCoreHooks;

/* ... */

public void ConfigureServices(IServiceColleciton services) {
   /* ... */
   services.ConfigureDbHooks();
   /* ... */
}
```

In most cases the only other thing you need to do is to have any dbcontext that you want to have hooks
extends from `HookedDbContext` or `HookedIdentityDbContext`. Both classes are designed to be drop-in
replacements for `DbContext` and `IdentityDbContext` respectively.

In cases where you need a base class other than `DbContext` or `IdentityDbContext`, you can use the
extension methods directly by implementing `IHookedDbContext`, and overriding the `SaveChanges` and
`SaveChagnesAsync` methods to call the extension methods instead of the base class.

```c#
public override int SaveChanges(bool acceptAllChangesOnSuccess)
{
    return this.HookedSaveChanges(acceptAllChangesOnSuccess);
}

public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
    CancellationToken cancellationToken = default(CancellationToken))
{
    return this.HookedSaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
}
```

Implementing the `IHookedDbContext` interface is quite simple:

```c#
public class HookedDbContext : DbContext, IHookedDbContext
{
    public HookedDbContext(DbContextOptions options, HookManagerContainer hooks) : base(options)
    {
        Hooks = hooks;
        Hooks.InitializeForAll(this);
    }

    public HookManagerContainer Hooks { get; }

    public int SaveChangesBase(bool acceptAllChanges)
    {
        // Mirror the base classes saved changes so the extension methods can access them
        return base.SaveChanges(acceptAllChanges);
    }

    public Task<int> SaveChangesBaseAsync(bool acceptAllChanges,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        // Mirror the base classes saved changes so the extension methods can access them
        return base.SaveChangesAsync(acceptAllChanges, cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        // Run the extension method instead of the base class's method
        return this.HookedSaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        // Run the extension method instead of the base class's method
        return this.HookedSaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
```
