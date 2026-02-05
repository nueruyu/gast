using System.Collections.Generic;

namespace Gast.Application.AI
{
    public class ObjectiveDefinition
    {
        public ObjectiveDefinition(string name, string description, IReadOnlyDictionary<string, object> parameters)
        {
            Name = name;
            Description = description;
            Parameters = parameters;
        }

        public string Name { get; }
        public string Description { get; }
        public IReadOnlyDictionary<string, object> Parameters { get; }
    }
}