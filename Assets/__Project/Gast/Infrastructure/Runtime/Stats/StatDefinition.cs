using System;
using UnityEngine;

namespace Gast.Infrastructure.Stats
{
    public abstract class StatDefinition<T> : StatDefinitionBase, Domain.Stats.IStatDefinition<T>
    {
        public override Type ValueType => typeof(T);
    }
}