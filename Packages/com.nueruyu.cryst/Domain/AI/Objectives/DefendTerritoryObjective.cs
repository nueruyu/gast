using Gast.Core.Observables;
using Gast.Domain.AI;
using Gast.Domain.Characters;

namespace Cryst.Domain.AI.Objectives
{
    /// <summary>
    /// An ongoing objective to defend a territory. It never self-completes —
    /// the character holds this objective until explicitly removed or the character is defeated.
    /// </summary>
    public class DefendTerritoryObjective : IAIObjective
    {
        /// <summary>Optional ID of the character to prioritise as a threat.</summary>
        public CharacterId? PriorityTargetId { get; }

        // This objective is persistent; IsCompleted is always false.
        public ILive<bool> IsCompleted { get; } = new Live<bool>(false);

        public DefendTerritoryObjective(CharacterId? priorityTargetId = null)
        {
            PriorityTargetId = priorityTargetId;
        }
    }
}
