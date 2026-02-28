using System.Collections.Generic;
using System.Reflection;

namespace Gast.Application.Reflection
{
    /// <summary>
    /// Provides a list of assemblies to be scanned for reflection-based features
    /// such as commands, objectives, and tools.
    /// </summary>
    public interface IReflectionAssemblyProvider
    {
        /// <summary>
        /// Gets the list of assemblies to scan.
        /// </summary>
        /// <returns>A collection of assemblies.</returns>
        IEnumerable<Assembly> GetAssemblies();
    }
}