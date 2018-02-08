// Copyright (c) 2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
using System.Collections;
using Ez.Binding.Vars;

namespace Ez.Binding.Internal
{
    public class EzBindAddObserver : MonoBehaviour
    {
        /// Inspector Data.
        public string bindName;
        public BoundItem observer;

        // Should the added observer update itself if this GameObject is not enabled?
        public bool updateWhenDisabled = true;

        private Bind bind = null;
        private ReferencedVariable obsVar = null;
        private bool isInitialized = false;

        private void Start()
        {
            if(string.IsNullOrEmpty(bindName))
            {
                Debug.LogError(@"[EzBind] Add Observer on the """ + gameObject.name + @""" GameObject must have a bind name assigned! Component is destroying itself.");
                Destroy(this);
                return;
            }

            if(observer.gameObject != gameObject || observer.component == null || string.IsNullOrEmpty(observer.variableName) || observer.variableName.Equals("None"))
            {
                // Bind was not configured properly
                Debug.LogError(@"[EzBind] Add Observer on the """ + gameObject.name + @""" GameObject was not configured correctly in the inspector. Component is destroying itself.");
                Destroy(this);
                return;
            }
            StartCoroutine(WaitOneFrameAndAddObserver());
        }

        private void OnEnable()
        {
            if(!isInitialized) { return; } // Start() didn't run yet
            if(updateWhenDisabled) { return; } // Only add once, from Start()

            EzBind.AddObserverToBind(bind, obsVar);
        }

        private void OnDisable()
        {
            if(!isInitialized) { return; } // Start() didn't run yet
            if(updateWhenDisabled) { return; } // Always receive updates, do nothing OnDisable() 

            EzBind.RemoveObserverFromBind(bind, obsVar);
        }

        private IEnumerator WaitOneFrameAndAddObserver()
        {
            yield return null;
            AddOwnObserver();
        }

        private void AddOwnObserver()
        {
            bind = EzBind.FindBindByName(bindName);
            if(bind == null)
            {
                bind = EzBind.AddBind(bindName);
            }

            obsVar = EzBind.AddObserverToBind(bind, observer.component, observer.variableName.Split(' ')[1]);
            if(obsVar == null)
            {
                Debug.LogError(@"[EzBind] Add Observer on the """ + gameObject.name + @""" GameObject could not attach a new observer to bind named """ + bindName + @"""");
                Destroy(this);
                return;
            }

            isInitialized = true;
        }
    }
}