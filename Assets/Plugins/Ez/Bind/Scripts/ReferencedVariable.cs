// Copyright (c) 2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Reflection;
using UnityEngine;

namespace Ez.Binding.Vars
{
    public class ReferencedVariable
    {
        private readonly UnityEngine.Object instance;
        private readonly FieldInfo fieldInfo;
        private readonly PropertyInfo propertyInfo;
        private readonly string binderName;
        private readonly string myFullName;

        public ReferencedVariable(
            string bindingVariableName,
            UnityEngine.Object targetObject,
            string targetName)
        {
            this.instance = targetObject;
            this.binderName = bindingVariableName;

            this.fieldInfo = targetObject.GetType().GetField(targetName);
            this.propertyInfo = targetObject.GetType().GetProperty(targetName);

            if(fieldInfo != null)
            {
                myFullName = targetObject.name + ": " + fieldInfo.Name;
            }
            else if(propertyInfo != null)
            {
                myFullName = targetObject.name + ": " + propertyInfo.Name;
            }
            else // both are null
            {
                throw new UnityException(@"[EzBind] Initialization error. Binding Variable """ + bindingVariableName +
                    @""" is trying to bind a data type that is not permitted on " + targetObject.name);
            }
        }

        public bool IsNull() { return (instance == null); }

        public object Value
        {
            get
            {
                return fieldInfo == null ? propertyInfo.GetValue(instance, null) : fieldInfo.GetValue(instance);
            }
            set
            {
                if(this.IsNull()) { return; }
                try
                {
                    if(fieldInfo != null)
                    {
                        if(value != null && value.GetType() != fieldInfo.FieldType)
                        {
                            fieldInfo.SetValue(instance, Convert.ChangeType(value, fieldInfo.FieldType));
                        }
                        else
                        {
                            fieldInfo.SetValue(instance, value);
                        }
                    }
                    else
                    {
                        if(value != null && value.GetType() != propertyInfo.PropertyType)
                        {
                            propertyInfo.SetValue(instance, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(instance, value, null);
                        }
                    }
                }
                catch(Exception e)
                {
                    Debug.LogError(@"[EzBind] - Bind named """ + binderName + @""" encountered the following error when trying to update observer """
                        + myFullName + @""": " + e.Message);
                }

            }
        }
    }
}
