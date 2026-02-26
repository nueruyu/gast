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

namespace Cryst.Modules.CharacterAI.Strategic.Actions
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
            CrystCharacter bestTarget = null;
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

        private (float cost, CrystCharacter target) CalculateObjectiveCost(CrystCharacter self, IAIObjective objective)
        {
            switch (objective)
            {
                case DefeatCharacterObjective defeat:
                    var target = FindClosestCharacterOfType(self, defeat.TargetTypeId);
                    if (target == null)
                        return (float.MaxValue, null);
                    var distanceToEnemy = Vector3.Distance(self.Body.Position, target.Body.Position);
                    return (distanceToEnemy, target);

                case AcquireItemObjective acquire:
                    var pickup = FindClosestPickupOfType(self, acquire.TargetItemId);
                    if (pickup == null)
                        return (float.MaxValue, null);
                    var distanceToItem = Vector3.Distance(self.Body.Position, pickup.Position);
                    return (distanceToItem * 0.5f, null); // Prioritize item gathering by halving the cost

                default:
                    return (float.MaxValue, null);
            }
        }

        private CrystCharacter FindClosestCharacterOfType(CrystCharacter self, CharacterTypeId typeId)
        {
            return _characterRepository.GetAll()
                .Select(c => c.As<CrystCharacter>())
                .Where(a => a.TypeId == typeId && a.Status.IsAlive.Value && a.Faction != self.Faction)
                .OrderBy(a => Vector3.Distance(self.Body.Position, a.Body.Position))
                .FirstOrDefault();
        }

        private IPickup FindClosestPickupOfType(CrystCharacter self, ItemId itemId)
        {
            return _pickupRepository.GetAll()
                .Where(p => p.ItemId == itemId)
                .OrderBy(p => Vector3.Distance(self.Body.Position, p.Position))
                .FirstOrDefault();
        }
    }
}