using Gast.Domain.Characters;
using Gast.Shared.Phantoms;
using UnityEngine;

namespace Cryst.Domain.Combat
{
    public static class AttackContextKeys
    {
        public static IPhantomKey<CharacterId> SourceCharacterId { get; } =
            new PhantomKey<CharacterId>(nameof(SourceCharacterId));

        public static IPhantomKey<float> Damage { get; } =
            new PhantomKey<float>(nameof(Damage));

        public static IPhantomKey<Vector3> KnockbackForce { get; } =
            new PhantomKey<Vector3>(nameof(KnockbackForce));
    }
}