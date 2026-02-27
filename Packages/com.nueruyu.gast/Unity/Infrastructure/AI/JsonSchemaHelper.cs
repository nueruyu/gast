using System;
using System.Text.RegularExpressions;

namespace Gast.Unity.Infrastructure.AI
{
    public static class JsonSchemaHelper
    {
        public static string ToSnakeCase(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            return Regex.Replace(text, "([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

        public static string GetJsonTypeName(Type type)
        {
            if (type == typeof(int) || type == typeof(float) || type == typeof(double)) return "number";
            if (type == typeof(bool)) return "boolean";
            if (type == typeof(string)) return "string";

            throw new NotSupportedException($"Type '{type}' is not supported for JSON schema conversion");
        }
    }
}