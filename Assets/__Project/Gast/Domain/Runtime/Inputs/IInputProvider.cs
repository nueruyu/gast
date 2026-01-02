using UnityEngine;

namespace Gast.Domain.Inputs
{
    /// <summary>
    /// Provides input data for gameplay systems.
    /// </summary>
    public interface IInputProvider
    {
        Vector2 Move { get; }
        Vector2 Look { get; }
        bool Jump { get; }
        bool Sprint { get; }
        bool InteractPressed { get; }
        bool InteractHeld { get; }
        bool Attack { get; }
        bool Dash { get; }
        bool GuardHeld { get; }
    }
}