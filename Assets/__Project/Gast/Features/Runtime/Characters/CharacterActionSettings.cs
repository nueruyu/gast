using Gast.Features.Combat;
using UnityEngine;

namespace Gast.Features.Characters
{
    public abstract class CharacterActionSettings : ScriptableObject
    {
        public abstract ICharacterAction CreateAction(CharacterContext context);
    }
}