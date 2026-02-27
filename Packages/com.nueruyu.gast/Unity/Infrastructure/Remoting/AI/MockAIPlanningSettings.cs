using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gast.Infrastructure.Remoting.AI
{
    [CreateAssetMenu(fileName = "MockAIPlanningSettings", menuName = "Gast/AI/Mock AI Planning Settings")]
    public class MockAIPlanningSettings : ScriptableObject
    {
        [Header("Mock Settings")]
        [Tooltip("If true, the mock AI planning service will be used instead of the real one.")]
        [SerializeField]
        private bool isEnabled = false;

        [Header("Mock Plan")]
        [Tooltip("The list of objectives the mock service will return.")]
        [SerializeField]
        private List<MockObjective> mockObjectives = new();

        public bool IsEnabled => isEnabled;
        public IReadOnlyList<MockObjective> MockObjectives => mockObjectives;

        [Serializable]
        public class MockObjective
        {
            [Tooltip("The type name of the objective (e.g., 'AcquireItem', 'DefeatCharacter').")]
            public string Type = "AcquireItem";

            [Tooltip("The priority of the objective (higher values are processed first).")]
            public int Priority = 1;

            [Tooltip("Parameters for the objective.")]
            public List<MockParameter> Parameters = new();
        }

        [Serializable]
        public class MockParameter
        {
            [Tooltip("The snake_case key for the parameter (e.g., 'target_item_id', 'target_quantity').")]
            public string Key = "target_item_id";

            [Tooltip("The string value for the parameter. It will be converted to the target type (e.g., int, Guid).")]
            public string Value = "00000000-0000-0000-0000-000000000000";
        }
    }
}