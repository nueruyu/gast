using Gast.Domain.Economy;

namespace Gast.Domain.AI.Goals
{
    [AIObjective("AcquireItem", "Collect a specific number of items.")]
    public class AcquireItemGoal : IGoal
    {
        [AIObjectiveParameter("Target item ID (Guid)")]
        public ItemId TargetItemId { get; }

        [AIObjectiveParameter("Number of items to collect")]
        public int TargetQuantity { get; }

        public int CurrentQuantity { get; private set; }
        public bool IsCompleted => CurrentQuantity >= TargetQuantity;

        public AcquireItemGoal(ItemId targetItemId, int targetQuantity)
        {
            TargetItemId = targetItemId;
            TargetQuantity = targetQuantity;
            CurrentQuantity = 0;
        }

        public void AddQuantity(int amount)
        {
            if (!IsCompleted)
            {
                CurrentQuantity += amount;
            }
        }
    }
}
