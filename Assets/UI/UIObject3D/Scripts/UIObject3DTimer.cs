#region Namespace Imports
using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
#endregion

namespace UI.ThreeDimensional
{
    internal class DelayedEditorAction
    {
        internal double TimeToExecute;
        internal Action Action;
        internal MonoBehaviour ActionTarget;

        public DelayedEditorAction(double timeToExecute, Action action, MonoBehaviour actionTarget)
        {
            TimeToExecute = timeToExecute;
            Action = action;
            ActionTarget = actionTarget;
        }
    }

    public static class UIObject3DTimer
    {
#if UNITY_EDITOR
        static List<DelayedEditorAction> delayedEditorActions = new List<DelayedEditorAction>();

        static UIObject3DTimer()
        {
            //if (!Application.isPlaying) UnityEditor.EditorApplication.update += EditorUpdate;
            UnityEditor.EditorApplication.update += EditorUpdate;
        }
#endif

        static void EditorUpdate()
        {
#if UNITY_EDITOR
            if (Application.isPlaying) return;

            var actionsToExecute = delayedEditorActions.Where(dea => UnityEditor.EditorApplication.timeSinceStartup >= dea.TimeToExecute).ToList();

            if (!actionsToExecute.Any()) return;

            foreach (var actionToExecute in actionsToExecute)
            {
                try
                {
                    if (actionToExecute.ActionTarget != null) // don't execute if the target is gone
                    {
                        actionToExecute.Action.Invoke();
                    }
                }
                finally
                {
                    delayedEditorActions.Remove(actionToExecute);
                }
            }
#endif
        }

        /// <summary>
        /// Call Action 'action' after the specified delay, provided the 'actionTarget' is still present and active in the scene at that time.
        /// Can be used in both edit and play modes.
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="action"></param>
        /// <param name="actionTarget"></param>
        public static void DelayedCall(float delay, Action action, MonoBehaviour actionTarget)
        {
            if (Application.isPlaying)
            {
                if (actionTarget.gameObject.activeInHierarchy) actionTarget.StartCoroutine(_DelayedCall(delay, action));                
            }
#if UNITY_EDITOR
            else
            {
                delayedEditorActions.Add(new DelayedEditorAction(UnityEditor.EditorApplication.timeSinceStartup + delay, action, actionTarget));
            }
#endif
        }

        private static IEnumerator _DelayedCall(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);

            action.Invoke();
        }

        /// <summary>
        /// Shorthand for DelayedCall(0, action, actionTarget)
        /// </summary>
        /// <param name="action"></param>
        /// <param name="actionTarget"></param>
        public static void AtEndOfFrame(Action action, MonoBehaviour actionTarget)
        {
            DelayedCall(0, action, actionTarget);
        }
    }
}
