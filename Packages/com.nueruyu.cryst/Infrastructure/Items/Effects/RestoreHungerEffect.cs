using Cryst.Domain.Characters;
using Gast.Domain.Characters;
using Gast.Unity.Infrastructure.Items;
using UnityEngine;

namespace Cryst.Infrastructure.Items.Effects
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
