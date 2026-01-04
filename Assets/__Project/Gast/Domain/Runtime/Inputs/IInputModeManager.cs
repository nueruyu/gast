using Gast.Core.Observables;

namespace Gast.Domain.Inputs
{
    /// <summary>
    /// Manages the application's current input mode.
    /// </summary>
    public interface IInputModeManager
    {
        /// <summary>
        /// Gets an observable property for the current input mode.
        /// </summary>
        ILive<InputMode> CurrentMode { get; }

        /// <summary>
        /// Sets the current input mode.
        /// </summary>
        /// <param name="mode">The new input mode to set.</param>
        void SetMode(InputMode mode);
    }
}