using Ez.DataManager;
using System;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("EzDataManager")]
    [Tooltip("Loads an array from Ez Data Manager and stores it into an FSM array. Array elements must be of the same type.")]
    public class GetEzDataManagerArray : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Name of the array from EzDataManager.")]
        [UIHint(UIHint.FsmString)]
        public FsmString arrayName;

        [RequiredField]
        [Tooltip("FSM array where to copy the values from EzDataManager.")]
        [UIHint(UIHint.Variable)]
        public FsmArray storeArray;

        public override void Reset()
        {
            base.Reset();
            arrayName = new FsmString() { UseVariable = false, Value = string.Empty };
            storeArray = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter()
        {
            GetArray();
            Finish();
        }

        private void GetArray()
        {
            if(string.IsNullOrEmpty(arrayName.Value) || storeArray == null || storeArray.IsNone)
            {
                return;
            }

            Array array;
            if(!EzDataManager.GetArrayByName(arrayName.Value, out array))
            {
                LogError(@"Could not get the Ez Data Manager array """ + arrayName.Value);
                return;
            }

            if(GetArrayElementType(array.GetType().GetElementType().ToString()) != storeArray.ElementType)
            {
                LogError(@"Could not get Ez Data Manager array """ + arrayName.Value + @"""! PlayMaker array""" + storeArray.Name + @"""has a  different type!");
                return;
            }

            storeArray.Resize(array.Length);
            storeArray.Values = array as object[];
        }

        private VariableType GetArrayElementType(string t)
        {
            if(t.Contains("[]"))
                t = t.Replace("[]", "");
            else if(t.Contains("System.Collections.Generic.List`1"))
            { t = t.Replace("System.Collections.Generic.List`1", ""); t = t.Replace("[", ""); t = t.Replace("]", ""); }

            switch(t)
            {
                case "System.Boolean":
                    return VariableType.Bool;
                case "UnityEngine.Color":
                    return VariableType.Color;
                case "System.Single":
                    return VariableType.Float;
                case "UnityEngine.GameObject":
                    return VariableType.GameObject;
                case "System.Int32":
                    return VariableType.Int;
                case "UnityEngine.Material":
                    return VariableType.Material;
                case "UnityEngine.Object":
                    return VariableType.Object;
                case "UnityEngine.Quaternion":
                    return VariableType.Quaternion;
                case "UnityEngine.Rect":
                    return VariableType.Rect;
                case "System.String":
                    return VariableType.String;
                case "UnityEngine.Texture":
                    return VariableType.Texture;
                case "UnityEngine.Vector2":
                    return VariableType.Vector2;
                case "UnityEngine.Vector3":
                    return VariableType.Vector3;
                default:
                    return VariableType.Unknown;
            }
        }
    }
}
