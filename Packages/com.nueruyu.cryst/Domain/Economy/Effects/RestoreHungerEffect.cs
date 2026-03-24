using Cryst.Domain.Characters;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using UnityEngine;

namespace Cryst.Domain.Economy.Effects
{
    [CreateAssetMenu(fileName = "RestoreHungerEffect", menuName = "Cryst/Item Effects/Restore Hunger")]
    public class RestoreHungerEffect : ItemEffect
    {
        [SerializeField]
        float recoveryAmount = 20f;

        public override void Apply(ICharacter character)
        {
            if (character.Is(out BaseCharacter baseCharacter))
            {
                var status = baseCharacter.Status;
                status.SetHunger(status.Hunger.Value + recoveryAmount);
            }
        }
    }
}
