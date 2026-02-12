using UnityEngine;

namespace Gast.Infrastructure.Stats
{
    [CreateAssetMenu(fileName = "FloatStatDefinition", menuName = "Gast/Stats/Float Stat Definition")]
    public class FloatStatDefinition : StatDefinition<float>
    {
        public override object ParseValue(string value)
        {
            return float.TryParse(value, out var result) ? result : DefaultValue;
        }
    }
}