using Gast.Domain.Stats;
using UnityEngine;

namespace Gast.Infrastructure.Settings
{
    [CreateAssetMenu(fileName = "StatDefinition", menuName = "Gast/Stats/Stat Definition")]
    public class StatDefinition : ScriptableObject, IStatDefinition
    {
        [SerializeField]
        string id;

        [SerializeField]
        string displayName;

        public StatId Id => StatId.FromString(id);
        public string DisplayName => displayName;

        void OnValidate()
        {
            if (!string.IsNullOrEmpty(name) && string.IsNullOrEmpty(id))
            {
                id = name.Replace(" ", "");
            }
        }
    }
}
