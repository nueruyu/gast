using System;
using UnityEngine;
using Gast.Domain.Economy;

namespace Gast.Features.Economy
{
    [CreateAssetMenu(fileName = "ItemReference", menuName = "DescrioGames/Economy/Item Reference")]
    public class ItemReference : ScriptableObject
    {
        [SerializeField]
        string id;

        public ItemId Id => ItemId.FromGuid(Guid.Parse(id));

        void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }
        }
    }
}