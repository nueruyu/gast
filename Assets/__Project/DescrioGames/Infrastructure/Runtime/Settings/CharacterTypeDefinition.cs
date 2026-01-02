using DescrioGames.Domain.Characters;
using DescrioGames.Domain.Loot;
using DescrioGames.Features.Characters;
using DescrioGames.Features.Combat;
using UnityEngine;

namespace DescrioGames.Infrastructure.Settings
{
    /// <summary>
    /// ScriptableObject implementation of ICharacterTypeDefinition.
    /// Stores character type configuration data.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterType", menuName = "DescrioGames/Characters/Type Definition")]
    public class CharacterTypeDefinition : ScriptableObject, ICharacterTypeDefinition
    {
        [SerializeField]
        CharacterTypeReference reference;

        [SerializeField]
        string displayName;

        [SerializeField]
        float maxHealth = 100f;

        [Header("Movement")]
        [SerializeField]
        float walkSpeed = 4f;

        [SerializeField]
        float sprintSpeed = 7f;

        [SerializeField]
        float jumpForce = 5f;

        [Header("Actions - Dash")]
        [SerializeField]
        float dashForce = 15f;

        [SerializeField]
        float dashDuration = 0.5f;

        [SerializeField]
        float dashCooldown = 1.0f;

        [SerializeField]
        float attackCooldown = 1f;

        [SerializeField]
        AnimationCurve dashSpeedCurve = new(new Keyframe(0, 1), new Keyframe(1, 0));

        [Header("Actions - Guard")]
        [SerializeField]
        bool canGuard = true;

        [SerializeField]
        int initialMoney = 100;

        [SerializeField]
        int slotCapacity = 20;

        [SerializeField]
        Character characterPrefab;

        [SerializeField]
        GameObject visualPrefab;

        [SerializeField]
        CombatMethodSettings combatMethodSettings;

        [Header("Audio")]
        [SerializeField]
        CharacterFootstepSettings footstepSettings;

        [Header("Economy")]
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
        public float MaxHealth => maxHealth;
        public float WalkSpeed => walkSpeed;
        public float SprintSpeed => sprintSpeed;
        public float JumpForce => jumpForce;
        public float DashForce => dashForce;
        public float DashDuration => dashDuration;
        public float DashCooldown => dashCooldown;
        public float AttackCooldown => attackCooldown;
        public bool CanGuard => canGuard;
        public int InitialMoney => initialMoney;
        public int SlotCapacity => slotCapacity;
        public AnimationCurve DashSpeedCurve => dashSpeedCurve;
        public Character CharacterPrefab => characterPrefab;
        public GameObject VisualPrefab => visualPrefab;
        public CombatMethodSettings CombatMethodSettings => combatMethodSettings;
        public CharacterFootstepSettings FootstepSettings => footstepSettings;
        public float SensorViewRadius => sensorViewRadius;
        public float SensorViewAngle => sensorViewAngle;
        public Vector3 SensorEyeOffset => sensorEyeOffset;
        public float NavigationStoppingDistance => navigationStoppingDistance;

        ILootTable ICharacterTypeDefinition.LootTable => lootTable;
    }
}