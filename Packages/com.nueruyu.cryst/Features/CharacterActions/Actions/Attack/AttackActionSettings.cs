using System;
using System.Collections.Generic;
using Cryst.Features.CharacterActions.Effects;
using Gast.Unity.Features.Characters;
using Gast.Unity.Shared.Animations;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Attack
{
    [CreateAssetMenu(fileName = "AttackActionSettings", menuName = "Gast/Actions/Attack Action Settings")]
    public class AttackActionSettings : CharacterActionSettings
    {
        [Header("Core")]
        [SerializeField]
        float cooldown = 1f;

        [SerializeField]
        float duration = 0.6f;

        [Header("Animation")]
        [SerializeField]
        AnimatorTriggerSymbol animationTrigger;

        [Header("Effects Triggered by Animation Events")]
        [SerializeField]
        List<TimedEffect> timedEffects = new();

        public float Cooldown => cooldown;
        public float Duration => duration;
        public AnimatorTriggerSymbol AnimationTrigger => animationTrigger;
        public IReadOnlyList<TimedEffect> TimedEffects => timedEffects;

        [Serializable]
        public class TimedEffect
        {
            [SerializeField]
            AnimationEventSymbol eventSymbol;

            [SerializeField]
            CharacterActionEffect effect;

            public AnimationEventSymbol EventSymbol => eventSymbol;
            public CharacterActionEffect Effect => effect;
        }
    }
}
