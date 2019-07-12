using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreHooks.Attributes;
using EFCoreHooks.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EFCoreHooks
{
    public class HookManagerContainer
    {
        public HookManagerContainer(IDbHookManager<OnBeforeCreate> onBeforeCreate,
            IDbHookManager<OnBeforeUpdate> onBeforeUpdate,
            IDbHookManager<OnBeforeSave> onBeforeSave,
            IDbHookManager<OnBeforeDelete> onBeforeDelete,
            IDbHookManager<OnAfterCreate> onAfterCreate,
            IDbHookManager<OnAfterUpdate> onAfterUpdate,
            IDbHookManager<OnAfterSave> onAfterSave,
            IDbHookManager<OnAfterDelete> onAfterDelete)
        {
            OnBeforeCreate = onBeforeCreate;
            OnBeforeUpdate = onBeforeUpdate;
            OnBeforeSave = onBeforeSave;
            OnBeforeDelete = onBeforeDelete;
            OnAfterCreate = onAfterCreate;
            OnAfterUpdate = onAfterUpdate;
            OnAfterSave = onAfterSave;
            OnAfterDelete = onAfterDelete;
        }

        public IDbHookManager<OnBeforeCreate> OnBeforeCreate { get; }
        public IDbHookManager<OnBeforeUpdate> OnBeforeUpdate { get; }
        public IDbHookManager<OnBeforeSave> OnBeforeSave { get; }
        public IDbHookManager<OnBeforeDelete> OnBeforeDelete { get; }
        public IDbHookManager<OnAfterCreate> OnAfterCreate { get; }
        public IDbHookManager<OnAfterUpdate> OnAfterUpdate { get; }
        public IDbHookManager<OnAfterSave> OnAfterSave { get; }
        public IDbHookManager<OnAfterDelete> OnAfterDelete { get; }

        public void InitializeForAll(DbContext context)
        {
            OnBeforeCreate.InitializeForContext(context);
            OnBeforeUpdate.InitializeForContext(context);
            OnBeforeSave.InitializeForContext(context);
            OnBeforeDelete.InitializeForContext(context);
            OnAfterCreate.InitializeForContext(context);
            OnAfterUpdate.InitializeForContext(context);
            OnAfterSave.InitializeForContext(context);
            OnAfterDelete.InitializeForContext(context);
        }

        public async Task<SavedChanges> BeforeSave(DbContext dbContext)
        {
            var changes = new SavedChanges();
            var handledModels = new HashSet<object>();
            int prevHandledCount;

            do
            {
                prevHandledCount = handledModels.Count;
                var entries = dbContext.ChangeTracker.Entries().ToList();

                foreach (var entry in entries)
                {
                    if (handledModels.Contains(entry.Entity)) continue; // already processed dbContext entity, skip it

                    handledModels.Add(entry.Entity);

                    switch (entry.State)
                    {
                        case EntityState.Deleted:
                            await OnBeforeDelete.ExecuteForEntity(dbContext, entry);
                            changes.Deleted.Add(entry);
                            break;

                        case EntityState.Modified:
                            await OnBeforeUpdate.ExecuteForEntity(dbContext, entry);
                            changes.Modified.Add(entry);
                            break;

                        case EntityState.Added:
                            await OnBeforeCreate.ExecuteForEntity(dbContext, entry);
                            changes.Added.Add(entry);
                            break;
                    }

                    if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                    {
                        await OnBeforeSave.ExecuteForEntity(dbContext, entry);
                        changes.Saved.Add(entry);
                    }
                }
            } while (handledModels.Count != prevHandledCount);


            return changes;
        }

        public async Task AfterSave(DbContext dbContext, SavedChanges changes)
        {
            foreach (var entity in changes.Added) await OnAfterCreate.ExecuteForEntity(dbContext, entity);

            foreach (var entity in changes.Modified) await OnAfterUpdate.ExecuteForEntity(dbContext, entity);

            foreach (var entity in changes.Deleted) await OnAfterDelete.ExecuteForEntity(dbContext, entity);

            foreach (var entity in changes.Saved) await OnAfterSave.ExecuteForEntity(dbContext, entity);
        }

        public class SavedChanges
        {
            public SavedChanges()
            {
                Added = new List<EntityEntry>();
                Modified = new List<EntityEntry>();
                Deleted = new List<EntityEntry>();
                Saved = new List<EntityEntry>();
            }

            public IList<EntityEntry> Added { get; }
            public IList<EntityEntry> Modified { get; }
            public IList<EntityEntry> Deleted { get; }
            public IList<EntityEntry> Saved { get; }
        }
    }
}