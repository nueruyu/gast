using Gast.Features.Characters;
using GastGame.Features.Characters;
using UnityEngine;

namespace GastGame.Features.CharacterActions
{
    [CreateAssetMenu(fileName = "DashActionSettings", menuName = "Gast/Actions/Dash Action Settings")]
    public class DashActionSettings : CharacterActionSettings
    {
        [SerializeField]
        float duration = 0.5f;

        [SerializeField]
        float cooldown = 1.0f;

        [SerializeField]
        float maxSpeed = 15f;

        [SerializeField]
        AnimationCurve speedCurve = new(new Keyframe(0, 1), new Keyframe(1, 0));

        [SerializeField]
        float lookDirectionSpeed = 100f;

        public float Duration => duration;
        public float Cooldown => cooldown;
        public float MaxSpeed => maxSpeed;
        public AnimationCurve SpeedCurve => speedCurve;
        public float LookDirectionSpeed => lookDirectionSpeed;

        public override ICharacterAction CreateAction(CharacterContext context)
        {
            var actionContext = CharacterActionContextFactory.Create(context);
            return new DashAction(actionContext, this);
        }
    }
}
