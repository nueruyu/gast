using System;
using System.IO;
using Newtonsoft.Json;

namespace Gast.Unity.Infrastructure.Stories.Converters
{
    /// <summary>
    /// Stores any JSON value (object, array, primitive) as a raw JSON string.
    /// Use as a [JsonConverter] attribute to defer typed deserialization.
    /// </summary>
    public class RawJsonStringConverter : JsonConverter<string>
    {
        public override string ReadJson(
            JsonReader reader, Type objectType,
            string existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);
            jw.WriteToken(reader);
            return sw.ToString();
        }

        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteRawValue(value);
        }
    }
}
