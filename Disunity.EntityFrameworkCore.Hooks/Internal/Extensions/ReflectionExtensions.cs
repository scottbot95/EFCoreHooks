using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Internal;

namespace Disunity.EntityFrameworkCore.Hooks.Internal.Extensions
{
    internal static class ReflectionExtensions
    {
        internal static object InvokeWithParamsOfType(this MethodBase self, object obj,
            IDictionary<Type, object> parameters)
        {
            return self.Invoke(obj, MapParameters(self, parameters));
        }

        private static object[] MapParameters(MethodBase method, IDictionary<Type, object> parametersByType)
        {
            var paramTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
            var parameters = new object[paramTypes.Length];

            for (var i = 0; i < parameters.Length; ++i)
            {
                parameters[i] = Type.Missing;
            }

            foreach (var (paramType, value) in parametersByType)
            {
                try
                {
                    var paramIndex = paramTypes.ToList().FindIndex(t => t.IsAssignableFrom(paramType));
                    parameters[paramIndex] = value;
                }
                catch
                {
                    // ignored
                }
            }

            return parameters;
        }
    }
}