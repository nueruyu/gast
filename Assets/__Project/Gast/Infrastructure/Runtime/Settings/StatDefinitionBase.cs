using Gast.Domain.Stats;
using System;
using UnityEngine;

namespace Gast.Infrastructure.Settings
{
    public abstract class StatDefinitionBase : ScriptableObject, IStatDefinition
    {
        [SerializeField]
        string id;

        [SerializeField]
        string displayName;

        public StatId Id => StatId.FromString(id);
        public string DisplayName => displayName;

        public abstract Type ValueType { get; }
        public abstract object GetDefaultValueAsObject();
        public abstract object ParseValue(string value);

        protected virtual void OnValidate()
        {
            if (!string.IsNullOrEmpty(name) && string.IsNullOrEmpty(id))
            {
                id = name.Replace(" ", "");
            }
        }
    }
}
