using System;
using ApplicationException = Gast.Core.Exceptions.ApplicationException;

namespace Gast.Unity.Shared
{
    public class ComponentNotFoundException : ApplicationException
    {
        public ComponentNotFoundException(Type componentType, Type ownerType = null)
            : base(ownerType == null
                ? $"Component of type '{componentType.Name}' not found."
                : $"Component of type '{componentType.Name}' not found on object of type '{ownerType.Name}'.")
        {
        }
    }
}