using Gast.Domain.Characters;
using System.Collections.Generic;

namespace Gast.Domain.AI
{
    /// <summary>
    /// Interface for brain components that can receive and report objective assignments.
    /// </summary>
    public interface ICharacterAIBrain : ICharacterBrain
    {
        void SetObjectives(IEnumerable<IAIObjective> objectives);

        IReadOnlyList<IAIObjective> CurrentObjectives { get; }
    }
}