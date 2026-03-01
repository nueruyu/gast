namespace Gast.Unity.Shared.Animations
{
    /// <summary>
    /// Interface for receiving animation events from StateMachineBehaviour.
    /// Implement this interface on components that need to respond to animation timing events.
    /// </summary>
    public interface IAnimationEventReceiver
    {
        /// <summary>
        /// Called when an animation event is triggered by the StateMachineBehaviour.
        /// </summary>
        /// <param name="eventSymbol">Event identifier from the animation state machine</param>
        void TriggerAnimationEvent(AnimationEventSymbol eventSymbol);
    }
}
