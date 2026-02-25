using System;

namespace Gast.Domain.Characters
{
    /// <summary>
    /// A registry to find the appropriate character factory based on parameter type.
    /// </summary>
    public interface ICharacterFactoryRegistry
    {
        /// <summary>
        /// Gets a character factory that can handle the specified parameter type.
        /// </summary>
        /// <param name="parametersType">The type of the creation parameters.</param>
        /// <returns>The corresponding character factory.</returns>
        ICharacterFactory Get(Type parametersType);
    }
}
