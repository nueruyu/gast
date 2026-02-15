using Cysharp.Threading.Tasks;
using Gast.Features.Characters;
using Gast.Features.Combat;
using Gast.Shared.Observables;
using R3;
using System;
using UnityEngine;

namespace Cryst.Modules.CharacterActions
{
    public class MeleeAttackEffect
    {
        readonly MeleeAttackEffectSettings settings;
        readonly CharacterContext context;

        public MeleeAttackEffect(
            MeleeAttackEffectSettings settings,
            CharacterContext context)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IDisposable BindEvents()
        {
            return context.AnimationReceiver.EventReceived
                .ToObservable()
                .Where(name => name == "WeaponSwing")
                .Subscribe(_ => PlayWeaponSwing(context.Audio.AudioSource));
        }

        public void OnHit(DamageHitInfo hit)
        {
            var point = hit.Point;

            context.FeedbackService.PlayHitEffect(
                point.position,
                point.rotation,
                settings.HitVfxPrefab);

            context.FeedbackService.PlaySound(
                point.position,
                settings.HitSfx,
                settings.SfxVolume);
        }

        void PlayWeaponSwing(AudioSource audioSource)
        {
            audioSource.volume = settings.SwingSfxVolume;
            audioSource.pitch = 1.0f;
            audioSource.PlayOneShot(settings.SwingSfx);
        }
    }
}
