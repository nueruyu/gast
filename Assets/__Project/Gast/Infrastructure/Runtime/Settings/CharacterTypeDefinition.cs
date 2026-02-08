using Gast.Domain.Characters;
using Gast.Domain.Loot;
using Gast.Domain.Stats;
using Gast.Features.Characters;
using Gast.Features.Characters.Actions;
using Gast.Features.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gast.Infrastructure.Settings
{
    /// <summary>
    /// ScriptableObject implementation of ICharacterTypeDefinition.
    /// Stores character type configuration data.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterType", menuName = "Gast/Characters/Type Definition")]
    public class CharacterTypeDefinition : ScriptableObject, ICharacterTypeDefinition
    {
        [Serializable]
        public class InitialStat
        {
            public StatDefinition Definition;
            public float Value;
        }

        [SerializeField]
        CharacterTypeReference reference;

        [SerializeField]
        string displayName;

        [Header("Stats")]
        [SerializeField]
        List<InitialStat> initialStats = new();

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

        [Header("Combat")]
        [SerializeField]
        CombatMethodSettings combatMethodSettings;

        [Header("Audio")]
        [SerializeField]
        CharacterFootstepSettings footstepSettings;

        [Header("Loot")]
        [SerializeField]
        LootTable lootTable;

        [Header("Sensor")]
        [SerializeField]
        float sensorViewRadius = 30f;

        [SerializeField]
        float sensorViewAngle = 180f;

        [SerializeField]
        Vector3 sensorEyeOffset = new(0, 1.5f, 0);

        [Header("Navigation")]
        [SerializeField]
        float navigationStoppingDistance = 0.5f;

        public CharacterTypeId TypeId => reference.Id;
        public string DisplayName => displayName;
        public float WalkSpeed => walkSpeed;
        public float SprintSpeed => sprintSpeed;
        public IReadOnlyList<CharacterActionSettings> ActionSettings => actionSettings;
        public bool CanGuard => actionSettings.Any(s => s is GuardActionSettings);
        public int InitialMoney => initialMoney;
        public int SlotCapacity => slotCapacity;
        public Character CharacterPrefab => characterPrefab;
        public GameObject VisualPrefab => visualPrefab;
        public CombatMethodSettings CombatMethodSettings => combatMethodSettings;
        public CharacterFootstepSettings FootstepSettings => footstepSettings;
        public float SensorViewRadius => sensorViewRadius;
        public float SensorViewAngle => sensorViewAngle;
        public Vector3 SensorEyeOffset => sensorEyeOffset;
        public float NavigationStoppingDistance => navigationStoppingDistance;
        public List<InitialStat> InitialStats => initialStats;

        ILootTable ICharacterTypeDefinition.LootTable => lootTable;
    }
}