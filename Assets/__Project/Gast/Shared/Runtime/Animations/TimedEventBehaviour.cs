using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gast.Shared.Animations
{
    /// <summary>
    /// StateMachineBehaviour that triggers events at specified timings during animation playback.
    /// Supports both normalized time (0.0~1.0) and real-time (seconds) modes.
    /// Handles looping animations correctly by tracking state across frames.
    /// </summary>
    public class TimedEventBehaviour : StateMachineBehaviour
    {
        /// <summary>
        /// Timing specification mode for trigger times.
        /// </summary>
        public enum TimingMode
        {
            /// <summary>Specify timing as normalized time (0.0 ~ 1.0 range)</summary>
            [Tooltip("Specify timing as normalized time (0.0 ~ 1.0 range)")]
            NormalizedTime,

            /// <summary>Specify timing in seconds from animation start</summary>
            [Tooltip("Specify timing in seconds from animation start")]
            RealTime
        }

        /// <summary>
        /// Pairs timing with event name for flexible event triggering.
        /// </summary>
        [Serializable]
        struct TimedEvent
        {
            [Tooltip("Timing value (0.0~1.0 for NormalizedTime, or seconds for RealTime)")]
            public float time;

            [Tooltip("Event name to trigger at this timing")]
            public string eventName;
        }

        /// <summary>
        /// Tracks state per animator instance.
        /// </summary>
        struct StateData
        {
            public float LastTime;
            public bool IsFirstUpdate;
        }

        [Header("Settings")]
        [Tooltip("Is this a looping animation? Affects how time wrapping is handled.")]
        [SerializeField] bool isLooping = false;

        [SerializeField] TimingMode mode = TimingMode.NormalizedTime;

        [Tooltip("Animation events with timing and event name pairs")]
        [SerializeField] TimedEvent[] events;

        /// <summary>
        /// Tracks the state for each animator instance.
        /// Key: Animator InstanceID, Value: State Data
        /// </summary>
        readonly Dictionary<int, StateData> states = new Dictionary<int, StateData>();

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Initialize state with current time and set first update flag
            states[animator.GetInstanceID()] = new StateData
            {
                LastTime = stateInfo.normalizedTime,
                IsFirstUpdate = true
            };
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (events == null || events.Length == 0)
                return;

            var id = animator.GetInstanceID();
            if (!states.TryGetValue(id, out StateData data))
            {
                return;
            }

            var currentTime = stateInfo.normalizedTime;
            var clipLength = stateInfo.length;

            foreach (var timedEvent in events)
            {
                if (string.IsNullOrEmpty(timedEvent.eventName))
                    continue;

                // Convert threshold to normalized time
                var threshold = timedEvent.time;
                if (mode == TimingMode.RealTime)
                {
                    if (clipLength > 0.001f)
                    {
                        threshold = timedEvent.time / clipLength;
                    }
                    else
                    {
                        threshold = 0;
                    }
                }

                var fired = false;

                if (isLooping)
                {
                    // === Looping animation logic ===
                    var looped = (int)currentTime > (int)data.LastTime;

                    if (looped)
                    {
                        // Loop occurred: check [prev%1.0 ~ 1.0] or [0.0 ~ curr%1.0]
                        var prev01 = data.LastTime % 1.0f;
                        var curr01 = currentTime % 1.0f;
                        var thresh01 = threshold % 1.0f;

                        // First update: use >= to catch events at exact start time
                        // Later updates: use > to avoid re-triggering
                        var coveredTail = data.IsFirstUpdate ? (thresh01 >= prev01) : (thresh01 > prev01);
                        var coveredHead = thresh01 <= curr01;

                        if (coveredTail || coveredHead)
                        {
                            fired = true;
                        }
                    }
                    else
                    {
                        // Normal progression within loop
                        var prev01 = data.LastTime % 1.0f;
                        var curr01 = currentTime % 1.0f;
                        var thresh01 = threshold % 1.0f;

                        // First update: >= to catch time 0.0 events
                        // Later updates: > to avoid duplicates
                        var lowerBound = data.IsFirstUpdate ? (prev01 <= thresh01) : (prev01 < thresh01);

                        if (lowerBound && curr01 >= thresh01)
                        {
                            fired = true;
                        }
                    }
                }
                else
                {
                    // === Non-looping (one-shot) animation logic ===
                    // Don't treat time > 1.0 as a loop

                    // First update: >= to catch time 0.0 events
                    // Later updates: > to avoid duplicates
                    var lowerBound = data.IsFirstUpdate ? (data.LastTime <= threshold) : (data.LastTime < threshold);

                    if (lowerBound && currentTime >= threshold)
                    {
                        fired = true;
                    }
                }

                if (fired)
                {
                    NotifyReceiver(animator, timedEvent.eventName);
                }
            }

            // Update state for next frame
            data.LastTime = currentTime;
            data.IsFirstUpdate = false;
            states[id] = data;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            states.Remove(animator.GetInstanceID());
        }

        /// <summary>
        /// Notifies the receiver component of the animation event.
        /// </summary>
        void NotifyReceiver(Animator animator, string eventName)
        {
            // Get receiver via interface
            var receiver = animator.GetComponent<IAnimationEventReceiver>();
            receiver?.TriggerAnimationEvent(eventName);
        }
    }
}