using System;

namespace EFCoreHooks.Attributes
{
    public class OnAfterSave : DbHookAttribute
    {
        /// <summary>
        ///     Register a callback to be called after a new or existing model of any type in <see cref="entityTypes" /> is saved
        ///     to the database
        /// </summary>
        /// <remarks>
        ///     See <see cref="OnBeforeCreate" /> for more details
        /// </remarks>
        /// <param name="entityTypes">
        ///     The entity types that this hook will respond to.
        ///     If left empty, the type of the model class will be assumed.
        /// </param>
        public OnAfterSave(params Type[] entityTypes) : base(entityTypes)
        {
        }
    }
}