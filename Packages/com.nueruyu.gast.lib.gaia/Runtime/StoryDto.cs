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
        public string DomainName { get; set; }
        public string RootTask { get; set; }
        public List<TaskDefinitionDto> Tasks { get; set; }
    }

    public class TaskDefinitionDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Selector { get; set; }
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
    /// </summary>
    public class TaskReferenceDto
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