using System;
using Gast.Core.Values;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Combat.Actions
{
    [Serializable]
    public class PostAttackManeuverActionSettings
    {
        [SerializeField]
        Rate guardChance = 0.3f;

        [SerializeField]
        Rate strafeChance = 0.4f;

        public Rate GuardChance => guardChance;
        public Rate StrafeChance => strafeChance;
    }
}
