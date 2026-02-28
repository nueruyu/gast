using System.Collections.Generic;
using System.Reflection;
using Gast.Application.Reflection;

namespace Cryst.Infrastructure.Reflection
{
    public class CrystReflectionAssemblyProvider : IReflectionAssemblyProvider
    {
        public IEnumerable<Assembly> GetAssemblies()
        {
            return new[]
            {
                typeof(Gast.Application.AI.ITool).Assembly,         // Gast.Application
                typeof(Gast.Domain.AI.IAIObjective).Assembly,      // Gast.Domain
                typeof(Cryst.Domain.Characters.Faction).Assembly   // Cryst
            };
        }
    }
}