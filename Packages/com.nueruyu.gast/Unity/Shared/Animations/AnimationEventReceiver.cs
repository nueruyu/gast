using Gast.Core.Observables;
using UnityEngine;

namespace Gast.Unity.Shared.Animations
{
    [RequireComponent(typeof(Animator))]
    public class AnimationEventReceiver : MonoBehaviour, IAnimationEventReceiver
    {
        readonly Signal<AnimationEventSymbol> eventReceived = new();

        public ISignal<AnimationEventSymbol> EventReceived => eventReceived;

        public void TriggerAnimationEvent(AnimationEventSymbol eventSymbol)
        {
            if (eventSymbol != null)
            {
                eventReceived.Publish(eventSymbol);
            }
        }
    }
}
