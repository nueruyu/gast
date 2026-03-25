using System.Linq;
using Cryst.Domain.AI.Objectives;
using Cryst.Domain.Characters;
using Gast.Domain.Characters;
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

        public ActorInfo? FindBestTargetFor(DefeatCharacterObjective objective, ObjectiveState state)
        {
            return characterRepository.GetAll()
                .Select(c => c.As<BaseCharacter>())
                .Where(c =>
                    c.TypeId == objective.TargetTypeId &&
                    c.Faction != state.Self.Faction &&
                    c.Status.IsAlive.Value)
                .Select(c => new ActorInfo(c.Id, c.TypeId, c.Body.Position, c.Faction, c.Status.IsAlive.Value))
                .Cast<ActorInfo?>()
                .OrderBy(c => Vector3.SqrMagnitude(state.Self.Position - c.Value.Position))
                .FirstOrDefault();
        }

        public PickupInfo? FindBestTargetFor(AcquireItemObjective objective, ObjectiveState state)
        {
            return pickupRepository.GetAll()
                .Where(p => p.ItemId == objective.TargetItemId)
                .Select(p => new PickupInfo(p.Id, p.ItemId, p.Position))
                .Cast<PickupInfo?>()
                .OrderBy(p => Vector3.SqrMagnitude(state.Self.Position - p.Value.Position))
                .FirstOrDefault();
        }
    }
}
