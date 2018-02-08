// Copyright (c) 2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using UnityEngine;
using Ez.Binding.Vars;

namespace Ez.Binding.Internal
{
    public class EzBindAddSource : MonoBehaviour
    {
        /// Inspector Data.
        public string bindName;
        public BoundItem source;

        private Bind bind = null;
        private ReferencedVariable sourceVar = null;

        private void Start()
        {
            if (string.IsNullOrEmpty(bindName))
            {
                // Bind does not have a name
                Debug.LogError(@"[EzBind] Add Source on the """ + gameObject.name + @""" GameObject must have a bind name assigned! Component is destroying itself.");
                Destroy(this);
                return;
            }

            if (source.gameObject != gameObject || source.component == null || string.IsNullOrEmpty(source.variableName) || source.Equals("None"))
            {
                // Bind was not configured properly
                Debug.LogError(@"[EzBind] Add Source on the """ + gameObject.name + @""" GameObject was not configured correctly in the inspector. Component is destroying itself.");
                Destroy(this);
                return;
            }
            StartCoroutine(WaitOneFrameAndAddSource());
        }

        private IEnumerator WaitOneFrameAndAddSource()
        {
            yield return null;
            AddSelfSource();
        }

        private void AddSelfSource()
        {
            bind = EzBind.FindBindByName(bindName);
            if (bind == null)
            {
                bind = EzBind.AddBind(bindName);
            }

            sourceVar = EzBind.AddSourceToBind(bind, source.component, source.variableName.Split(' ')[1]);
            if (sourceVar == null)
            {
                Debug.LogError(@"[EzBind] Add Source on the """ + gameObject.name + @""" GameObject could not attach a new source to bind named """ + bindName + @"""");
                Destroy(this);
                return;
            }
        }

    }
}