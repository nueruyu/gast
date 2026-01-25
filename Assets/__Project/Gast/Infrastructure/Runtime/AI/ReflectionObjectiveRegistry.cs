using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Gast.Application.AI;
using Gast.Domain.AI;
using Gast.Domain.AI.Attributes;

namespace Gast.Infrastructure.AI
{
    public class ReflectionObjectiveRegistry : IObjectiveRegistry
    {
        public List<ObjectiveDefinition> GetObjectiveDefinitions()
        {
            var definitions = new List<ObjectiveDefinition>();

            var assembly = typeof(IAIObjective).Assembly;
            var objectiveTypes = assembly.GetTypes()
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

                    var paramName = ToSnakeCase(prop.Name);
                    paramName = NormalizeParamName(paramName);

                    parameters[paramName] = new Dictionary<string, object>
                    {
                        { "type", paramAttr.TypeName ?? GetJsonTypeName(prop.PropertyType) },
                        { "description", paramAttr.Description }
                    };

                    required.Add(paramName);
                }

                definitions.Add(new ObjectiveDefinition
                {
                    Name = attr.Name,
                    Description = attr.Description,
                    Parameters = new Dictionary<string, object>
                    {
                        { "type", "object" },
                        { "properties", parameters },
                        { "required", required }
                    }
                });
            }

            return definitions;
        }

        string ToSnakeCase(string text)
        {
            return Regex.Replace(text, "([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

        string NormalizeParamName(string name)
        {
            return name.Replace("target_", "");
        }

        string GetJsonTypeName(Type type)
        {
            if (type == typeof(int) || type == typeof(float) || type == typeof(double)) return "number";
            if (type == typeof(bool)) return "boolean";
            if (type == typeof(string)) return "string";

            throw new NotSupportedException($"Type '{type}' is not supported");
        }
    }
}