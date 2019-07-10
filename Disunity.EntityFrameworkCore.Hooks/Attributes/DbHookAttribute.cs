using System;
using System.Collections.Generic;

namespace Disunity.EntityFrameworkCore.Hooks.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class DbHookAttribute : Attribute
    {
        public List<Type> EntityTypes { get; }

        public bool WatchDescendants { get; set; }

        protected DbHookAttribute(params Type[] entityTypes)
        {
            EntityTypes = new List<Type>(entityTypes);
            WatchDescendants = true;
        }
    }
}