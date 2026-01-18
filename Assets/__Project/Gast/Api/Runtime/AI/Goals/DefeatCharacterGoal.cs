using Gast.Domain.Characters;

namespace Gast.Api.AI.Goals
{
    public class DefeatCharacterGoal : IGoal
    {
        public CharacterTypeId TargetTypeId { get; }
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