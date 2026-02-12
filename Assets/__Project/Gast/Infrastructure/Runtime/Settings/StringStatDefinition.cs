using UnityEngine;

namespace Gast.Infrastructure.Settings
{
    [CreateAssetMenu(fileName = "StringStatDefinition", menuName = "Gast/Stats/String Stat Definition")]
    public class StringStatDefinition : StatDefinition<string>
    {
        public override object ParseValue(string value)
        {
            return value ?? DefaultValue;
        }
    }
}
