using Gast.Domain.Characters;
using Gast.Domain.Loot;
using Gast.Features.Characters;
using Gast.Infrastructure.Pickups;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gast.Infrastructure.Characters
{
    [CreateAssetMenu(fileName = "CharacterType", menuName = "Gast/Characters/Type Definition")]
    public class CharacterTypeDefinition : ScriptableObject, ICharacterTypeDefinition
    {
        [SerializeField]
        CharacterTypeReference reference;

        [SerializeField]
        string displayName;

        [Header("Movement")]
        [SerializeField]
        float walkSpeed = 4f;

        [SerializeField]
        float sprintSpeed = 7f;

        [Header("Actions")]
        [SerializeField]
        List<CharacterActionSettings> actionSettings = new();

        [Header("Economy")]
        [SerializeField]
        int initialMoney = 100;

        [SerializeField]
        int slotCapacity = 20;

        [Header("Prefabs")]
        [SerializeField]
        Character characterPrefab;

        [SerializeField]
        GameObject visualPrefab;

        [Header("Loot")]
        [SerializeField]
        LootTable lootTable;

        [Header("Extensions")]
        [SerializeField]
        UnityEngine.Object[] extensions = { };

        public CharacterTypeId TypeId => reference.Id;
        public string DisplayName => displayName;
        public float WalkSpeed => walkSpeed;
        public float SprintSpeed => sprintSpeed;
        public IReadOnlyList<CharacterActionSettings> ActionSettings => actionSettings;
        public int InitialMoney => initialMoney;
        public int SlotCapacity => slotCapacity;
        public Character CharacterPrefab => characterPrefab;
        public GameObject VisualPrefab => visualPrefab;

        ILootTable ICharacterTypeDefinition.LootTable => lootTable;

        public T GetExtension<T>()
        {
            return extensions.OfType<T>().FirstOrDefault();
        }

        public IReadOnlyList<UnityEngine.Object> Extensions => extensions;
    }
}