using Gast.Core.Observables;
using UnityEngine;

namespace Gast.Unity.Shared.Animations
{
    /// <summary>
    /// Receives animation events from StateMachineBehaviours and exposes them as C# events.
    /// This component bridges the animation system with gameplay logic for characters.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimationEventReceiver : MonoBehaviour, IAnimationEventReceiver
    {
        readonly Signal<string> eventReceived = new();

        public ISignal<string> EventReceived => eventReceived;

        /// <summary>
        /// Called by TimedEventBehaviour when an animation event is triggered.
        /// Routes the event to appropriate C# event handlers based on event name.
        /// </summary>
        /// <param name="eventName">Event identifier from the animation state machine</param>
        public void TriggerAnimationEvent(string eventName)
        {
            eventReceived.Publish(eventName);
        }
    }
}