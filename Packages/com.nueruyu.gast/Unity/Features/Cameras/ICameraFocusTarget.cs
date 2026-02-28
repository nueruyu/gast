using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Unity.Features.Cameras
{
    public interface ICameraFocusTarget : ICharacterFacet
    {
        Transform FocusTransform { get; }
    }
}