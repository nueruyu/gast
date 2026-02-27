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
        /// <param name="eventName">Event identifier (e.g., "Footstep", "Attack", "WeaponSwing")</param>
        void TriggerAnimationEvent(string eventName);
    }
}