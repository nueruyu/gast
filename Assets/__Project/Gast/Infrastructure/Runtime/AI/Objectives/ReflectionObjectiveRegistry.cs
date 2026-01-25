using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Gast.Application.AI.Models;
using Gast.Application.AI.Objectives;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Economy;

namespace Gast.Infrastructure.AI.Objectives
{
    public class ReflectionObjectiveRegistry : IObjectiveRegistry
    {
        public List<ObjectiveDefinition> GetObjectiveDefinitions()
        {
            var definitions = new List<ObjectiveDefinition>();

            // Scan all types in the Domain Assembly that implement IGoal
            var assembly = typeof(IGoal).Assembly;
            var goalTypes = assembly.GetTypes()
                .Where(t => typeof(IGoal).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in goalTypes)
            {
                var attr = type.GetCustomAttribute<AIObjectiveAttribute>();
                if (attr == null) continue;

                var parameters = new Dictionary<string, object>();
                var required = new List<string>();

                // Scan properties with AIObjectiveParameterAttribute
                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var paramAttr = prop.GetCustomAttribute<AIObjectiveParameterAttribute>();
                    if (paramAttr == null) continue;

                    var snakeName = ToSnakeCase(prop.Name);
                    snakeName = NormalizeParamName(snakeName);

                    parameters[snakeName] = new Dictionary<string, object>
                    {
                        { "type", GetJsonTypeName(prop.PropertyType) },
                        { "description", paramAttr.Description }
                    };

                    required.Add(snakeName);
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

            // Value Objects treated as strings
            if (type == typeof(ItemId) || type == typeof(CharacterTypeId) || type == typeof(Guid)) return "string";

            return "string";
        }
    }
}
