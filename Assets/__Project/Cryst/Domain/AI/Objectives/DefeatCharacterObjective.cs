using Gast.Core.Observables;
using Gast.Domain.AI;
using Gast.Domain.AI.Attributes;
using Gast.Domain.Characters;

namespace Cryst.Domain.AI.Objectives
{
    [AIObjective("DefeatCharacter", "Defeat a specific number of characters of a certain type.")]
    public class DefeatCharacterObjective : IAIObjective
    {
        [AIObjectiveParameter("Target character type ID (Guid)", "string")]
        public CharacterTypeId TargetTypeId { get; }

        [AIObjectiveParameter("Number of enemies to defeat")]
        public int TargetQuantity { get; }

        private readonly Live<int> currentQuantity = new(0);
        public ILive<int> CurrentQuantity => currentQuantity;
        public ILive<bool> IsCompleted { get; }

        public DefeatCharacterObjective(CharacterTypeId targetTypeId, int targetQuantity)
        {
            TargetTypeId = targetTypeId;
            TargetQuantity = targetQuantity;
            IsCompleted = currentQuantity.Select(current => current >= TargetQuantity);
        }

        public void IncrementCount()
        {
            if (!IsCompleted.Value)
            {
                currentQuantity.Value++;
            }
        }
    }
}
