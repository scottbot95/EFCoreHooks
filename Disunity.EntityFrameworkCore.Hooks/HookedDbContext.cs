using System.Threading;
using System.Threading.Tasks;
using Disunity.EntityFrameworkCore.Hooks.Extensions;
using Disunity.EntityFrameworkCore.Hooks.Internal.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Disunity.EntityFrameworkCore.Hooks
{
    public class HookedDbContext : DbContext, IHookedDbContext
    {
        public HookManagerContainer Hooks { get; }

        public HookedDbContext(DbContextOptions options, HookManagerContainer hooks) : base(options)
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