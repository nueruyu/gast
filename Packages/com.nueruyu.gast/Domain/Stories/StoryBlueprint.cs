using System.Collections.Generic;

namespace Gast.Domain.Stories
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
        public IReadOnlyList<BlueprintTaskRef> Tasks { get; }

        public BlueprintMethod(string name, IReadOnlyList<BlueprintTaskRef> tasks)
        {
            Name = name;
            Tasks = tasks;
        }
    }

    /// <summary>
    /// A single entry in a method's task list: either a compound-task reference or an inline primitive action.
    /// </summary>
    public class BlueprintTaskRef
    {
        public string CompoundTaskName { get; }
        public string ActionName { get; }
        /// <summary>Raw JSON string of action parameters. Null for compound references.</summary>
        public string ParametersJson { get; }
        public bool IsCompound => CompoundTaskName != null;

        BlueprintTaskRef(string compoundTaskName, string actionName, string parametersJson)
        {
            CompoundTaskName = compoundTaskName;
            ActionName = actionName;
            ParametersJson = parametersJson;
        }

        public static BlueprintTaskRef ForCompound(string taskName)
            => new(taskName, null, null);

        public static BlueprintTaskRef ForPrimitive(string actionName, string parametersJson)
            => new(null, actionName, parametersJson);
    }
}
