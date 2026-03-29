using System;
using Gast.Domain.Economy;
using UnityEngine;

namespace Gast.Unity.Features.Economy
{
    [CreateAssetMenu(fileName = "ItemReference", menuName = "Gast/Economy/Item Reference")]
    public class ItemReference : ScriptableObject
    {
        [SerializeField]
        string id;

        public ItemId Id => ItemId.FromString(id);

        void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }
        }
    }
}