using System;
using System.Collections.Generic;

namespace Disunity.EntityFrameworkCore.Hooks {

    [AttributeUsage(AttributeTargets.Method)]
    public abstract class DbHookAttribute : Attribute {

        protected DbHookAttribute(params Type[] entityTypes) {
            EntityTypes = new List<Type>(entityTypes);
        }

        public List<Type> EntityTypes { get; }

    }

}