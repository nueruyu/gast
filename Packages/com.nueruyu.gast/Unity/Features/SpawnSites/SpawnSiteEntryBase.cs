using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Unity.Features.SpawnSites
{
    public abstract class SpawnSiteEntryBase : MonoBehaviour
    {
        public abstract ICharacterCreationParameters CreationParameters { get; }
    }
}
