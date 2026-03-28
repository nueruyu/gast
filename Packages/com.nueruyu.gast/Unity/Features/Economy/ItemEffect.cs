using Gast.Domain.Characters;
using Gast.Domain.Economy;
using UnityEngine;

namespace Gast.Unity.Features.Economy
{
    public abstract class ItemEffect : ScriptableObject, IItemEffect
    {
        public abstract void Apply(ICharacter character);
    }
}
