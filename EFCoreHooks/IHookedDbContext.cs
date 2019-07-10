using System.Threading;
using System.Threading.Tasks;

namespace EFCoreHooks
{
    public interface IHookedDbContext
    {
        HookManagerContainer Hooks { get; }

        int SaveChangesBase(bool acceptAllChanges);

        Task<int> SaveChangesBaseAsync(bool acceptAllChanges,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}