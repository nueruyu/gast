using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Unity.Features.SpawnSites
{
    public abstract class SpawnSiteEntryBase : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Optional fixed character id. Leave empty for dynamic generation.")]
        string fixedCharacterId;

        public CharacterId? FixedCharacterId =>
            string.IsNullOrEmpty(fixedCharacterId) ? null : CharacterId.FromString(fixedCharacterId);

        public abstract ICharacterCreationParameters CreationParameters { get; }
    }
}
