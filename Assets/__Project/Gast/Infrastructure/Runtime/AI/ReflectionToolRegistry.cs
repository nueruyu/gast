using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gast.Application.AI;
using Gast.Application.AI.Attributes;
using Newtonsoft.Json;
using UnityEngine;

namespace Gast.Infrastructure.AI
{
    public class ReflectionToolRegistry : IToolRegistry
    {
        readonly Dictionary<string, (MethodInfo Method, object Target)> tools = new();

        public void RegisterToolSet(object target)
        {
            var methods = target.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<ToolAttribute>();
                if (attr != null)
                {
                    if (tools.ContainsKey(attr.Name))
                    {
                        Debug.LogWarning($"[ReflectionToolRegistry] Duplicate tool name: {attr.Name}");
                        continue;
                    }
                    tools[attr.Name] = (method, target);
                }
            }
        }

        public void UnregisterToolSet(object target)
        {
            var keysToRemove = tools
                .Where(kvp => kvp.Value.Target == target)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                tools.Remove(key);
            }
        }

        public List<ToolDefinition> GetToolDefinitions()
        {
            var definitions = new List<ToolDefinition>();

            foreach (var kvp in tools)
            {
                var name = kvp.Key;
                var method = kvp.Value.Method;
                var attr = method.GetCustomAttribute<ToolAttribute>();

                var parameters = new Dictionary<string, object>();
                var required = new List<string>();

                foreach (var param in method.GetParameters())
                {
                    var paramAttr = param.GetCustomAttribute<ToolParameterAttribute>();
                    var paramName = ToSnakeCase(param.Name);

                    parameters[paramName] = new Dictionary<string, object>
                    {
                        { "type", GetJsonTypeName(param.ParameterType) },
                        { "description", paramAttr?.Description ?? "" }
                    };

                    if (!param.IsOptional)
                    {
                        required.Add(paramName);
                    }
                }

                definitions.Add(new ToolDefinition
                {
                    Name = name,
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

        public async Task<string> ExecuteAsync(string functionName, string argumentsJson)
        {
            if (!tools.TryGetValue(functionName, out var entry))
            {
                throw new ArgumentException($"Tool not found: {functionName}");
            }

            try
            {
                var argsDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(argumentsJson);
                var parameters = entry.Method.GetParameters();
                var invokeArgs = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    var param = parameters[i];
                    var paramName = ToSnakeCase(param.Name);

                    if (argsDict != null && argsDict.TryGetValue(paramName, out var val))
                    {
                        // Convert JSON object to actual parameter type
                        var valJson = JsonConvert.SerializeObject(val);
                        invokeArgs[i] = JsonConvert.DeserializeObject(valJson, param.ParameterType);
                    }
                    else if (param.HasDefaultValue)
                    {
                        invokeArgs[i] = param.DefaultValue;
                    }
                    else
                    {
                        throw new ArgumentException($"Missing required argument: {paramName}");
                    }
                }

                var result = entry.Method.Invoke(entry.Target, invokeArgs);

                if (result is Task task)
                {
                    await task.ConfigureAwait(false);
                    var resultProperty = task.GetType().GetProperty("Result");
                    result = resultProperty?.GetValue(task);
                }

                // TODO: convert to snake_case
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ReflectionToolRegistry] Error executing {functionName}: {ex}");
                return JsonConvert.SerializeObject(new { error = ex.Message });
            }
        }

        string ToSnakeCase(string text)
        {
            return Regex.Replace(text, "([a-z0-9])([A-Z])", "$1_$2").ToLower();
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