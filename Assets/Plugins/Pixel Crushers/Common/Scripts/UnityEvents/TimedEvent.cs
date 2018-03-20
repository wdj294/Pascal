// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Events;

namespace PixelCrushers
{

    /// <summary>
    /// Invokes an event after a specified duration. The timer can be configured to
    /// start when the script starts or manually.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class TimedEvent : MonoBehaviour
    {

        [Tooltip("After starting timer, wait this many seconds before firing event.")]
        [SerializeField]
        private float m_duration = 0;

        [Tooltip("Start timer when this component starts.")]
        [SerializeField]
        private bool m_activateOnStart = false;

        [SerializeField]
        private UnityEvent m_onTimeReached = new UnityEvent();

        /// <summary>
        /// After starting timer, wait this many seconds before firing event.
        /// </summary>
        public float duration
        {
            get { return m_duration; }
            set { m_duration = value; }
        }

        /// <summary>
        /// Start timer when this component starts.
        /// </summary>
        public bool activateOnStart
        {
            get { return m_activateOnStart; }
            set { m_activateOnStart = value; }
        }

        private UnityEvent onTimeReached
        {
            get { return m_onTimeReached; }
            set { m_onTimeReached = value; }
        }

        protected virtual void Start()
        {
            if (activateOnStart) StartTimer(duration);
        }

        protected virtual void OnDisable()
        {
            CancelTimer();
        }

        /// <summary>
        /// Starts the timer manually.
        /// </summary>
        public virtual void StartTimer()
        {
            StartTimer(duration);
        }

        /// <summary>
        /// Starts the timer manually with a specified duration.
        /// </summary>
        /// <param name="duration">Duration to wait before invoking the event.</param>
        public virtual void StartTimer(float duration)
        {
            Invoke("TimeReached", duration);
        }

        /// <summary>
        /// Cancels the timer's current countdown.
        /// </summary>
        protected virtual void CancelTimer()
        {
            CancelInvoke("TimeReached");
        }

        protected virtual void TimeReached()
        {
            onTimeReached.Invoke();
        }

        public void DestroyUnityObject(UnityEngine.Object o)
        {
            Destroy(o);
        }

    }

}