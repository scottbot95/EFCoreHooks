using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFCoreHooks.Extensions
{
    public static class DbContextExtensions
    {
        public static int HookedSaveChanges<TContext>(this TContext dbContext, bool acceptAllChangesOnSuccess)
            where TContext : DbContext, IHookedDbContext
        {
            var changedEntities = dbContext.Hooks.BeforeSave(dbContext).Result;
            var numChanges = dbContext.SaveChangesBase(acceptAllChangesOnSuccess);
            dbContext.Hooks.AfterSave(dbContext, changedEntities).Wait();
            return numChanges;
        }

        public static async Task<int> HookedSaveChangesAsync<TContext>(this TContext dbContext,
            bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
            where TContext : DbContext, IHookedDbContext
        {
            var changedEntities = await dbContext.Hooks.BeforeSave(dbContext);
            var numChanges = await dbContext.SaveChangesBaseAsync(acceptAllChangesOnSuccess, cancellationToken);
            await dbContext.Hooks.AfterSave(dbContext, changedEntities);
            return numChanges;
        }
    }
}