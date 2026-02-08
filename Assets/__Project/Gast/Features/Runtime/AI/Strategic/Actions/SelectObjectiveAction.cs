using Cysharp.Threading.Tasks;
using Gast.Domain.AI;
using Gast.Domain.AI.Objectives;
using Gast.Domain.Characters;
using Gast.Domain.Pickups;
using Gast.Lib.AI;
using System.Linq;
using UnityEngine;

namespace Gast.Features.AI.Strategic.Actions
{
    /// <summary>
    /// Selects the best objective to pursue based on a cost evaluation.
    /// Also sets the combat target if the chosen objective is combat-related.
    /// </summary>
    public class SelectObjectiveAction : IAction<StrategicState, AIContext<StrategicState>>
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IPickupRepository _pickupRepository;

        public SelectObjectiveAction(ICharacterRepository characterRepository, IPickupRepository pickupRepository)
        {
            _characterRepository = characterRepository;
            _pickupRepository = pickupRepository;
        }

        public bool CanExecute(StrategicState worldState)
        {
            return worldState.AvailableObjectives.Count > 0;
        }

        public void Simulate(StrategicState worldState)
        {
            // This action selects a goal, which is a cognitive step.
            // It doesn't directly change the simulated world state for planning purposes.
        }

        public UniTask ExecuteAsync(AIContext<StrategicState> ctx)
        {
            var self = ctx.Actor;
            var objectives = ctx.WorldState.AvailableObjectives;

            IAIObjective bestObjective = null;
            ICharacter bestTarget = null;
            float lowestCost = float.MaxValue;

            foreach (var objective in objectives)
            {
                var (cost, target) = CalculateObjectiveCost(self, objective);

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

        private (float cost, ICharacter target) CalculateObjectiveCost(ICharacter self, IAIObjective objective)
        {
            switch (objective)
            {
                case DefeatCharacterObjective defeat:
                    var target = FindClosestCharacterOfType(self, defeat.TargetTypeId);
                    if (target == null) return (float.MaxValue, null);
                    var distanceToEnemy = Vector3.Distance(self.Body.Position, target.Body.Position);
                    return (distanceToEnemy, target);

                case AcquireItemObjective acquire:
                    var pickup = FindClosestPickupOfType(self, acquire.TargetItemId);
                    if (pickup == null) return (float.MaxValue, null);
                    var distanceToItem = Vector3.Distance(self.Body.Position, pickup.Position);
                    return (distanceToItem * 0.5f, null); // Prioritize item gathering by halving the cost

                default:
                    return (float.MaxValue, null);
            }
        }

        private ICharacter FindClosestCharacterOfType(ICharacter self, CharacterTypeId typeId)
        {
            return _characterRepository.GetAll()
                .Where(c => c.TypeId == typeId && c.IsAlive && c.Faction != self.Faction)
                .OrderBy(c => Vector3.Distance(self.Body.Position, c.Body.Position))
                .FirstOrDefault();
        }

        private IPickup FindClosestPickupOfType(ICharacter self, Domain.Economy.ItemId itemId)
        {
            return _pickupRepository.GetAll()
                .Where(p => p.ItemId == itemId)
                .OrderBy(p => Vector3.Distance(self.Body.Position, p.Position))
                .FirstOrDefault();
        }
    }
}
