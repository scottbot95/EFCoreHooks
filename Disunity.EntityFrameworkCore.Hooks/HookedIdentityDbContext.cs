using System;
using System.Threading;
using System.Threading.Tasks;
using Disunity.EntityFrameworkCore.Hooks.Internal;
using Disunity.EntityFrameworkCore.Hooks.Internal.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Disunity.EntityFrameworkCore.Hooks
{
    public class HookedIdentityDbContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        internal readonly HookManagerContainer _hooks;
        
        public HookedIdentityDbContext(DbContextOptions options,HookManagerContainer hooks): base(options)
        {
            _hooks = hooks;
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return this.HookedSaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.HookedSaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }

//    public class Foo: HookedIdentityDbContext<IdentityUser>{}
}