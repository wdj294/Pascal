// Copyright (c) 2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Ez.Binding.Vars;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ez.Binding
{
    public class EzBind : Ez.Singleton<EzBind>
    {
        protected EzBind() { }
        private void Awake()
        {
            if(Instance != this) { Destroy(gameObject); }
            CreateBinds();
        }
        /// <summary>
        /// Event invoked on (late) update to trigger all listening binds to check if the source has changed.
        /// </summary>
        protected UnityEvent onUpdate = new UnityEvent();
        /// <summary>
        /// Dinctionary holding all the binds, using their names as key.
        /// </summary>
        protected Dictionary<string, Bind> bindsHolder = new Dictionary<string, Bind>();
        public Dictionary<string, Bind> BindsHolder { get { return bindsHolder; } }

        #region STATIC UTILS
        /// <summary>
        /// Creates a new bind with the specified name and returns it.
        /// <para>If the operation fails, returns null.</para>
        /// </summary>
        /// <param name="newBindName">The bind's name. It must be unique among all binds.</param>
        /// <returns></returns>
        public static Bind AddBind(string newBindName)
        {
            var bind = new Bind(newBindName);
            return AddBind(bind) ? bind : null;
        }
        /// <summary>
        /// Creates a new bind with the specified name and having the specified initial value, then returns it.
        /// <para>If the operation fails, returns null.</para>
        /// </summary>
        /// <param name="newBindName">The bind's name. It must be unique among all binds.</param>
        /// <param name="newBindValue">The bind's initial value.</param>
        /// <returns></returns>
        public static Bind AddBind(string newBindName, object newBindValue)
        {
            var bind = new Bind(newBindName, newBindValue);
            return AddBind(bind) ? bind : null;
        }
        /// <summary>
        /// Creates a new bind with the specified name and returns it.
        /// <para>If the operation fails, returns null.</para>
        /// </summary>
        /// <param name="newBindName">The bind's name. It must be unique among all binds.</param>
        /// <param name="onValueChanged">Event invoked when the bind's value changes.</param>
        /// <returns></returns>
        public static Bind AddBind(string newBindName, UnityEvent onValueChanged)
        {
            var bind = new Bind(newBindName, onValueChanged);
            return AddBind(bind) ? bind : null;
        }
        /// <summary>
        /// Creates a new bind with the specified name and having the specified initial value, then returns it.
        /// <para>If the operation fails, returns null.</para>
        /// </summary>
        /// <param name="newBindName">The bind's name. It must be unique among all binds.</param>
        /// <param name="newBindValue">The bind's initial value.</param>
        /// <param name="onValueChanged">Event invoked when the bind's value changes.</param>
        /// <returns></returns>
        public static Bind AddBind(string newBindName, object newBindValue, UnityEvent onValueChanged)
        {
            var bind = new Bind(newBindName, newBindValue, onValueChanged);
            return AddBind(bind) ? bind : null;
        }
        /// <summary>
        /// Private method that adds the bind to the dictionary.
        /// <para>Returns true if the operation is successful, false otherwise.</para>
        /// </summary>
        /// <param name="newBind">Bind to be added</param>
        /// <returns>Returns <c>true</c> if the operation is successful, <c>false</c> otherwise</returns>
        private static bool AddBind(Bind newBind)
        {
            try
            {
                EzBind.Instance.bindsHolder.Add(newBind.name, newBind);
            }
            catch(Exception e)
            {
                Debug.LogError(@"[EzBind] - Error when adding a new bind named """ + newBind.name + @""" : " + e.Message);
                return false;
            }
            return true;
        }
        /// <summary>
        /// Removes the bind specified by name and destroys it.
        /// </summary>
        /// <param name="bindName">Bind's name.</param>
        public static void RemoveBind(string bindName)
        {
            Bind var;
            if(EzBind.Instance.bindsHolder.TryGetValue(bindName, out var))
            {
                EzBind.Instance.bindsHolder.Remove(bindName);
                var.Dispose();
            }
        }
        /// <summary>
        /// Removes the bind specified by reference and destroys it.
        /// </summary>
        /// <param name="bind">Bind's reference.</param>
        public static void RemoveBind(Bind bind)
        {
            Bind var;
            if(EzBind.Instance.bindsHolder.TryGetValue(bind.name, out var))
            {
                EzBind.Instance.bindsHolder.Remove(bind.name);
                var.Dispose();
            }
        }
        /// <summary>
        /// Adds a new observer to the referenced bind and returns a reference to the new observer.
        /// <para>If unsuccessful, returns null.</para>
        /// </summary>
        /// <param name="bind">Bind reference.</param>
        /// <param name="parent">Observer's Component.</param>
        /// <param name="observerName">Observer's name (field or property name)</param>
        /// <returns></returns>
        public static ReferencedVariable AddObserverToBind(Bind bind, UnityEngine.Object parent, string observerName)
        {
            if(bind == null)
            {
                return null;
            }
            var refVar = new ReferencedVariable(bind.name, parent, observerName);
            return bind.AddObserver(refVar) ? refVar : null;
        }
        /// <summary>
        ///  Adds a new observer to the specified bind and returns a reference to the new observer.
        ///  <para>If unsuccessful, returns null.</para>
        /// </summary>
        /// <param name="bindName">Bind's name.</param>
        /// <param name="parent">Observer's Component.</param>
        /// <param name="observerName">Observer's name (field or property name)</param>
        /// <returns></returns>
        public static ReferencedVariable AddObserverToBind(string bindName, UnityEngine.Object parent, string observerName)
        {
            var bind = FindBindByName(bindName);
            return bind != null ? AddObserverToBind(bind, parent, observerName) : null;
        }
        /// <summary>
        /// Add a new observer to the referenced bind. The observer is a reference to a ReferencedVariable previously created.
        /// <para>Returns true if the operation was successful, false otherwise.</para>
        /// </summary>
        /// <param name="bind">Bind reference.</param>
        /// <param name="observerVar">Observer reference.</param>
        /// <returns></returns>
        public static bool AddObserverToBind(Bind bind, ReferencedVariable observerVar)
        {
            if(bind == null)
            {
                return false;
            }
            return bind.AddObserver(observerVar);
        }
        /// <summary>
        /// Add a new observer to the specified bind. The observer is a reference to a ReferencedVariable previously created.
        /// <para>Returns true if the operation was successful, false otherwise.</para>
        /// </summary>
        /// <param name="bindName">Bind's name.</param>
        /// <param name="observerVar">Observer reference.</param>
        /// <returns></returns>
        public static bool AddObserverToBind(string bindName, ReferencedVariable observerVar)
        {
            var bind = FindBindByName(bindName);
            return bind != null ? AddObserverToBind(bind, observerVar) : false;
        }
        /// <summary>
        /// Adds a new source to the referenced bind and returns a reference to the new source. 
        /// <para>If another source was defined, it is replaced. If unsuccessful, returns null.</para>
        /// </summary>
        /// <param name="bind">Bind reference.</param>
        /// <param name="parent">Source's Component.</param>
        /// <param name="sourceName">Source's name (field or property name)</param>
        /// <returns></returns>
        public static ReferencedVariable AddSourceToBind(Bind bind, UnityEngine.Object parent, string sourceName)
        {
            if(bind == null)
            {
                return null;
            }
            var refVar = new ReferencedVariable(bind.name, parent, sourceName);
            return bind.AddSourceVariable(refVar) ? refVar : null;
        }
        /// <summary>
        /// Adds a new source to the specified bind and returns a reference to the new source. 
        /// <para>If another source was defined, it is replaced. If unsuccessful, returns null.</para>
        /// </summary>
        /// <param name="bindName">Bind's name.</param>
        /// <param name="parent">Source's Component.</param>
        /// <param name="observerName">Source's name (field or property name)</param>
        /// <returns></returns>
        public static ReferencedVariable AddSourceToBind(string bindName, UnityEngine.Object parent, string observerName)
        {
            var bind = FindBindByName(bindName);
            return bind != null ? AddSourceToBind(bind, parent, observerName) : null;
        }
        /// <summary>
        /// Adds a new source to the referenced bind. The source is a reference to a ReferenceVariable previously created. 
        /// <para>If another source was defined, it is replaced. Returns true if successful, false otherwise.</para>
        /// </summary>
        /// <param name="bind">Bind reference.</param>
        /// <param name="refVar">Source reference.</param>
        /// <returns></returns>
        public static bool AddSourceToBind(Bind bind, ReferencedVariable sourceVar)
        {
            if(bind == null)
            {
                return false;
            }
            return bind.AddSourceVariable(sourceVar);
        }
        /// <summary>
        /// Adds a new source to the specified bind. The source is a reference to a ReferenceVariable previously created.
        /// <para>If another source was defined, it is replaced. Returns true if successful, false otherwise.</para>
        /// </summary>
        /// <param name="bindName">Bind's name.</param>
        /// <param name="sourceVar">Source reference.</param>
        /// <returns></returns>
        public static bool AddSourceToBind(string bindName, ReferencedVariable sourceVar)
        {
            var bind = FindBindByName(bindName);
            return bind != null ? AddSourceToBind(bind, sourceVar) : false;
        }
        /// <summary>
        /// Finds and returns an existing bind by it's name. 
        /// <para>If the bind doesn't exist, returns null.</para>
        /// </summary>
        /// <param name="bindName">The bind's name.</param>
        /// <returns>The bind, if found, <c>null</c> otherwise.</returns>
        public static Bind FindBindByName(string bindName)
        {
            Bind var;
            return EzBind.Instance.bindsHolder.TryGetValue(bindName, out var) ? var : null;
        }
        /// <summary>
        /// Removes an observer from the bind.
        /// <para>Returns true if successful, false otherwise.</para>
        /// </summary>
        /// <param name="bind">Bind reference.</param>
        /// <param name="refVar">Observer reference.</param>
        /// <returns>Returns <c>true</c> if the operation was successful, <c>false</c> otherwise.</returns>
        public static bool RemoveObserverFromBind(Bind bind, ReferencedVariable refVar)
        {
            return (bind != null && refVar != null) ? bind.RemoveObserver(refVar) : false;
        }
        /// <summary>
        /// Removes an observer from the bind.
        /// <para>Returns true if successful, false otherwise.</para>
        /// </summary>
        /// <param name="bindName">The bind's name.</param>
        /// <param name="refVar">Observer reference.</param>
        /// <returns>Returns <c>true</c> if the operation was successful, <c>false</c> otherwise.</returns>
        public static bool RemoveObserverFromBind(string bindName, ReferencedVariable refVar)
        {
            return RemoveObserverFromBind(FindBindByName(bindName), refVar);
        }
        /// <summary>
        /// Adds a listener method to a bind. 
        /// </summary>
        /// <param name="bind">Bind reference.</param>
        /// <param name="listener">Listener method.</param>
        public static void AddListenerToBind(Bind bind, UnityAction listener)
        {
            bind.AddListener(listener);
        }
        /// <summary>
        /// Adds a listener method to a bind.
        /// <para> Returns true if successful, false otherwise.</para>
        /// </summary>
        /// <param name="bindName">The bind's name.</param>
        /// <param name="listener">Listener method.</param>
        /// <returns>Returns <c>true</c> if successful, <c>false</c> otherwise.</returns>
        public static bool AddListenerToBind(string bindName, UnityAction listener)
        {
            var bind = FindBindByName(bindName);
            if(bind != null)
            {
                AddListenerToBind(bind, listener);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Removes the listener from the bind.
        /// </summary>
        /// <param name="bind">Bind reference.</param>
        /// <param name="listener">Listener method.</param>
        public static void RemoveListenerFromBind(Bind bind, UnityAction listener)
        {
            bind.RemoveListener(listener);
        }
        /// <summary>
        /// Removes the listener method from the bind.
        /// <para> Returns true if successful, false otherwise.</para>
        /// </summary>
        /// <param name="bindName">The bind's name.</param>
        /// <param name="listener">Listener method.</param>
        /// <returns></returns>
        public static bool RemoveListenerFromBind(string bindName, UnityAction listener)
        {
            var bind = FindBindByName(bindName);
            if(bind != null)
            {
                RemoveListenerFromBind(bind, listener);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Helper method to find a variable's name at runtime. The variable MUST be passed as an annonymous type!
        /// <para>Usage example: var myVariable; EzBind.GetObserverName(new { myVariable });</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetReferenceName<T>(T item)
        {
            if(item == null)
            {
                Debug.LogError("[EzBind] You are trying to get an observer name for a null reference! Empty string is passed instead.");
                return string.Empty;
            }

            if(!IsAnonymousType(typeof(T)))
            {
                Debug.LogError("[EzBind] You are trying to get an observer name for a non-anonymous type! " +
                    " To find the name, you muse pass an anonymous type as a parameter like this: GetTargetName(new { <target> });");
                return string.Empty;
            }
            return typeof(T).GetProperties()[0].Name;
        }
        /// <summary>
        /// Checks if the given type is Anonymous or not.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsAnonymousType(Type type)
        {
            return type == null ?
                false :
                Attribute.IsDefined(type, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("Anon")
                && (type.Name.StartsWith("<>", StringComparison.OrdinalIgnoreCase)
                    || type.Name.StartsWith("VB$", StringComparison.OrdinalIgnoreCase))
                && (type.Attributes & System.Reflection.TypeAttributes.NotPublic) == System.Reflection.TypeAttributes.NotPublic;
        }
        #endregion //STATIC UTILS
        /// <summary>
        /// Helper method allowing binds to subscribe to the onUpdate event.
        /// </summary>
        /// <param name="listener">Listener method.</param>
        public void SubscribeToUpdates(UnityAction listener)
        {
            onUpdate.AddListener(listener);
        }
        /// <summary>
        /// Helper method allowing binds to subscribe to the onUpdate event.
        /// </summary>
        /// <param name="listener">Listener method.</param>
        public void UnsubscribeFromUpdates(UnityAction listener)
        {
            onUpdate.RemoveListener(listener);
        }

        private void LateUpdate()
        {
            onUpdate.Invoke();
        }

        public List<BindData> bindsData;

        private void CreateBinds()
        {
            Bind tempBind;
            if(bindsData == null || bindsData.Count == 0) { return; }
            for(int i = 0; i < bindsData.Count; i++)
            {
                if(string.IsNullOrEmpty(bindsData[i].bindName)) { continue; }
                tempBind = EzBind.AddBind(bindsData[i].bindName,bindsData[i].OnValueChanged); //Create Bind

                if(tempBind == null) { continue; }

                if(bindsData[i].source.gameObject != null &&
                    bindsData[i].source.component != null &&
                    !string.IsNullOrEmpty(bindsData[i].source.variableName))
                {
                    EzBind.AddSourceToBind(tempBind, bindsData[i].source.component, bindsData[i].source.variableName.Split(' ')[1]); //Add Source
                }
                if(bindsData[i].observers != null && bindsData[i].observers.Count > 0)
                {
                    for(int j = 0; j < bindsData[i].observers.Count; j++)
                    {
                        if(bindsData[i].observers[j].gameObject != null &&
                            bindsData[i].observers[j].component != null &&
                            !string.IsNullOrEmpty(bindsData[i].observers[j].variableName)&&
                            !bindsData[i].observers[j].variableName.Equals("None"))
                        {
                            EzBind.AddObserverToBind(tempBind, bindsData[i].observers[j].component, bindsData[i].observers[j].variableName.Split(' ')[1]); //Add Observers
                        }
                    }
                }

            }
        }
    }

    [Serializable]
    public class BindData
    {
        public string bindName = string.Empty;
        public BoundItem source = new BoundItem();
        public List<BoundItem> observers = new List<BoundItem>();
        public UnityEvent OnValueChanged = new UnityEvent();
        public void InvokeOwnEvent()
        {
            OnValueChanged.Invoke();
        }
    }

    [Serializable]
    public class BoundItem
    {
        public GameObject gameObject;
        public Component component;
        public string variableName;

        public BoundItem()
        {
            Reset();
        }

        public BoundItem(GameObject gameObject, Component component, string variableName)
        {
            this.gameObject = gameObject;
            this.component = component;
            this.variableName = variableName;
        }

        public void Reset()
        {
            gameObject = null;
            component = null;
            variableName = "";
        }
    }

}
