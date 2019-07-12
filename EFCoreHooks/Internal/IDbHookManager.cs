using System.Threading.Tasks;
using EFCoreHooks.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EFCoreHooks.Internal
{
    public interface IDbHookManager<T> where T : DbHookAttribute
    {
        void InitializeForContext(DbContext context);

        Task ExecuteForEntity(DbContext context, EntityEntry entityEntry);
    }
}