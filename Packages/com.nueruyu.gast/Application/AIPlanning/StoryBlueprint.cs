using System.Collections.Generic;

namespace Gast.Application.AIPlanning
{
    public enum BlueprintTaskType
    {
        Compound,
        Primitive
    }

    /// <summary>
    ///     Domain model representing a parsed story definition.
    ///     Produced by <c>IStoryGenerationService</c> and consumed by <c>IStoryRunner</c>.
    /// </summary>
    public class StoryBlueprint
    {
        public StoryBlueprint(string rootTask, IReadOnlyList<BlueprintTask> tasks)
        {
            RootTask = rootTask;
            Tasks = tasks;
        }

        public string RootTask { get; }
        public IReadOnlyList<BlueprintTask> Tasks { get; }
    }

    public class BlueprintTask
    {
        public BlueprintTask(string name, BlueprintTaskType type, string selector, IReadOnlyList<BlueprintMethod> methods)
        {
            Name = name;
            Type = type;
            Selector = selector;
            Methods = methods;
        }

        public string Name { get; }
        public BlueprintTaskType Type { get; }
        public string Selector { get; }
        public IReadOnlyList<BlueprintMethod> Methods { get; }
    }

    public class BlueprintMethod
    {
        public BlueprintMethod(string name, IReadOnlyList<IBlueprintTaskRef> tasks)
        {
            Name = name;
            Tasks = tasks;
        }

        public string Name { get; }
        public IReadOnlyList<IBlueprintTaskRef> Tasks { get; }
    }
}