using Gast.Application.AIPlanning;

namespace Gast.Unity.Features.Stories
{
    public class CompoundTaskRef : IBlueprintTaskRef
    {
        public string CompoundTaskName { get; }

        public CompoundTaskRef(string compoundTaskName)
        {
            CompoundTaskName = compoundTaskName;
        }
    }
}
