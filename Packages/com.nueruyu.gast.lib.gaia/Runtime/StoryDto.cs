using System.Collections.Generic;

namespace Gast.Lib.Gaia
{
    // --- Story Generation ---
    public class CreateStoryRequest
    {
        public string Instruction { get; set; }
        public List<CharacterContextDto> AvailableCharacters { get; set; }
    }

    public class CharacterContextDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Faction { get; set; }
    }

    public class StoryResponseDto
    {
        public StoryDefinitionDto Story { get; set; }
    }

    public class StoryDefinitionDto
    {
        public string RootTask { get; set; }
        public List<TaskDefinitionDto> Tasks { get; set; }
    }

    public enum TaskType
    {
        Compound,
        Primitive
    }

    public class TaskDefinitionDto
    {
        public string Name { get; set; }
        public TaskType Type { get; set; }

        [JsonOptional]
        public string Selector { get; set; }

        [JsonOptional]
        public List<MethodDefinitionDto> Methods { get; set; }
    }

    public class MethodDefinitionDto
    {
        public string Name { get; set; }

        /// <summary>
        ///     Each element is either a compound-task reference (by name) or an inline primitive task.
        ///     Deserialization of the string/object polymorphism is handled by TaskReferenceDtoJsonConverter.
        /// </summary>
        public List<TaskReferenceDto> Tasks { get; set; }
    }

    /// <summary>
    ///     Represents one entry in a method's task list.
    ///     Exactly one of <see cref="CompoundTaskName" /> or <see cref="Action" /> is set.
    ///     Deserialized via <see cref="TaskReferenceDtoJsonConverter" />, so attributes do not apply.
    /// </summary>
    public class TaskReferenceDto
    {
        /// <summary>Name of a compound task to call (when the JSON value is a plain string).</summary>
        [JsonOptional]
        public string CompoundTaskName { get; set; }

        /// <summary>Action name of an inline primitive task (when the JSON value is an object).</summary>
        [JsonOptional]
        public string Action { get; set; }

        /// <summary>Parameters of an inline primitive task. Null when this is a compound reference.</summary>
        [JsonOptional]
        public Dictionary<string, object> Parameters { get; set; }

        public bool IsCompound => CompoundTaskName != null;
    }
}