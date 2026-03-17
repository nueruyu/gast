using System.Linq;
using System.Threading;
using Cryst.Domain.AI.Objectives;
using Cryst.Domain.Characters;
using Cysharp.Threading.Tasks;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Pickups;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Strategic.Actions
{
    public class SelectObjectiveAction : IAction<ActorContext<StrategicState>, StrategicState>
    {
        public bool IsAvailable(StrategicState worldState)
        {
            return worldState.AvailableObjectives.Count > 0;
        }

        public void Simulate(StrategicState worldState)
        {
        }

        public UniTask ExecuteAsync(ActorContext<StrategicState> context, CancellationToken cancellationToken)
        {
            var self = context.Actor;
            var objectives = context.WorldState.AvailableObjectives;

            IAIObjective bestObjective = null;
            BaseCharacter bestTarget = null;
            var lowestCost = float.MaxValue;

            foreach (var objective in objectives)
            {
                var (cost, target) = CalculateObjectiveCost(context, self, objective);

                if (cost < lowestCost)
                {
                    lowestCost = cost;
                    bestObjective = objective;
                    bestTarget = target;
                }
            }

            var memory = context.GetModule<HumanoidMemory>();
            memory.SetObjective(bestObjective);
            memory.SetCombatTarget(bestTarget);

            return UniTask.CompletedTask;
        }

        (float cost, BaseCharacter target) CalculateObjectiveCost(ActorContext<StrategicState> context,
            BaseCharacter self, IAIObjective objective)
        {
            switch (objective)
            {
                case DefeatCharacterObjective defeat:
                    var target = FindClosestCharacterOfType(context, self, defeat.TargetTypeId);
                    if (target == null)
                        return (float.MaxValue, null);
                    var distanceToEnemy = Vector3.Distance(self.Body.Position, target.Body.Position);
                    return (distanceToEnemy, target);

                case AcquireItemObjective acquire:
                    var pickup = FindClosestPickupOfType(context, self, acquire.TargetItemId);
                    if (pickup == null)
                        return (float.MaxValue, null);
                    var distanceToItem = Vector3.Distance(self.Body.Position, pickup.Position);
                    return (distanceToItem * 0.5f, null);

                default:
                    return (float.MaxValue, null);
            }
        }

        BaseCharacter FindClosestCharacterOfType(ActorContext<StrategicState> context, BaseCharacter self,
            CharacterTypeId typeId)
        {
            return context.CharacterRepository.GetAll()
                .Select(c => c.As<BaseCharacter>())
                .Where(a => a.TypeId == typeId && a.Status.IsAlive.Value && a.Faction != self.Faction)
                .OrderBy(a => Vector3.Distance(self.Body.Position, a.Body.Position))
                .FirstOrDefault();
        }

        IPickup FindClosestPickupOfType(ActorContext<StrategicState> context, BaseCharacter self, ItemId itemId)
        {
            return context.PickupRepository.GetAll()
                .Where(p => p.ItemId == itemId)
                .OrderBy(p => Vector3.Distance(self.Body.Position, p.Position))
                .FirstOrDefault();
        }
    }
}