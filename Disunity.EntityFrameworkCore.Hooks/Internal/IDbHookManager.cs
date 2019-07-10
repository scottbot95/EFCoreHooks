using Disunity.EntityFrameworkCore.Hooks.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Disunity.EntityFrameworkCore.Hooks.Internal {

    public interface IDbHookManager<T> where T : DbHookAttribute {

        void InitializeForContext(DbContext context);

        void ExecuteForEntity(DbContext context, EntityEntry entityEntry);

    }

}