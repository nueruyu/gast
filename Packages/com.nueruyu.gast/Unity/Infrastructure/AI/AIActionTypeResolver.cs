using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gast.Application.Reflection;
using Gast.Domain.AI.Attributes;
using Gast.Lib.AI;
using Gast.Unity.Features.Stories;

namespace Gast.Unity.Infrastructure.AI
{
    public class AIActionTypeResolver
    {
        readonly Dictionary<string, Type> actionTypeMap = new();

        public AIActionTypeResolver(IReflectionAssemblyProvider assemblyProvider)
        {
            CacheActionTypes(assemblyProvider.GetAssemblies());
        }

        void CacheActionTypes(IEnumerable<Assembly> assembliesToScan)
        {
            var actionTypes = assembliesToScan
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => typeof(IAction<StoryActorContext, StoryWorldState>).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in actionTypes)
            {
                var attr = type.GetCustomAttribute<AIActionAttribute>();
                if (attr != null)
                {
                    actionTypeMap[attr.Name] = type;
                }
            }
        }

        public Type Resolve(string actionName)
        {
            if (!actionTypeMap.TryGetValue(actionName, out var type))
            {
                throw new ArgumentException($"No story action type registered for '{actionName}'.");
            }
            return type;
        }
    }
}
