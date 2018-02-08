// Copyright (c) 2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Ez.Binding.Vars;
using UnityEngine;
using UnityEngine.Events;

namespace Ez.Binding.Internal
{
    public class EzBindExtension : MonoBehaviour
    {
        public List<BindData> bindsData;

        private List<Bind> bindsCreatedBySelf;
        private List<KeyValuePair<Bind, ReferencedVariable>> refVarsCreatedBySelf;
        private List<KeyValuePair<Bind, UnityAction>> listenersAddedBySelf;
        private bool shouldCleanup = false;

        // Use this for initialization
        void Start()
        {
            bindsCreatedBySelf = new List<Bind>();
            refVarsCreatedBySelf = new List<KeyValuePair<Bind, ReferencedVariable>>();
            listenersAddedBySelf = new List<KeyValuePair<Bind, UnityAction>>();

            AddOwnBinds();
            shouldCleanup = true;
        }

        private void OnDisable() { if (shouldCleanup) { RemoveOwnBinds(); } }
        private void OnApplicationQuit() { shouldCleanup = false; }

        void AddOwnBinds()
        {
            Bind bind;
            ReferencedVariable refVar;

            if (bindsData == null || bindsData.Count == 0) { return; }
            for (int i = 0; i < bindsData.Count; i++)
            {
                if (string.IsNullOrEmpty(bindsData[i].bindName)) { continue; }

                bind = EzBind.FindBindByName(bindsData[i].bindName);

                if (bind == null)
                {
                    // The bind does not exist, it must be created first
                    bind = EzBind.AddBind(bindsData[i].bindName, bindsData[i].OnValueChanged);
                    if (bind == null) { continue; } // should NEVER happen here, but check just in case

                    bindsCreatedBySelf.Add(bind);

                    if (bindsData[i].source.gameObject != null &&
                        bindsData[i].source.component != null &&
                        !string.IsNullOrEmpty(bindsData[i].source.variableName))
                    {
                        refVar = EzBind.AddSourceToBind(bind, bindsData[i].source.component, bindsData[i].source.variableName.Split(' ')[1]); //Add Source
                    }
                    if (bindsData[i].observers != null && bindsData[i].observers.Count > 0)
                    {
                        for (int j = 0; j < bindsData[i].observers.Count; j++)
                        {
                            if (bindsData[i].observers[j].gameObject != null &&
                                bindsData[i].observers[j].component != null &&
                                !string.IsNullOrEmpty(bindsData[i].observers[j].variableName) &&
                                !bindsData[i].observers[j].variableName.Equals("None"))
                            {
                                refVar = EzBind.AddObserverToBind(bind, bindsData[i].observers[j].component, bindsData[i].observers[j].variableName.Split(' ')[1]); //Add Observers
                            }
                        }
                    }
                }
                else
                {
                    // bind !=null, add to it
                    if (bindsData[i].source.gameObject != null &&
                        bindsData[i].source.component != null &&
                        !string.IsNullOrEmpty(bindsData[i].source.variableName))
                    {
                        refVar = EzBind.AddSourceToBind(bind, bindsData[i].source.component, bindsData[i].source.variableName.Split(' ')[1]); //Add Source
                        refVarsCreatedBySelf.Add(new KeyValuePair<Bind, ReferencedVariable>(bind, refVar));
                    }

                    if (bindsData[i].observers != null && bindsData[i].observers.Count > 0)
                    {
                        for (int j = 0; j < bindsData[i].observers.Count; j++)
                        {
                            if (bindsData[i].observers[j].gameObject != null &&
                                bindsData[i].observers[j].component != null &&
                                !string.IsNullOrEmpty(bindsData[i].observers[j].variableName))
                            {
                                refVar = EzBind.AddObserverToBind(bind, bindsData[i].observers[j].component, bindsData[i].observers[j].variableName.Split(' ')[1]); //Add Observers
                                refVarsCreatedBySelf.Add(new KeyValuePair<Bind, ReferencedVariable>(bind, refVar));
                            }
                        }
                    }

                    bind.AddListener(bindsData[i].InvokeOwnEvent);
                    listenersAddedBySelf.Add(new KeyValuePair<Bind, UnityAction>(bind, bindsData[i].InvokeOwnEvent));
                }
            }
        }

        void RemoveOwnBinds()
        {
            int i;
            for (i = 0; i < bindsCreatedBySelf.Count; i++)
            {
                EzBind.RemoveBind(bindsCreatedBySelf[i]);
            }
            bindsCreatedBySelf = null;

            for (i = 0; i < refVarsCreatedBySelf.Count; i++)
            {
                EzBind.RemoveObserverFromBind(refVarsCreatedBySelf[i].Key, refVarsCreatedBySelf[i].Value);
            }
            refVarsCreatedBySelf = null;

            for (i = 0; i < listenersAddedBySelf.Count; i++)
            {
                EzBind.RemoveListenerFromBind(listenersAddedBySelf[i].Key, listenersAddedBySelf[i].Value);
            }
            listenersAddedBySelf = null;

            shouldCleanup = false;
        }
    }
}