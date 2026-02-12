using UnityEngine;

namespace Gast.Infrastructure.Stats
{
    [CreateAssetMenu(fileName = "IntStatDefinition", menuName = "Gast/Stats/Int Stat Definition")]
    public class IntStatDefinition : StatDefinition<int>
    {
        public override object ParseValue(string value)
        {
            return int.TryParse(value, out var result) ? result : DefaultValue;
        }
    }
}