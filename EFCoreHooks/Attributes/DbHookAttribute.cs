using System;
using System.Collections.Generic;

namespace EFCoreHooks.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class DbHookAttribute : Attribute
    {
        protected DbHookAttribute(params Type[] entityTypes)
        {
            EntityTypes = new List<Type>(entityTypes);
            WatchDescendants = true;
        }

        public List<Type> EntityTypes { get; }

        public bool WatchDescendants { get; set; }
    }
}