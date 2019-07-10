using System;
using System.Threading;
using System.Threading.Tasks;
using Disunity.EntityFrameworkCore.Hooks.Extensions;
using Disunity.EntityFrameworkCore.Hooks.Internal.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Disunity.EntityFrameworkCore.Hooks
{
    public class HookedIdentityDbContext : HookedIdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public HookedIdentityDbContext(DbContextOptions options, HookManagerContainer hooks) : base(options, hooks)
        {
        }
    }

    public class HookedIdentityDbContext<TUser> : HookedIdentityDbContext<TUser, IdentityRole, string>
        where TUser : IdentityUser<string>
    {
        public HookedIdentityDbContext(DbContextOptions options, HookManagerContainer hooks) : base(options, hooks)
        {
        }
    }

    public class HookedIdentityDbContext<TUser, TRole> : HookedIdentityDbContext<TUser, TRole, string>
        where TUser : IdentityUser<string>
        where TRole : IdentityRole<string>
    {
        public HookedIdentityDbContext(DbContextOptions options, HookManagerContainer hooks) : base(options, hooks)
        {
        }
    }

    public class HookedIdentityDbContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey>, IHookedDbContext
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        public HookManagerContainer Hooks { get; }

        public HookedIdentityDbContext(DbContextOptions options, HookManagerContainer hooks) : base(options)
        {
            Hooks = hooks;
            Hooks.InitializeForAll(this);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return this.HookedSaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.HookedSaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public int SaveChangesBase(bool acceptAllChanges)
        {
            return base.SaveChanges(acceptAllChanges);
        }

        public Task<int> SaveChangesBaseAsync(bool acceptAllChanges,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return base.SaveChangesAsync(acceptAllChanges, cancellationToken);
        }
    }
}