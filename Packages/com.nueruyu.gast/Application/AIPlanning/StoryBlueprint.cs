using System.Collections.Generic;

namespace Gast.Application.AIPlanning
{
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

    /// <summary>
    ///     Represents a compound task definition with its methods.
    /// </summary>
    public class BlueprintTask
    {
        public BlueprintTask(string name, string selector, IReadOnlyList<BlueprintMethod> methods)
        {
            Name = name;
            Selector = selector;
            Methods = methods;
        }

        public string Name { get; }
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