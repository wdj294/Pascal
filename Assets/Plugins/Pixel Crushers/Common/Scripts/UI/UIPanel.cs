// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers
{

    /// <summary>
    /// Manages a UI panel. When the panel is active and on top, it ensures that one of 
    /// its Selectables is selected if using joystick or keyboard.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UIPanel : MonoBehaviour
    {

        [Tooltip("When enabling the panel, select this if input device is Joystick or Keyboard.")]
        public GameObject firstSelected;

        [Tooltip("Seconds between checks to ensure that one of the panel's Selectables is focused.")]
        public float focusCheckFrequency = 0.2f;

        [Tooltip("If non-zero, refresh list of Selectables at this frequency. Use if Selectables are added dynamically.")]
        public float refreshSelectablesFrequency = 0;

        [Tooltip("When opening, set this animator trigger.")]
        public string showAnimationTrigger = "Show";

        [Tooltip("When closing, set this animator trigger.")]
        public string hideAnimationTrigger = "Hide";

        public enum StartState { GameObjectState, Open, Closed }

        [Tooltip("Normally the panel starts flagged open if the GameObject is active. To explicitly specify whether the panel is open or closed, select from the dropdown.")]
        public StartState startState = StartState.GameObjectState;

        public UnityEvent onOpen = new UnityEvent();
        public UnityEvent onClose = new UnityEvent();
        public UnityEvent onBackButtonDown = new UnityEvent();

        private GameObject m_previousSelected = null;
        private List<GameObject> selectables = new List<GameObject>();
        private float m_timeNextCheck = 0;
        private float m_timeNextRefresh = 0;
        private UIAnimatorMonitor m_animatorMonitor = null;
        private bool m_isOpen = false;

        private static List<UIPanel> panelStack = new List<UIPanel>();

        private static UIPanel topPanel
        {
            get { return (panelStack.Count > 0) ? panelStack[panelStack.Count - 1] : null; }
        }

        public bool isOpen
        {
            get { return gameObject.activeInHierarchy; }
        }

        private void Awake()
        {
            m_isOpen = GetStartingOpenState();
            gameObject.SetActive(m_isOpen);
            RefreshSelectablesList();
            m_animatorMonitor = new UIAnimatorMonitor(gameObject);
        }

        private void Start()
        {
            if (startState == StartState.Open || (startState == StartState.GameObjectState && gameObject.activeInHierarchy))
            {
                gameObject.SetActive(true);
                m_animatorMonitor.SetTrigger(showAnimationTrigger, OnVisible, false);
            }
        }

        private bool GetStartingOpenState()
        {
            switch (startState)
            {
                case StartState.Open:
                    return true;
                case StartState.Closed:
                    return false;
                default:
                    return gameObject.activeInHierarchy;

            }
        }

        public void RefreshSelectablesList()
        {
            selectables.Clear();
            foreach (var selectable in GetComponentsInChildren<UnityEngine.UI.Selectable>())
            {
                if (selectable.IsActive() && selectable.IsInteractable())
                {
                    selectables.Add(selectable.gameObject);
                }
            }
        }

        public void RefreshAfterOneFrame()
        {
            StartCoroutine(RefreshAfterOneFrameCoroutine());
        }

        private IEnumerator RefreshAfterOneFrameCoroutine()
        {
            yield return null;
            RefreshSelectablesList();
        }

        private void OnEnable()
        {
            panelStack.Add(this);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            if (InputDeviceManager.autoFocus && UnityEngine.EventSystems.EventSystem.current != null && !selectables.Contains(m_previousSelected))
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_previousSelected);
            }
            panelStack.Remove(this);
        }

        public void Open()
        {
            if (m_isOpen) return;
            gameObject.SetActive(true);
            onOpen.Invoke();
            m_animatorMonitor.SetTrigger(showAnimationTrigger, OnVisible, false);
        }

        public void Close()
        {
            CancelInvoke();
            if (!m_isOpen) return;
            m_isOpen = false;
            onClose.Invoke();
            m_animatorMonitor.SetTrigger(hideAnimationTrigger, OnHidden, true);

            // Deselect ours:
            if (selectables.Contains(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject))
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            }
        }

        public void Toggle()
        {
            if (m_isOpen) Close(); else Open();
        }

        private void OnVisible()
        {
            m_isOpen = true;
            RefreshSelectablesList();

            // Deselect the previous selection if it's not ours:
            m_previousSelected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            if (m_previousSelected != null && !selectables.Contains(m_previousSelected))
            {
                var previousSelectable = m_previousSelected.GetComponent<UnityEngine.UI.Selectable>();
                if (previousSelectable != null) previousSelectable.OnDeselect(null);
            }
        }

        private void OnHidden()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!(m_isOpen && topPanel == this)) return;
            if (InputDeviceManager.isBackButtonDown)
            {
                onBackButtonDown.Invoke();
            }
            else
            {
                if (Time.realtimeSinceStartup >= m_timeNextCheck && topPanel == this && InputDeviceManager.autoFocus)
                {
                    m_timeNextCheck = Time.realtimeSinceStartup + focusCheckFrequency;
                    CheckFocus();
                }
                if (Time.realtimeSinceStartup >= m_timeNextRefresh && refreshSelectablesFrequency > 0 && topPanel == this && InputDeviceManager.autoFocus)
                {
                    m_timeNextRefresh = Time.realtimeSinceStartup + refreshSelectablesFrequency;
                    RefreshSelectablesList();
                }
            }
        }

        public void CheckFocus()
        {
            if (!InputDeviceManager.autoFocus) return;
            if (topPanel != this) return;
            var currentSelected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            if (currentSelected == null || !selectables.Contains(currentSelected))
            {
                var firstSelectable = (firstSelected != null) ? firstSelected.GetComponent<UnityEngine.UI.Selectable>() : null;
                var firstInteractive = firstSelectable != null && firstSelectable.IsActive() && firstSelectable.IsInteractable();
                var selectableToFocus = firstInteractive ? firstSelected : GetFirstInteractableButton();
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(selectableToFocus);
            }
        }

        private GameObject GetFirstInteractableButton()
        {
            foreach (var selectable in GetComponentsInChildren<UnityEngine.UI.Selectable>())
            {
                if (selectable.interactable) return selectable.gameObject;
            }
            return null;
        }

    }

}