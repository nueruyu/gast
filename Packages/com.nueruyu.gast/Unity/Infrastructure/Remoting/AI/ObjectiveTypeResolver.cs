using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gast.Application.Reflection;
using Gast.Domain.AI;
using Gast.Domain.AI.Attributes;

namespace Gast.Unity.Infrastructure.Remoting.AI
{
    public class ObjectiveTypeResolver
    {
        readonly Dictionary<string, Type> goalTypeMap = new();

        public ObjectiveTypeResolver(IReflectionAssemblyProvider assemblyProvider)
        {
            CacheGoalTypes(assemblyProvider.GetAssemblies());
        }

        void CacheGoalTypes(IEnumerable<Assembly> assembliesToScan)
        {
            var goalTypes = assembliesToScan
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => typeof(IAIObjective).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in goalTypes)
            {
                var attr = type.GetCustomAttribute<AIObjectiveAttribute>();
                if (attr != null)
                {
                    goalTypeMap[attr.Name] = type;
                }
            }
        }

        public Type Resolve(string objectiveType)
        {
            if (!goalTypeMap.TryGetValue(objectiveType, out var type))
            {
                throw new ArgumentException($"No objective type registered for '{objectiveType}'.");
            }
            return type;
        }
    }
}
