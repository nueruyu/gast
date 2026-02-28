using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Domain.Interactions
{
    public interface IInteractor : ICharacterFacet
    {
        Vector3 InteractionPoint { get; }
        IInteractionSensor Sensor { get; }
    }
}