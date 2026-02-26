using Gast.Domain.Characters;
using Cryst.Domain.Combat;

namespace Cryst.Domain.Characters.Commands
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