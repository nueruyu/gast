using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gast.Application.Reflection;
using Gast.Core.Commands;
using Newtonsoft.Json;

namespace Gast.Unity.Infrastructure.Services
{
    /// <summary>
    /// An implementation of ICommandSerializer that uses Newtonsoft.Json.
    /// It caches command types on startup for efficient deserialization using simple type names.
    /// </summary>
    public class JsonCommandSerializer : ICommandSerializer
    {
        readonly Dictionary<string, Type> commandTypeCache = new();

        readonly JsonSerializerSettings settings = new()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        public JsonCommandSerializer(IReflectionAssemblyProvider assemblyProvider)
        {
            CacheCommandTypes(assemblyProvider.GetAssemblies());
        }

        /// <summary>
        /// Cache types implementing ICommand or ICommand<T>
        /// </summary>
        void CacheCommandTypes(IEnumerable<Assembly> targetAssemblies)
        {
            foreach (var assembly in targetAssemblies)
            {
                var types = assembly.GetTypes().Where(IsCommandType);

                foreach (var type in types)
                {
                    if (commandTypeCache.ContainsKey(type.Name))
                        continue;

                    commandTypeCache.Add(type.Name, type);
                }
            }
        }

        public string Serialize(object command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var commandType = command.GetType();
            if (!IsCommandType(commandType))
            {
                throw new ArgumentException("Object to serialize is not a valid command.", nameof(command));
            }

            var payload = JsonConvert.SerializeObject(command, commandType, settings);
            var transport = new CommandTransport(commandType.Name, payload);

            return JsonConvert.SerializeObject(transport, settings);
        }

        public object Deserialize(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data));

            var transport = JsonConvert.DeserializeObject<CommandTransport>(data, settings);
            if (transport == null || string.IsNullOrEmpty(transport.CommandTypeName))
            {
                throw new JsonSerializationException("Invalid command transport format.");
            }

            if (!commandTypeCache.TryGetValue(transport.CommandTypeName, out var commandType))
            {
                throw new JsonSerializationException($"Could not find command type '{transport.CommandTypeName}'.");
            }

            return JsonConvert.DeserializeObject(transport.CommandPayload, commandType, settings);
        }

        static bool IsCommandType(Type type)
        {
            if (!type.IsValueType || type.IsAbstract)
                return false;

            var interfaces = type.GetInterfaces();
            if (interfaces.Contains(typeof(ICommand)) || interfaces.Contains(typeof(IAsyncCommand)))
                return true;

            return interfaces.Any(i =>
                i.IsGenericType &&
                (i.GetGenericTypeDefinition() == typeof(ICommand<>) ||
                 i.GetGenericTypeDefinition() == typeof(IAsyncCommand<>))
            );
        }
    }
}