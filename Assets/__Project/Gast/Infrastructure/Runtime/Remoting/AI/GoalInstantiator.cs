using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Gast.Domain.AI;
using Gast.Domain.AI.Attributes;
using Gast.Infrastructure.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gast.Infrastructure.Remoting.AI
{
    public class GoalInstantiator
    {
        readonly Dictionary<string, Type> goalTypeMap = new();
        readonly JsonSerializer jsonSerializer;

        public GoalInstantiator(IReflectionAssemblyProvider assemblyProvider)
        {
            CacheGoalTypes(assemblyProvider.GetAssemblies());

            jsonSerializer = new JsonSerializer();
            jsonSerializer.Converters.Add(new DomainValueObjectConverter());
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

        public IAIObjective CreateGoal(string objectiveType, Dictionary<string, object> parameters)
        {
            if (!goalTypeMap.TryGetValue(objectiveType, out var type))
            {
                throw new ArgumentException($"No goal type registered for objective '{objectiveType}'.");
            }

            // Find the most suitable constructor (e.g., the one with the most parameters)
            var constructor = type.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();

            if (constructor == null)
            {
                throw new InvalidOperationException($"No public constructor found for goal type '{type.Name}'.");
            }

            var constructorParams = constructor.GetParameters();
            var args = new object[constructorParams.Length];
            var jObject = JObject.FromObject(parameters);

            for (int i = 0; i < constructorParams.Length; i++)
            {
                var param = constructorParams[i];
                // Convert C# parameter name (camelCase) to snake_case for dictionary lookup
                var snakeName = ToSnakeCase(param.Name);

                if (jObject.TryGetValue(snakeName, StringComparison.OrdinalIgnoreCase, out var token))
                {
                    args[i] = token.ToObject(param.ParameterType, jsonSerializer);
                }
                else
                {
                    throw new ArgumentException($"Missing parameter '{snakeName}' for goal '{objectiveType}'.");
                }
            }

            return (IAIObjective)constructor.Invoke(args);
        }

        string ToSnakeCase(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return Regex.Replace(text, "(?<!^)([A-Z])", "_$1").ToLower();
        }
    }
}