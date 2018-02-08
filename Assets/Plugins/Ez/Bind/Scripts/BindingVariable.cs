// Copyright (c) 2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Ez.Binding.Vars
{
    public class Bind : IDisposable
    {
        public readonly string name;
        protected object _value;
        public object Value
        {
            get { return _value; }
            set
            {
                if(_value == null && value == null) { return; }
                if(_value == null || !_value.Equals(value))
                {
                    _value = value;
                    NotifyAll();
                }
            }
        }
        /// <summary>
        /// Returns true if this Bind has a valid source attached to it.
        /// </summary>
        public bool HasSource
        {
            get { return (sourceVariable != null && !sourceVariable.IsNull()); }
        }

        public int ObserverCount { get { return observers == null ? 0 : observers.Count; } }

        protected ReferencedVariable sourceVariable;
        protected UnityEvent onValueChanged;
        protected List<ReferencedVariable> observers = new List<ReferencedVariable>();

        public Bind(string name)
        {
            this.name = name;
            onValueChanged = new UnityEvent();
        }

        public Bind(string name, object initialValue)
        {
            this.name = name;
            this._value = initialValue;
            onValueChanged = new UnityEvent();
        }

        public Bind(string name, UnityEvent onValueChangedEvent)
        {
            this.name = name;
            this.onValueChanged = onValueChangedEvent != null ? onValueChangedEvent : new UnityEvent();
        }

        public Bind(string name, object initialValue, UnityEvent onValueChangedEvent)
        {
            this.name = name;
            this._value = initialValue;
            this.onValueChanged = onValueChangedEvent != null ? onValueChangedEvent : new UnityEvent();
        }

        public void AddListener(UnityAction listener) { onValueChanged.AddListener(listener); }
        public void RemoveListener(UnityAction listener) { onValueChanged.RemoveListener(listener); }

        public bool AddSourceVariable(ReferencedVariable newSourceVar)
        {
            if(newSourceVar == null || newSourceVar.IsNull())
            {
                RemoveSourceVariable();
                return false;
            }
            this.sourceVariable = newSourceVar;
            this.Value = newSourceVar.Value;
            EzBind.Instance.SubscribeToUpdates(UpdateVariable);
            return true;
        }

        public void RemoveSourceVariable()
        {
            this.sourceVariable = null;
            EzBind.Instance.UnsubscribeFromUpdates(UpdateVariable);
        }

        public bool AddObserver(ReferencedVariable newObserver)
        {
            if(newObserver != null && !newObserver.IsNull() && !observers.Contains(newObserver))
            {
                observers.Add(newObserver);
                newObserver.Value = this._value;
                return true;
            }
            return false;
        }

        public bool RemoveObserver(ReferencedVariable observer)
        {
            int index = observers.IndexOf(observer);
            if(index >= 0 && index < observers.Count)
            {
                observers.RemoveAt(index);
                return true;
            }
            return false;
        }

        protected void NotifyAll()
        {
            if(this.HasSource)
            {
                sourceVariable.Value = this._value;
            }

            for(int i = 0; i < observers.Count; i++)
            {
                if(observers[i].IsNull())
                {
                    observers[i] = null;
                    observers.RemoveAt(i);
                    i--;
                }
                else
                {
                    observers[i].Value = this._value;
                }
            }
            onValueChanged.Invoke();
        }

        public void UpdateVariable()
        {
            if(!this.HasSource)
            {
                RemoveSourceVariable();
                return;
            }

            if(sourceVariable.Value == null || !(sourceVariable.Value).Equals(_value))
            {
                Value = sourceVariable.Value;
            }
        }




        public void Dispose()
        {
            sourceVariable = null;
            onValueChanged.RemoveAllListeners();
            while(observers.Count > 0)
            {
                observers[0] = null;
                observers.RemoveAt(0);
            }
            onValueChanged = null;
            observers = null;
        }
    }
}