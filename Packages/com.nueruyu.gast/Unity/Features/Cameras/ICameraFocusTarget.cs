using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Features.Cameras
{
    public interface ICameraFocusTarget : ICharacterFacet
    {
        Transform FocusTransform { get; }
    }
}