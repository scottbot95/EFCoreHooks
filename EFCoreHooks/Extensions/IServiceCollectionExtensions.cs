using EFCoreHooks.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace EFCoreHooks.Extensions
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