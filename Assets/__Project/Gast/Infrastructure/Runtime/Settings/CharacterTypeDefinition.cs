using Gast.Domain.Characters;
using Gast.Domain.Loot;
using Gast.Domain.Stats;
using Gast.Features.Characters;
using Gast.Features.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gast.Infrastructure.Settings
{
    [CreateAssetMenu(fileName = "CharacterType", menuName = "Gast/Characters/Type Definition")]
    public class CharacterTypeDefinition : ScriptableObject, ICharacterTypeDefinition
    {
        [SerializeField]
        CharacterTypeReference reference;

        [SerializeField]
        string displayName;

        [Header("Stats")]
        [SerializeField]
        StatSchema statSchema;

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
        public int InitialMoney => initialMoney;
        public int SlotCapacity => slotCapacity;
        public Character CharacterPrefab => characterPrefab;
        public GameObject VisualPrefab => visualPrefab;
        public CharacterFootstepSettings FootstepSettings => footstepSettings;
        public float SensorViewRadius => sensorViewRadius;
        public float SensorViewAngle => sensorViewAngle;
        public Vector3 SensorEyeOffset => sensorEyeOffset;
        public float NavigationStoppingDistance => navigationStoppingDistance;

        public IStatSchema StatSchema => statSchema;

        ILootTable ICharacterTypeDefinition.LootTable => lootTable;
    }
}