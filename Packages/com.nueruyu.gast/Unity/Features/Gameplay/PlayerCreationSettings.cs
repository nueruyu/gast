using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Unity.Features.Gameplay
{
    public abstract class PlayerCreationSettings : ScriptableObject
    {
        public abstract ICharacterCreationParameters Create();
    }
}
