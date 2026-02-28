using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gast.Application.AI;
using Gast.Application.Reflection;
using Gast.Domain.AI;
using Gast.Domain.AI.Attributes;

namespace Gast.Unity.Infrastructure.AI
{
    public class ReflectionObjectiveRegistry : IObjectiveRegistry
    {
        readonly List<ObjectiveDefinition> objectiveDefinitions;

        public ReflectionObjectiveRegistry(IReflectionAssemblyProvider assemblyProvider)
        {
            objectiveDefinitions = GatherObjectiveDefinitions(assemblyProvider.GetAssemblies());
        }

        public List<ObjectiveDefinition> GetObjectiveDefinitions()
        {
            return objectiveDefinitions;
        }

        static List<ObjectiveDefinition> GatherObjectiveDefinitions(IEnumerable<Assembly> assembliesToScan)
        {
            var definitions = new List<ObjectiveDefinition>();
            var objectiveTypes = assembliesToScan
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => typeof(IAIObjective).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in objectiveTypes)
            {
                var attr = type.GetCustomAttribute<AIObjectiveAttribute>();
                if (attr == null) continue;

                var parameters = new Dictionary<string, object>();
                var required = new List<string>();

                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var paramAttr = prop.GetCustomAttribute<AIObjectiveParameterAttribute>();
                    if (paramAttr == null) continue;

                    var paramName = JsonSchemaHelper.ToSnakeCase(prop.Name);

                    parameters[paramName] = new Dictionary<string, object>
                    {
                        { "type", paramAttr.TypeName ?? JsonSchemaHelper.GetJsonTypeName(prop.PropertyType) },
                        { "description", paramAttr.Description }
                    };
                    required.Add(paramName);
                }

                definitions.Add(new ObjectiveDefinition(
                    attr.Name,
                    attr.Description,
                    new Dictionary<string, object>
                    {
                        { "type", "object" },
                        { "properties", parameters },
                        { "required", required }
                    }));
            }

            return definitions;
        }
    }
}