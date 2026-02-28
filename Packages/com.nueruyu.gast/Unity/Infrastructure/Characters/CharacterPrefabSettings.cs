using UnityEngine;

namespace Gast.Unity.Infrastructure.Characters
{
    [CreateAssetMenu(fileName = "CharacterPrefabSettings", menuName = "Gast/Characters/Settings/Prefab Settings")]
    public class CharacterPrefabSettings : ScriptableObject
    {
        [Header("Prefabs")]
        [SerializeField]
        GameObject characterPrefab;

        [SerializeField]
        GameObject visualPrefab;

        public GameObject CharacterPrefab => characterPrefab;
        public GameObject VisualPrefab => visualPrefab;
    }
}
