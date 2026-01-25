using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Gast.Application.AI;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Gast.Infrastructure.AI
{
    public class ReflectionTool : ITool
    {
        readonly MethodInfo method;
        readonly object target;
        readonly JsonSerializerSettings jsonSerializerSettings;

        public ToolDefinition Definition { get; }

        public ReflectionTool(MethodInfo method, object target, ToolDefinition definition)
        {
            this.method = method;
            this.target = target;
            Definition = definition;
            jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
        }

        public async Task<string> ExecuteAsync(string argumentsJson)
        {
            try
            {
                var argsDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(argumentsJson);
                var parameters = method.GetParameters();
                var invokeArgs = new object[parameters.Length];

                for (var i = 0; i < parameters.Length; i++)
                {
                    var param = parameters[i];
                    var paramName = JsonSchemaHelper.ToSnakeCase(param.Name);

                    if (argsDict != null && argsDict.TryGetValue(paramName, out var val))
                    {
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

                var result = method.Invoke(target, invokeArgs);

                if (result is Task task)
                {
                    await task.ConfigureAwait(false);
                    var resultProperty = task.GetType().GetProperty("Result");
                    result = resultProperty?.GetValue(task);
                }

                return JsonConvert.SerializeObject(result, jsonSerializerSettings);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ReflectionTool] Error executing {Definition.Name}: {ex}");
                return JsonConvert.SerializeObject(new { error = ex.Message });
            }
        }
    }
}