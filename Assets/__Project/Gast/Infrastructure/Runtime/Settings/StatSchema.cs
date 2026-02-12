using Gast.Domain.Stats;
using UnityEngine;

namespace Gast.Infrastructure.Settings
{
    public abstract class StatSchema : ScriptableObject, IStatSchema
    {
        public abstract void Initialize(IStatRegistrar registrar);
    }
}
