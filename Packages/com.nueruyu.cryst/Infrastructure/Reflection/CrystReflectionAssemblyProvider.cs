using System.Collections.Generic;
using System.Reflection;
using Cryst.Domain.Characters;
using Gast.Application.AI;
using Gast.Application.Reflection;
using Gast.Domain.AI;
using Gast.Unity.Features.Characters;

namespace Cryst.Infrastructure.Reflection
{
    public class CrystReflectionAssemblyProvider : IReflectionAssemblyProvider
    {
        public IEnumerable<Assembly> GetAssemblies()
        {
            return new[]
            {
                typeof(ITool).Assembly, // Gast.Application
                typeof(IAIObjective).Assembly, // Gast.Domain
                typeof(Character).Assembly, // Gast.Unity.Features
                typeof(Faction).Assembly // Cryst
            };
        }
    }
}