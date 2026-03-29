using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gast.Lib.Gaia
{
    /// <summary>
    /// Deserializes a task reference object.
    /// Compound: <c>{ "name": "task_name" }</c>
    /// Primitive: <c>{ "action": "action_name", "parameters": { ... } }</c>
    /// </summary>
    public class TaskReferenceDtoJsonConverter : JsonConverter<TaskReferenceDto>
    {
        public override TaskReferenceDto ReadJson(
            JsonReader reader, Type objectType,
            TaskReferenceDto existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            string name = null;
            string action = null;
            Dictionary<string, object> parameters = null;

            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType != JsonToken.PropertyName) continue;

                var propName = (string)reader.Value;
                reader.Read();

                switch (propName)
                {
                    case "name":
                        name = (string)reader.Value;
                        break;
                    case "action":
                        action = (string)reader.Value;
                        break;
                    case "parameters":
                        parameters = serializer.Deserialize<Dictionary<string, object>>(reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            if (name != null)
                return new TaskReferenceDto { CompoundTaskName = name };

            if (action != null)
                return new TaskReferenceDto { Action = action, Parameters = parameters };

            throw new JsonSerializationException(
                "Task reference must have either a 'name' or 'action' field.");
        }

        public override void WriteJson(JsonWriter writer, TaskReferenceDto value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            if (value.IsCompound)
            {
                writer.WritePropertyName("name");
                writer.WriteValue(value.CompoundTaskName);
            }
            else
            {
                writer.WritePropertyName("action");
                writer.WriteValue(value.Action);
                if (value.Parameters != null)
                {
                    writer.WritePropertyName("parameters");
                    serializer.Serialize(writer, value.Parameters);
                }
            }

            writer.WriteEndObject();
        }
    }
}
