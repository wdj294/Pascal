//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// http://www.digitalruby.com
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRubyShared
{
    [System.Serializable]
    public class GestureRecognizerComponentEvent : UnityEngine.Events.UnityEvent<GestureRecognizer, ICollection<GestureTouch>> { }

    public abstract class GestureRecognizerComponentScriptBase : MonoBehaviour
    {
        public GestureRecognizer GestureBase { get; protected set; }
    }

    public abstract class GestureRecognizerComponentScript<T> : GestureRecognizerComponentScriptBase where T : GestureRecognizer, new()
    {
        [Header("Gesture properties")]
        [Tooltip("Gesture callback")]
        public GestureRecognizerComponentEvent GestureCallback;

        [Tooltip("The game object the gesture must execute over, null to allow the gesture to execute anywhere.")]
        public GameObject GestureView;

        [Tooltip("The minimum number of touches to track. This gesture will not start unless this many touches are tracked. Default is usually 1 or 2. Not all gestures will honor values higher than 1.")]
        [Range(1, 10)]
        public int MinimumNumberOfTouchesToTrack = 1;

        [Tooltip("The maximum number of touches to track. This gesture will never track more touches than this. Default is usually 1 or 2. Not all gestures will honor values higher than 1.")]
        [Range(1, 10)]
        public int MaximumNumberOfTouchesToTrack = 1;

        [Tooltip("Gesture components to allow simultaneous execution with. By default, gestures cannot execute together.")]
        public List<GestureRecognizerComponentScriptBase> AllowSimultaneousExecutionWith;

        [Tooltip("Whether to allow the gesture to execute simultaneously with all other gestures.")]
        public bool AllowSimultaneousExecutionWithAllGestures;

        public T Gesture { get; private set; }

        protected virtual void GestureUpdated(GestureRecognizer gesture, ICollection<GestureTouch> touches)
        {
            if (GestureCallback != null)
            {
                GestureCallback.Invoke(gesture, touches);
            }
        }

        protected virtual void Awake()
        {
            Gesture = new T();
            GestureBase = Gesture;
        }

        protected virtual void Start()
        {
            Gesture.Updated += GestureUpdated;
            Gesture.PlatformSpecificView = GestureView;
            Gesture.MinimumNumberOfTouchesToTrack = MinimumNumberOfTouchesToTrack;
            Gesture.MaximumNumberOfTouchesToTrack = MaximumNumberOfTouchesToTrack;
            if (AllowSimultaneousExecutionWithAllGestures)
            {
                Gesture.AllowSimultaneousExecutionWithAllGestures();
            }
            else if (AllowSimultaneousExecutionWith != null)
            {
                foreach (GestureRecognizerComponentScriptBase gesture in AllowSimultaneousExecutionWith)
                {
                    Gesture.AllowSimultaneousExecution(gesture.GestureBase);
                }
            }
            FingersScript.Instance.AddGesture(Gesture);
        }

        protected virtual void Update()
        {

        }

        protected virtual void LateUpdate()
        {

        }

        protected virtual void OnEnable()
        {
            if (FingersScript.HasInstance)
            {
                FingersScript.Instance.AddGesture(Gesture);
            }
        }

        protected virtual void OnDisable()
        {
            if (FingersScript.HasInstance)
            {
                FingersScript.Instance.RemoveGesture(Gesture);
            }
        }

        protected virtual void OnDestroy()
        {
        }
    }
}