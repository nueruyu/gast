using Gast.Domain.Economy;
using Gast.Unity.Features.Economy;
using UnityEngine;

namespace Gast.Unity.Infrastructure.Items
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Gast/Economy/Item Definition")]
    public class ItemDefinition : ScriptableObject, IItemDefinition
    {
        [SerializeField]
        ItemReference itemReference;

        [SerializeField]
        string displayName;

        [SerializeField]
        int price;

        [SerializeField, Min(1)]
        int maxStack = 99;

        [TextArea(3, 5)]
        [SerializeField]
        string description;

        [SerializeField]
        GameObject prefab;

        [SerializeField]
        Sprite icon;

        public ItemId Id => itemReference.Id;
        public string Name => displayName;
        public int Price => price;
        public int MaxStack => maxStack;
        public string Description => description;
        public GameObject Prefab => prefab;
        public Sprite Icon => icon;
    }
}