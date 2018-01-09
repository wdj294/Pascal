// Copyright (c) 2016 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Ez
{
    public class PropertyReader
    {
        /// <summary>
        /// Simple struct to stores the type and name of variables
        /// </summary>
        public struct Variable
        {
            public Type type;
            public string name;
        }

        /// <summary>
        /// Fileds cache used for instances of classes that inherit PropertyReader
        /// </summary>
        private Variable[] _fields_cache;
        /// <summary>
        /// Properties cache used for instances of classes that inherit PropertyReader
        /// </summary>
        private Variable[] _props_cache;

        public Variable[] getFields()
        {
            if (_fields_cache == null)
                _fields_cache = getFields(this.GetType());

            return _fields_cache;
        }

        public Variable[] getProperties()
        {
            if (_props_cache == null)
                _props_cache = getProperties(this.GetType());

            return _props_cache;
        }

        /// <summary>
        /// Getter for instance values that inherit PropertyReader
        /// </summary>
        public object getValue(string name)
        {
            return this.GetType().GetProperty(name).GetValue(this, null);
        }

        /// <summary>
        /// Setter for instance values that inherit PropertyReader
        /// </summary>
        public void setValue(string name, object value)
        {
            this.GetType().GetProperty(name).SetValue(this, value, null);
        }

        /// <summary>
        /// Returns all the values of a given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Variable[] getFields(Type type)
        {
            var fieldValues = type.GetFields();
            var result = new Variable[fieldValues.Length];
            for (int i = 0; i < fieldValues.Length; i++)
            {
                result[i].name = fieldValues[i].Name;
                result[i].type = fieldValues[i].GetType();
            }

            return result;
        }

        /// <summary>
        /// Returns all the values of the given type
        /// </summary>
        public static Variable[] getProperties(Type type)
        {
            var propertyValues = type.GetProperties();
            var result = new Variable[propertyValues.Length];
            for (int i = 0; i < propertyValues.Length; i++)
            {
                result[i].name = propertyValues[i].Name;
                result[i].type = propertyValues[i].GetType();
            }

            return result;
        }
    }
}
