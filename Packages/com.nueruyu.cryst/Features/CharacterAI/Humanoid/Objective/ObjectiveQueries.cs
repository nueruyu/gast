using System.Linq;
using Cryst.Domain.AI.Objectives;
using Cryst.Domain.Characters;
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

        public IActorInfo FindBestTargetFor(DefeatCharacterObjective objective, ObjectiveState state)
        {
            var allCharacters = characterRepository.GetAll()
                .Select(c => c.As<BaseCharacter>())
                .ToList();

            return allCharacters
                .Where(c =>
                    c.TypeId == objective.TargetTypeId &&
                    c.Faction != state.Self.Faction &&
                    c.Status.IsAlive.Value)
                .Select(c => new ActorInfo(c.Id, c.TypeId, c.Body.Position, c.Faction, c.Status.IsAlive.Value))
                .OrderBy(c => Vector3.SqrMagnitude(state.Self.Position - c.Position))
                .FirstOrDefault();
        }

        public IPickupInfo FindBestTargetFor(AcquireItemObjective objective, ObjectiveState state)
        {
            return pickupRepository.GetAll()
                .Where(p => p.ItemId == objective.TargetItemId)
                .Select(p => new PickupInfo(p.Id, p.ItemId, p.Position))
                .OrderBy(p => Vector3.SqrMagnitude(state.Self.Position - p.Position))
                .FirstOrDefault();
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
