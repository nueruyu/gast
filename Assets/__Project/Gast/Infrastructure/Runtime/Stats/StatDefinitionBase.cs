using Gast.Domain.Stats;
using System;
using UnityEngine;

namespace Gast.Infrastructure.Stats
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

        protected virtual void OnValidate()
        {
            if (!string.IsNullOrEmpty(name) && string.IsNullOrEmpty(id))
            {
                id = name.Replace(" ", "");
            }
        }
    }
}