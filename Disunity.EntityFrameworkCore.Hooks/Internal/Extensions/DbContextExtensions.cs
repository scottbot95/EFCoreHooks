using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Disunity.EntityFrameworkCore.Hooks.Internal.Extensions
{
    public static class DbContextExtensions
    {
        private static IEnumerable<Type> DbSetTypes(this DbContext context)
        {
            return context.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => p.PropertyType)
                .Where(t => t.IsGenericType)
                .Where(t => typeof(DbSet<>).IsAssignableFrom(t.GetGenericTypeDefinition()));
        }

        internal static IEnumerable<Type> DbEntityTypes(this DbContext context)
        {
            return context.DbSetTypes().Select(t => t.GetGenericArguments()[0]);
        }

        public static int HookedSaveChanges(this HookedDbContext dbContext, bool acceptAllChangesOnSuccess)
        {
            var changedEntities = dbContext._hooks.BeforeSave(dbContext);
            var numChanges = dbContext.SaveChanges(acceptAllChangesOnSuccess);
            dbContext._hooks.AfterSave(dbContext, changedEntities);
            return numChanges;
        }

        public static async Task<int> HookedSaveChangesAsync(this HookedDbContext dbContext,
            bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var changedEntities = dbContext._hooks.BeforeSave(dbContext);
            var numChanges = await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            dbContext._hooks.AfterSave(dbContext, changedEntities);
            return numChanges;
        }

        public static int HookedSaveChanges<TUser, TRole, TKey>(
            this HookedIdentityDbContext<TUser, TRole, TKey> dbContext, bool acceptAllChangesOnSuccess)
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
        {
            var changedEntities = dbContext._hooks.BeforeSave(dbContext);
            var numChanges = dbContext.SaveChanges(acceptAllChangesOnSuccess);
            dbContext._hooks.AfterSave(dbContext, changedEntities);
            return numChanges;
        }

        public static async Task<int> HookedSaveChangesAsync<TUser, TRole, TKey>(
            this HookedIdentityDbContext<TUser, TRole, TKey> dbContext,
            bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
        {
            var changedEntities = dbContext._hooks.BeforeSave(dbContext);
            var numChanges = await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            dbContext._hooks.AfterSave(dbContext, changedEntities);
            return numChanges;
        }
    }
}