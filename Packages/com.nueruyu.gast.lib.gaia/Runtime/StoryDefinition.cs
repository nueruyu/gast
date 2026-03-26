using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gast.Lib.Gaia
{
    public class StoryDefinition
    {
        [JsonProperty("domain_name")]
        public string DomainName { get; set; }

        [JsonProperty("root_task")]
        public string RootTask { get; set; }

        [JsonProperty("tasks")]
        public List<TaskDefinition> Tasks { get; set; }
    }

    public class TaskDefinition
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("selector")]
        public string Selector { get; set; }

        [JsonProperty("methods")]
        public List<MethodDefinition> Methods { get; set; }
    }

    public class MethodDefinition
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Each element is either a string (reference to a compound task by name)
        /// or a JObject (inline primitive task definition with "action" and "parameters").
        /// JSON parsing is intentionally kept within this DTO layer.
        /// </summary>
        [JsonProperty("tasks")]
        public List<JToken> TaskReferences { get; set; }
    }

    public class PrimitiveTaskDefinition
    {
        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("parameters")]
        public JObject Parameters { get; set; }
    }
}
