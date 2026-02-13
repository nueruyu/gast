using Gast.Features.Characters;
using GastGame.Features.Characters;
using UnityEngine;

namespace GastGame.Features.CharacterActions
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

        public override ICharacterAction CreateAction(CharacterContext context)
        {
            var actionContext = CharacterActionContextFactory.Create(context);
            return new JumpAction(actionContext, this);
        }
    }
}
