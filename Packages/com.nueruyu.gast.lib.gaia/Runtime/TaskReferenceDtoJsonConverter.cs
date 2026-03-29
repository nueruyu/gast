using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gast.Lib.Gaia
{
    /// <summary>
    /// Deserializes a task reference that is either a plain string (compound task name)
    /// or a JSON object with "action" and optional "parameters" fields (inline primitive task).
    /// </summary>
    public class TaskReferenceDtoJsonConverter : JsonConverter<TaskReferenceDto>
    {
        public override TaskReferenceDto ReadJson(
            JsonReader reader, Type objectType,
            TaskReferenceDto existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
                return new TaskReferenceDto { CompoundTaskName = (string)reader.Value };

            // Object form: { "action": "...", "parameters": { ... } }
            string action = null;
            Dictionary<string, object> parameters = null;

            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType != JsonToken.PropertyName) continue;

                var propName = (string)reader.Value;
                reader.Read();

                switch (propName)
                {
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

            return new TaskReferenceDto { Action = action, Parameters = parameters };
        }

        public override void WriteJson(JsonWriter writer, TaskReferenceDto value, JsonSerializer serializer)
        {
            if (value.IsCompound)
            {
                writer.WriteValue(value.CompoundTaskName);
                return;
            }

            writer.WriteStartObject();
            writer.WritePropertyName("action");
            writer.WriteValue(value.Action);
            if (value.Parameters != null)
            {
                writer.WritePropertyName("parameters");
                serializer.Serialize(writer, value.Parameters);
            }
            writer.WriteEndObject();
        }
    }
}
