using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gast.Application.AI;
using Gast.Application.AI.Attributes;
using UnityEngine;

namespace Gast.Infrastructure.AI
{
    public class ReflectionToolRegistry : IToolRegistry
    {
        readonly Dictionary<string, ITool> tools = new();

        public ReflectionToolRegistry(IEnumerable<IToolSet> toolSets)
        {
            foreach (var toolSet in toolSets)
            {
                RegisterToolSet(toolSet);
            }
        }

        void RegisterToolSet(object target)
        {
            var methods = target.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<ToolAttribute>();
                if (attr == null)
                    continue;

                if (tools.ContainsKey(attr.Name))
                {
                    Debug.LogWarning($"[ReflectionToolRegistry] Duplicate tool name: {attr.Name}");
                    continue;
                }

                var definition = CreateToolDefinition(attr, method);
                var tool = new ReflectionTool(method, target, definition);
                tools[attr.Name] = tool;
            }
        }

        public List<ToolDefinition> GetToolDefinitions()
        {
            return tools.Values.Select(t => t.Definition).ToList();
        }

        public ITool GetTool(string toolName)
        {
            if (tools.TryGetValue(toolName, out var tool))
            {
                return tool;
            }
            throw new KeyNotFoundException($"Tool with name '{toolName}' not found.");
        }

        ToolDefinition CreateToolDefinition(ToolAttribute attr, MethodInfo method)
        {
            var parameters = new Dictionary<string, object>();
            var required = new List<string>();

            foreach (var param in method.GetParameters())
            {
                var paramAttr = param.GetCustomAttribute<ToolParameterAttribute>();
                var paramName = JsonSchemaHelper.ToSnakeCase(param.Name);

                parameters[paramName] = new Dictionary<string, object>
                {
                    { "type", JsonSchemaHelper.GetJsonTypeName(param.ParameterType) },
                    { "description", paramAttr?.Description ?? "" }
                };

                if (!param.IsOptional)
                {
                    required.Add(paramName);
                }
            }

            return new ToolDefinition
            {
                Name = attr.Name,
                Description = attr.Description,
                Parameters = new Dictionary<string, object>
                {
                    { "type", "object" },
                    { "properties", parameters },
                    { "required", required }
                }
            };
        }
    }
}