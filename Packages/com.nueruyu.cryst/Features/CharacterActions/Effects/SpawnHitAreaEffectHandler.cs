using Cryst.Domain.Combat;
using Gast.Domain.Characters;
using Gast.Unity.Features.Characters;
using Gast.Unity.Features.HitDetection;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Effects
{
    /// <summary>
    /// Handles <see cref="SpawnHitAreaEffect"/> by spawning a hit detection area
    /// via <see cref="IHitAreaFactory"/>.
    /// </summary>
    public class SpawnHitAreaEffectHandler : ICharacterActionEffectHandler<SpawnHitAreaEffect>
    {
        readonly IHitAreaFactory hitAreaFactory;
        readonly CharacterBody body;
        readonly CharacterId characterId;

        public SpawnHitAreaEffectHandler(IHitAreaFactory hitAreaFactory, CharacterBody body, CharacterId characterId)
        {
            this.hitAreaFactory = hitAreaFactory;
            this.body = body;
            this.characterId = characterId;
        }

        public void Handle(SpawnHitAreaEffect effect)
        {
            var attackerTransform = body.transform;
            var forward = attackerTransform.forward;
            var spawnPosition = attackerTransform.position + attackerTransform.rotation * effect.Offset + forward * effect.Range;
            var spawnRotation = attackerTransform.rotation;
            var pose = new Pose(spawnPosition, spawnRotation);
            var knockbackDirection = spawnRotation * Vector3.forward;

            var attackInfo = new AttackInfo(
                characterId,
                effect.Damage,
                effect.KnockbackForce * knockbackDirection);

            hitAreaFactory.Create(
                pose,
                effect.HitboxSize,
                effect.Duration,
                attackInfo);
        }
    }
}
