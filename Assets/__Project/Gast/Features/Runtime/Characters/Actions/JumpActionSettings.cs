using Gast.Features.Combat;
using UnityEngine;

namespace Gast.Features.Characters.Actions
{
    [CreateAssetMenu(fileName = "JumpActionSettings", menuName = "Gast/Actions/Jump Action Settings")]
    public class JumpActionSettings : CharacterActionSettings
    {
        [SerializeField]
        float force = 5f;

        [SerializeField]
        float lookDirectionSpeed = 10f;

        public float Force => force;
        public float LookDirectionSpeed => lookDirectionSpeed;

        public override ICharacterAction CreateAction(CharacterContext context, ICombatMethod combatMethod)
        {
            return new JumpAction(context, this);
        }
    }
}
