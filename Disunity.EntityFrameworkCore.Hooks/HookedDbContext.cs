using System.Threading;
using System.Threading.Tasks;
using Disunity.EntityFrameworkCore.Hooks.Internal;
using Disunity.EntityFrameworkCore.Hooks.Internal.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Disunity.EntityFrameworkCore.Hooks
{
    public class HookedDbContext:DbContext
    {
        internal readonly HookManagerContainer _hooks;
        
        public HookedDbContext(DbContextOptions options,HookManagerContainer hooks): base(options)
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
}