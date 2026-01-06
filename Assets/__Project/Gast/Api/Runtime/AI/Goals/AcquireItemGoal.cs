using Gast.Domain.Economy;

namespace Gast.Domain.Npcs.Goals
{
    public class AcquireItemGoal : IGoal
    {
        public ItemId TargetItemId { get; }
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
