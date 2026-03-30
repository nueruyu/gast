using Cryst.Domain.Combat;
using Gast.Domain.Characters;
using Gast.Unity.Features.Characters;
using Gast.Unity.Features.HitDetection;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Effects
{
    public class SpawnGrappleAreaEffectHandler : ICharacterActionEffectHandler<SpawnGrappleAreaEffect>
    {
        readonly IHitAreaFactory hitAreaFactory;
        readonly CharacterBody body;
        readonly CharacterId characterId;

        public SpawnGrappleAreaEffectHandler(
            IHitAreaFactory hitAreaFactory,
            CharacterBody body,
            CharacterId characterId)
        {
            this.hitAreaFactory = hitAreaFactory;
            this.body = body;
            this.characterId = characterId;
        }

        public void Handle(SpawnGrappleAreaEffect effect)
        {
            var attackerTransform = body.transform;
            var spawnPosition = attackerTransform.position
                + attackerTransform.rotation * effect.Offset
                + attackerTransform.forward * effect.Range;
            var spawnRotation = attackerTransform.rotation;
            var pose = new Pose(spawnPosition, spawnRotation);

            var grappleInfo = new GrappleAttackInfo(characterId);

            hitAreaFactory.Create(
                pose,
                effect.HitboxSize,
                effect.Duration,
                grappleInfo);
        }
    }
}
