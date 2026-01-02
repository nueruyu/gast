using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DescrioGames.Infrastructure.Settings
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "DescrioGames/Economy/Item Database")]
    public class ItemDatabaseSettings : ScriptableObject
    {
        [SerializeField]
        List<ItemDefinition> items = new();

        public IReadOnlyList<ItemDefinition> Items => items;
    }
}