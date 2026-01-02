using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationException = DescrioGames.Core.Exceptions.ApplicationException;

namespace DescrioGames.Shared.Exceptions
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