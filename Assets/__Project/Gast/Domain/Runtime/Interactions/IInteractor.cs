using UnityEngine;

namespace Gast.Domain.Interactions
{
    public interface IInteractor
    {
        Vector3 InteractionPoint { get; }
    }
}