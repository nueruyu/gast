using System;
using UnityEngine;

namespace Gast.Infrastructure.Stats
{
    public abstract class StatDefinition<T> : StatDefinitionBase, Domain.Stats.IStatDefinition<T>
    {
        [SerializeField]
        T defaultValue;

        public T DefaultValue => defaultValue;
        public override Type ValueType => typeof(T);

        public override object GetDefaultValueAsObject()
        {
            return DefaultValue;
        }
    }
}