using System.Collections.Generic;

namespace Gast.Lib.Gaia
{
    public class StoryDefinition
    {
        public string DomainName { get; set; }
        public string RootTask { get; set; }
        public List<TaskDefinition> Tasks { get; set; }
    }

    public class TaskDefinition
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Selector { get; set; }
        public List<MethodDefinition> Methods { get; set; }
    }

    public class MethodDefinition
    {
        public string Name { get; set; }

        /// <summary>
        /// Each element is either a compound-task reference (by name) or an inline primitive task.
        /// Deserialization of the string/object polymorphism is handled by TaskReferenceJsonConverter.
        /// </summary>
        public List<TaskReference> Tasks { get; set; }
    }

    /// <summary>
    /// Represents one entry in a method's task list.
    /// Exactly one of <see cref="CompoundTaskName"/> or <see cref="Action"/> is set.
    /// </summary>
    public class TaskReference
    {
        /// <summary>Name of a compound task to call (when the JSON value is a plain string).</summary>
        public string CompoundTaskName { get; set; }

        /// <summary>Action name of an inline primitive task (when the JSON value is an object).</summary>
        public string Action { get; set; }

        /// <summary>Raw JSON string of the action parameters object. Null when this is a compound reference.</summary>
        public string ParametersJson { get; set; }

        public bool IsCompound => CompoundTaskName != null;
    }
}
