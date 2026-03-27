using System;
using System.IO;
using Gast.Lib.Gaia;
using Newtonsoft.Json;

namespace Cryst.Features.Stories.Converters
{
    /// <summary>
    /// Deserializes a task reference that is either a plain string (compound task name)
    /// or a JSON object with "action" and optional "parameters" fields (inline primitive task).
    /// </summary>
    public class TaskReferenceJsonConverter : JsonConverter<TaskReference>
    {
        public override TaskReference ReadJson(
            JsonReader reader, Type objectType,
            TaskReference existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
                return new TaskReference { CompoundTaskName = (string)reader.Value };

            // Object form: { "action": "...", "parameters": { ... } }
            string action = null;
            string parametersJson = null;

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
                        parametersJson = CaptureRawJson(reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            return new TaskReference { Action = action, ParametersJson = parametersJson };
        }

        public override void WriteJson(JsonWriter writer, TaskReference value, JsonSerializer serializer)
        {
            if (value.IsCompound)
            {
                writer.WriteValue(value.CompoundTaskName);
                return;
            }

            writer.WriteStartObject();
            writer.WritePropertyName("action");
            writer.WriteValue(value.Action);
            if (value.ParametersJson != null)
            {
                writer.WritePropertyName("parameters");
                writer.WriteRawValue(value.ParametersJson);
            }
            writer.WriteEndObject();
        }

        static string CaptureRawJson(JsonReader reader)
        {
            var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);
            jw.WriteToken(reader);
            return sw.ToString();
        }
    }
}
