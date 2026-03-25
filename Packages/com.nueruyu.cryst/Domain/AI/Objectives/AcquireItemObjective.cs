using Gast.Core.Observables;
using Gast.Domain.AI;
using Gast.Domain.AI.Attributes;
using Gast.Domain.Economy;

namespace Cryst.Domain.AI.Objectives
{
    [AIObjective("AcquireItem", "Collect a specific number of items.")]
    public class AcquireItemObjective : IAIObjective
    {
        [AIObjectiveParameter("Target item ID (Guid)", "string")]
        public ItemId TargetItemId { get; }

        [AIObjectiveParameter("Number of items to collect")]
        public int TargetQuantity { get; }

        readonly Live<int> currentQuantity = new(0);
        public ILive<int> CurrentQuantity => currentQuantity;
        public ILive<bool> IsCompleted { get; }

        public AcquireItemObjective(ItemId targetItemId, int targetQuantity)
        {
            TargetItemId = targetItemId;
            TargetQuantity = targetQuantity;
            IsCompleted = currentQuantity.Select(current => current >= TargetQuantity);
        }

        public void AddQuantity(int amount)
        {
            if (!IsCompleted.Value)
            {
                currentQuantity.Value += amount;
            }
        }
    }
}
