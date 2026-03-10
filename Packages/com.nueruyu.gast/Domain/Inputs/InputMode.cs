namespace Gast.Domain.Inputs
{
    /// <summary>
    /// Defines the current input handling mode of the application.
    /// </summary>
    public enum InputMode
    {
        /// <summary>
        /// Input is directed to gameplay (character movement, camera control, etc.).
        /// </summary>
        Gameplay,

        /// <summary>
        /// Input is directed to UI elements (menus, dialogs, etc.).
        /// </summary>
        UI,

        /// <summary>
        /// Input is directed to item placement logic.
        /// </summary>
        Placement
    }
}