using Gast.Domain.Combat;

namespace Gast.Features.Characters.Actions.Commands
{
    public readonly struct HitCommand : ITriggerActionCommand
    {
        public DamageInfo DamageInfo { get; }

        public HitCommand(DamageInfo damageInfo)
        {
            DamageInfo = damageInfo;
        }
    }
}