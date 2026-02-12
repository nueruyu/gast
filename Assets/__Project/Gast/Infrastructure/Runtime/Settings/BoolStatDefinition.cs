using UnityEngine;

namespace Gast.Infrastructure.Settings
{
    [CreateAssetMenu(fileName = "BoolStatDefinition", menuName = "Gast/Stats/Bool Stat Definition")]
    public class BoolStatDefinition : StatDefinition<bool>
    {
        public override object ParseValue(string value)
        {
            return bool.TryParse(value, out var result) ? result : DefaultValue;
        }
    }
}
