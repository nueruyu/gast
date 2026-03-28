using System.Collections.Generic;

namespace Gast.Application.AIPlanning
{
    /// <summary>
    /// Domain model representing a parsed story definition.
    /// Produced by <c>IStoryGenerationService</c> and consumed by <c>IStoryRunner</c>.
    /// </summary>
    public class StoryBlueprint
    {
        public string DomainName { get; }
        public string RootTask { get; }
        public IReadOnlyList<BlueprintTask> Tasks { get; }

        public StoryBlueprint(string domainName, string rootTask, IReadOnlyList<BlueprintTask> tasks)
        {
            DomainName = domainName;
            RootTask = rootTask;
            Tasks = tasks;
        }
    }

    public class BlueprintTask
    {
        public string Name { get; }
        public string Type { get; }
        public string Selector { get; }
        public IReadOnlyList<BlueprintMethod> Methods { get; }

        public BlueprintTask(string name, string type, string selector, IReadOnlyList<BlueprintMethod> methods)
        {
            Name = name;
            Type = type;
            Selector = selector;
            Methods = methods;
        }
    }

    public class BlueprintMethod
    {
        public string Name { get; }
        public IReadOnlyList<IBlueprintTaskRef> Tasks { get; }

        public BlueprintMethod(string name, IReadOnlyList<IBlueprintTaskRef> tasks)
        {
            Name = name;
            Tasks = tasks;
        }
    }
}
