using System.Linq;
using Cryst.Domain.Characters;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Pickups;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Objective
{
    public class ObjectiveQueries : IObjectiveQueries
    {
        readonly ICharacterRepository characterRepository;
        readonly IPickupRepository pickupRepository;

        public ObjectiveQueries(
            ICharacterRepository characterRepository,
            IPickupRepository pickupRepository)
        {
            this.characterRepository = characterRepository;
            this.pickupRepository = pickupRepository;
        }

        public (IAIObjective objective, IActorInfo target) FindBestCombatObjective(ObjectiveState state)
        {
            if (!state.AvailableCombatObjectives.Any()) return (null, null);

            IActorInfo bestTarget = null;
            IAIObjective bestObjective = null;
            var closestDistSq = float.MaxValue;

            var allCharacters = characterRepository.GetAll()
                .Select(c => c.As<BaseCharacter>())
                .ToList();

            foreach (var objective in state.AvailableCombatObjectives)
            {
                var currentBestTarget = allCharacters
                    .Where(c =>
                        c.TypeId == objective.TargetTypeId &&
                        c.Faction != state.Self.Faction &&
                        c.Status.IsAlive.Value)
                    .Select(c => new ActorInfo(c.Id, c.TypeId, c.Body.Position, c.Faction, c.Status.IsAlive.Value))
                    .OrderBy(c => Vector3.SqrMagnitude(state.Self.Position - c.Position))
                    .FirstOrDefault();

                if (currentBestTarget == null) continue;

                var distSq = Vector3.SqrMagnitude(state.Self.Position - currentBestTarget.Position);
                if (distSq < closestDistSq)
                {
                    closestDistSq = distSq;
                    bestTarget = currentBestTarget;
                    bestObjective = objective;
                }
            }

            return (bestObjective, bestTarget);
        }

        public (IAIObjective objective, IPickupInfo target) FindBestGatheringObjective(ObjectiveState state)
        {
            if (!state.AvailableGatheringObjectives.Any()) return (null, null);

            IPickupInfo bestTarget = null;
            IAIObjective bestObjective = null;
            var closestDistSq = float.MaxValue;

            foreach (var objective in state.AvailableGatheringObjectives)
            {
                var currentBestTarget = pickupRepository.GetAll()
                    .Where(p => p.ItemId == objective.TargetItemId)
                    .Select(p => new PickupInfo(p.Id, p.ItemId, p.Position))
                    .OrderBy(p => Vector3.SqrMagnitude(state.Self.Position - p.Position))
                    .FirstOrDefault();

                if (currentBestTarget == null) continue;

                var distSq = Vector3.SqrMagnitude(state.Self.Position - currentBestTarget.Position);
                if (distSq < closestDistSq)
                {
                    closestDistSq = distSq;
                    bestTarget = currentBestTarget;
                    bestObjective = objective;
                }
            }

            return (bestObjective, bestTarget);
        }

        record ActorInfo(
            CharacterId Id,
            CharacterTypeId TypeId,
            Vector3 Position,
            Faction Faction,
            bool IsAlive) : IActorInfo;

        record PickupInfo(
            PickupId Id,
            ItemId ItemId,
            Vector3 Position) : IPickupInfo;
    }
}