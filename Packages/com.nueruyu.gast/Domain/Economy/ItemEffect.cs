using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Domain.Economy
{
    public abstract class ItemEffect : ScriptableObject
    {
        public abstract void Apply(ICharacter character);
    }
}
