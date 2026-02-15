using System.Collections.Generic;
using UnityEngine;

namespace Gast.Domain.Characters
{
    public interface IVisionSensor
    {
        IReadOnlyList<ICharacter> VisibleCharacters { get; }

        bool IsVisible(ICharacter target);

        Vector3 EyePosition { get; }
    }
}