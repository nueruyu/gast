using Gast.Features.Characters;
using UnityEngine;

namespace GastGame.Features.CharacterActions
{
    [CreateAssetMenu(fileName = "GuardActionSettings", menuName = "Gast/Actions/Guard Action Settings")]
    public class GuardActionSettings : CharacterActionSettings
    {
        [SerializeField]
        float moveSpeedPenalty = 0.5f;

        [SerializeField]
        float lookDirectionSpeed = 5f;

        public float MoveSpeedPenalty => moveSpeedPenalty;
        public float LookDirectionSpeed => lookDirectionSpeed;

        public override ICharacterAction CreateAction(CharacterContext context)
        {
            return new GuardAction(context, this);
        }
    }
}