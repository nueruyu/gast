using Cysharp.Threading.Tasks;
using Gast.Domain.AI;
using Cryst.Domain.AI.Objectives;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Pickups;
using Gast.Lib.AI;
using Cryst.Domain.Characters;
using System.Linq;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Strategic.Actions
{
    public class SelectObjectiveAction : IAction<StrategicState, AIContext<StrategicState>>
    {
        public bool CanExecute(StrategicState worldState)
        {
            return worldState.AvailableObjectives.Count > 0;
        }

        public void Simulate(StrategicState worldState)
        {
        }

        public UniTask ExecuteAsync(AIContext<StrategicState> ctx)
        {
            var self = ctx.Actor;
            var objectives = ctx.WorldState.AvailableObjectives;

            IAIObjective bestObjective = null;
            BaseCharacter bestTarget = null;
            float lowestCost = float.MaxValue;

            foreach (var objective in objectives)
            {
                var (cost, target) = CalculateObjectiveCost(ctx, self, objective);

                if (cost < lowestCost)
                {
                    lowestCost = cost;
                    bestObjective = objective;
                    bestTarget = target;
                }
            }

            ctx.Memory.CurrentObjective = bestObjective;
            ctx.Memory.CombatTarget = bestTarget;

            return UniTask.CompletedTask;
        }

        private (float cost, BaseCharacter target) CalculateObjectiveCost(AIContext<StrategicState> ctx, BaseCharacter self, IAIObjective objective)
        {
            switch (objective)
            {
                case DefeatCharacterObjective defeat:
                    var target = FindClosestCharacterOfType(ctx, self, defeat.TargetTypeId);
                    if (target == null)
                        return (float.MaxValue, null);
                    var distanceToEnemy = Vector3.Distance(self.Body.Position, target.Body.Position);
                    return (distanceToEnemy, target);

                case AcquireItemObjective acquire:
                    var pickup = FindClosestPickupOfType(ctx, self, acquire.TargetItemId);
                    if (pickup == null)
                        return (float.MaxValue, null);
                    var distanceToItem = Vector3.Distance(self.Body.Position, pickup.Position);
                    return (distanceToItem * 0.5f, null);

                default:
                    return (float.MaxValue, null);
            }
        }

        private BaseCharacter FindClosestCharacterOfType(AIContext<StrategicState> ctx, BaseCharacter self, CharacterTypeId typeId)
        {
            return ctx.CharacterRepository.GetAll()
                .Select(c => c.As<BaseCharacter>())
                .Where(a => a.TypeId == typeId && a.Status.IsAlive.Value && a.Faction != self.Faction)
                .OrderBy(a => Vector3.Distance(self.Body.Position, a.Body.Position))
                .FirstOrDefault();
        }

        private IPickup FindClosestPickupOfType(AIContext<StrategicState> ctx, BaseCharacter self, ItemId itemId)
        {
            return ctx.PickupRepository.GetAll()
                .Where(p => p.ItemId == itemId)
                .OrderBy(p => Vector3.Distance(self.Body.Position, p.Position))
                .FirstOrDefault();
        }
    }
}
