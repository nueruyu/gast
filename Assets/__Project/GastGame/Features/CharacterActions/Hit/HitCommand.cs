using Gast.Domain.Characters;
using GastGame.Domain.Combat;

namespace GastGame.Features.CharacterActions
{
    public readonly struct HitCommand : ICharacterTriggerCommand
    {
        public DamageInfo DamageInfo { get; }

        public HitCommand(DamageInfo damageInfo)
        {
            DamageInfo = damageInfo;
        }
    }
}