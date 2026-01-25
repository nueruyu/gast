using Gast.Domain.Characters;

namespace Gast.Domain.AI.Goals
{
    [AIObjective("DefeatCharacter", "Defeat a specific number of characters of a certain type.")]
    public class DefeatCharacterGoal : IGoal
    {
        [AIObjectiveParameter("Target character type ID (e.g., 'soldier')")]
        public CharacterTypeId TargetTypeId { get; }

        [AIObjectiveParameter("Number of enemies to defeat")]
        public int TargetQuantity { get; }

        public int CurrentQuantity { get; private set; }
        public bool IsCompleted => CurrentQuantity >= TargetQuantity;

        public DefeatCharacterGoal(CharacterTypeId targetTypeId, int targetQuantity)
        {
            TargetTypeId = targetTypeId;
            TargetQuantity = targetQuantity;
            CurrentQuantity = 0;
        }

        public void IncrementCount()
        {
            if (!IsCompleted)
            {
                CurrentQuantity++;
            }
        }
    }
}
