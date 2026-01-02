using Gast.Features.Characters;
using UnityEngine;

namespace Gast.Features.Combat
{
    public abstract class CombatMethodSettings : ScriptableObject
    {
        /// <summary>
        /// Creates the attack method instance based on this provider's data.
        /// </summary>
        public abstract ICombatMethod CreateMethod(CombatContext context);
    }
}