using Gast.Domain.AI.Attributes;
using Gast.Domain.Economy;

namespace Gast.Domain.AI.Objectives
{
    [AIObjective("AcquireItem", "Collect a specific number of items.")]
    public class AcquireItemObjective : IAIObjective
    {
        [AIObjectiveParameter("Target item ID (Guid)", "string")]
        public ItemId TargetItemId { get; }

        [AIObjectiveParameter("Number of items to collect")]
        public int TargetQuantity { get; }

        public int CurrentQuantity { get; private set; }
        public bool IsCompleted => CurrentQuantity >= TargetQuantity;

        public AcquireItemObjective(ItemId targetItemId, int targetQuantity)
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