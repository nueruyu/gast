using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gast.Infrastructure.Items
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Gast/Economy/Item Database")]
    public class ItemDatabaseSettings : ScriptableObject
    {
        [SerializeField]
        List<ItemDefinition> items = new();

        public IReadOnlyList<ItemDefinition> Items => items;
    }
}