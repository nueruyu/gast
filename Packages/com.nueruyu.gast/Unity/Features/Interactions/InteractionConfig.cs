using System;
using Gast.Domain.Interactions;
using UnityEngine;

namespace Gast.Unity.Features.Interactions
{
    /// <summary>
    /// Default implementation of IInteractionConfig.
    /// </summary>
    [Serializable]
    public class InteractionConfig : IInteractionConfig
    {
        [SerializeField, Tooltip("Text shown to the player (e.g., 'Pickup', 'Power On')")]
        string prompt = "Interact";

        [SerializeField, Tooltip("Key binding hint (e.g., 'E')")]
        string key = "E";

        [SerializeField, Tooltip("Type of interaction")]
        InteractionType type = InteractionType.Instant;

        [SerializeField, Tooltip("Duration for hold interactions (seconds)")]
        float holdDuration = 1.5f;

        public string Prompt
        {
            get => prompt;
            set => prompt = value;
        }

        public string Key
        {
            get => key;
            set => key = value;
        }

        public InteractionType Type
        {
            get => type;
            set => type = value;
        }

        public float HoldDuration
        {
            get => holdDuration;
            set => holdDuration = value;
        }
    }
}