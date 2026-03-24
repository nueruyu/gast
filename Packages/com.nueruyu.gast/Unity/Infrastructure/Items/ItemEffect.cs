using Gast.Domain.Characters;
using Gast.Domain.Economy;
using UnityEngine;

namespace Gast.Unity.Infrastructure.Items
{
    public abstract class ItemEffect : ScriptableObject, IItemEffect
    {
        public abstract void Apply(ICharacter character);
    }
}
