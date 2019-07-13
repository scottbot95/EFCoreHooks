using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EFCoreHooks.Attributes;
using EFCoreHooks.Internal.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace EFCoreHooks.Internal
{
    public class DbHookManager<T> : IDbHookManager<T> where T : DbHookAttribute
    {
        private static readonly List<MethodBase> AssemblyHookMethods;

        private readonly IDictionary<DbContext, HookMap> _contextHooks = new Dictionary<DbContext, HookMap>();
        private readonly ILogger<DbHookManager<T>> _logger;
        private readonly IServiceProvider _serviceProvider;

        static DbHookManager()
        {
            AssemblyHookMethods = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(GetAttributesFromAssembly)
                .Where(m => m.GetCustomAttributes<T>().Any())
                .ToList();
        }

        public DbHookManager(ILogger<DbHookManager<T>> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public void InitializeForContext(DbContext context)
        {
            var hooks = new HookMap();

            if (!_contextHooks.TryAdd(context, hooks))
                throw new InvalidOperationException("Hook Manager has already been initialized with given context");

            var entityTypes = context.DbEntityTypes().ToHashSet();

            AssemblyHookMethods.ForEach(m => HandleMethod(hooks, m, entityTypes));


            _logger.LogDebug($"Registered {typeof(T).Name} for {hooks.Keys.Count} types");
        }

        public async Task ExecuteForEntity(DbContext context, EntityEntry entityEntry)
        {
            if (!_contextHooks.ContainsKey(context))
                throw new InvalidOperationException(
                    "Must initialize HookManager with context before attempted to execute hooks in that context");

            var hooks = _contextHooks[context];

            var entityName = entityEntry.Entity.GetType().Name;

            _logger.LogInformation(
                $"Executing {typeof(T).Name} for Entity Type: {entityName}. Total Hook Types: {hooks.Keys.Count}");
            _logger.LogDebug($"Hooks: {string.Join(";", hooks.SelectMany(h => h.Value).Select(m => m.Name))}");

            var entityType = entityEntry.Entity.GetType();

            if (!hooks.ContainsKey(entityType))
            {
                _logger.LogDebug($"No hooks for {entityName} {typeof(T).Name}");
                return;
            }

            var methods = hooks[entityType];

            var paramsByType = GenerateParameterDictionary(entityEntry.Entity, entityEntry, _serviceProvider, context);

            foreach (var method in methods)
            {
                _logger.LogInformation(
                    $"Executing method {method.Name} for entity type {entityEntry.Entity.GetType()}");
                _logger.LogInformation($"Param types {string.Join(";", paramsByType.Select(p => $"{p.Key.Name}"))}");
                var result = method.InvokeWithParamsOfType(null, paramsByType);
                _logger.LogDebug($"Hook result: {result}");
                if (result != null && result is Task task)
                {
                    await task;
                }
            }
        }

        private static IEnumerable<MethodBase> GetAttributesFromAssembly(Assembly assembly)
        {
            return assembly.GetTypes().SelectMany(t => t.GetMethods())
                .Where(m => m.IsStatic && m.GetCustomAttributes<T>().Any());
        }

        private void RegisterHandler(HookMap hooks, Type entityType, MethodBase method)
        {
            if (!hooks.ContainsKey(entityType)) hooks.Add(entityType, new List<MethodBase>());

            hooks[entityType].Add(method);

            _logger.LogDebug(
                $"Registered {method.Name} to listen for {typeof(T).Name} on {entityType.Name}");
        }

        private void HandleAttr(HookMap hooks, Type classType, MethodBase method, T hookAttr,
            HashSet<Type> allowedTypes)
        {
            var targetEntityTypes = hookAttr.EntityTypes.Count > 0
                ? hookAttr.EntityTypes
                : new List<Type> {classType};

            if (!hookAttr.WatchDescendants && !allowedTypes.Contains(classType))
                throw new InvalidOperationException(
                    $"Cannot watch type {classType.Name} as it is not in on the DbContext");

            targetEntityTypes.ForEach(entityType =>
            {
                if (hookAttr.WatchDescendants)
                    foreach (var watchType in allowedTypes.Where(t => entityType.IsAssignableFrom(t)))
                        RegisterHandler(hooks, watchType, method);
                else
                    RegisterHandler(hooks, entityType, method);
            });
        }

        private void HandleMethod(HookMap hooks, MethodBase method, HashSet<Type> allowedTypes)
        {
            foreach (var attr in method.GetCustomAttributes(typeof(T)))
            {
                var classType = method.DeclaringType;
                HandleAttr(hooks, classType, method, attr as T, allowedTypes);
            }
        }

        private IDictionary<Type, object> GenerateParameterDictionary(params object[] parameters)
        {
            var parameterDict = new Dictionary<Type, object>();

            foreach (var parameter in parameters) parameterDict.TryAdd(parameter.GetType(), parameter);

            return parameterDict;
        }

        private class HookMap : Dictionary<Type, IList<MethodBase>>
        {
        }
    }
}