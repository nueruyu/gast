using System.Collections.Generic;
using System.Linq;
using Gast.Domain.Interactions;
using UnityEngine;

namespace Gast.Features.Interactions
{
    public class InteractionSensor : MonoBehaviour, IInteractionSensor
    {
        [Header("Detection Settings")]
        [SerializeField]
        float detectionRadius = 3f;

        [SerializeField]
        LayerMask interactableLayer;

        [SerializeField]
        float detectionInterval = 0.2f;

        readonly List<IInteractable> detectableInteractables = new();
        readonly Collider[] overlapBuffer = new Collider[16];
        float timer;

        public IReadOnlyList<IInteractable> DetectableInteractables => detectableInteractables;

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= detectionInterval)
            {
                timer = 0f;
                Detect();
            }
        }

        void Detect()
        {
            detectableInteractables.Clear();
            var count = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, overlapBuffer, interactableLayer);

            for (var i = 0; i < count; i++)
            {
                if (overlapBuffer[i].TryGetComponent<IInteractable>(out var interactable))
                {
                    if (interactable.CanInteract)
                    {
                        detectableInteractables.Add(interactable);
                    }
                }
            }
        }

        public bool IsDetectable(InteractableId id)
        {
            return detectableInteractables.Any(i => i.Id == id);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            Gizmos.color = Color.cyan;
            foreach (var interactable in detectableInteractables)
            {
                Gizmos.DrawLine(transform.position, interactable.Position);
            }
        }
    }
}
