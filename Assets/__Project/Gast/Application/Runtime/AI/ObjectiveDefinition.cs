using System.Collections.Generic;

namespace Gast.Application.AI
{
    public class ObjectiveDefinition
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}