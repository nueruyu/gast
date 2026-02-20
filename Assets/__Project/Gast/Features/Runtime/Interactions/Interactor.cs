using Gast.Domain.Interactions;
using UnityEngine;

namespace Gast.Features.Interactions
{
    public class Interactor : MonoBehaviour, IInteractor
    {
        public Vector3 InteractionPoint => transform.position;
    }
}