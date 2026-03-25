using System.Collections.Generic;
using UnityEngine;

namespace Gast.Unity.Features.Characters
{
    [CreateAssetMenu(fileName = "CharacterActionProfile", menuName = "Gast/Characters/Action Profile")]
    public class CharacterActionProfile : ScriptableObject
    {
        [SerializeField]
        List<CharacterActionSettings> actionSettings = new();

        public IReadOnlyList<CharacterActionSettings> ActionSettings => actionSettings;
    }
}
