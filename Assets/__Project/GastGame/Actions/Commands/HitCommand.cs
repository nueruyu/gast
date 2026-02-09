using Gast.Domain.Characters;
using Gast.Domain.Combat;

namespace GastGame.Actions.Commands
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