using Gast.Domain.Stats;
using UnityEngine;

namespace Gast.Infrastructure.Stats
{
    public abstract class StatSchema : ScriptableObject, IStatSchema
    {
        public abstract void Initialize(IStatRegistrar registrar);
    }
}