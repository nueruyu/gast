using UnityEngine;

namespace Gast.Infrastructure.Stats
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