using System.Collections.Generic;
using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Domain.Sensors
{
    public interface IVisionSensor
    {
        IReadOnlyList<ICharacter> VisibleCharacters { get; }
        bool IsVisible(ICharacter target);
        Vector3 EyePosition { get; }
    }
}
