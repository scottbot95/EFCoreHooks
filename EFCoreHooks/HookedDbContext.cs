using System.Threading;
using System.Threading.Tasks;
using EFCoreHooks.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EFCoreHooks
{
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
            return base.SaveChanges(acceptAllChanges);
        }

        public Task<int> SaveChangesBaseAsync(bool acceptAllChanges,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return base.SaveChangesAsync(acceptAllChanges, cancellationToken);
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
    }
}