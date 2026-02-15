using Gast.Domain.Characters;
using Cryst.Domain.Combat;

namespace Cryst.Modules.CharacterActions
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