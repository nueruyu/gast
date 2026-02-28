using Gast.Domain.Interactions;
using UnityEngine;

namespace Gast.Unity.Features.Interactions
{
    public class Interactor : MonoBehaviour, IInteractor
    {
        [SerializeField]
        InteractionSensor sensor;

        public Vector3 InteractionPoint => transform.position;
        public IInteractionSensor Sensor => sensor;
    }
}