using Gast.Domain.Characters;

namespace Cryst.Domain.Combat
{
    public readonly struct GrappleAttackInfo
    {
        public CharacterId AttackerId { get; }

        public GrappleAttackInfo(CharacterId attackerId)
        {
            AttackerId = attackerId;
        }
    }
}
