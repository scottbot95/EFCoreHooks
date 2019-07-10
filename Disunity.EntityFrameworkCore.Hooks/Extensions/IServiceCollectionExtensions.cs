using Disunity.EntityFrameworkCore.Hooks.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Disunity.EntityFrameworkCore.Hooks.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureDbHooks(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IDbHookManager<>), typeof(DbHookManager<>));
            services.AddSingleton(typeof(HookManagerContainer));
        }
    }
}