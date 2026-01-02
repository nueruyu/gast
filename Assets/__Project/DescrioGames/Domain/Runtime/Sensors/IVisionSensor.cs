using System.Collections.Generic;
using DescrioGames.Domain.Characters;
using UnityEngine;

namespace DescrioGames.Domain.Sensors
{
    public interface IVisionSensor
    {
        IReadOnlyList<ICharacter> VisibleCharacters { get; }
        bool IsVisible(ICharacter target);
        Vector3 EyePosition { get; }
    }
}
