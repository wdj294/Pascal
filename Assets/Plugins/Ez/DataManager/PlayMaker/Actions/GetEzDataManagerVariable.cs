using UnityEngine;
using Ez.DataManager;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("EzDataManager")]
    [Tooltip("Loads a variable from Ez Data Manager and passes it to a FSM variable.")]
    public class GetEzDataManagerVariable : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Name of the variable from EzDataManager.")]
        [UIHint(UIHint.FsmString)]
        public FsmString variableName;

        [RequiredField]
        [Tooltip("FSM variable where to store the retrieved value.")]
        [UIHint(UIHint.Variable)]
        public FsmVar storeValue;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            base.Reset();
            variableName = null;
            storeValue = new FsmVar() { useVariable = true };
            everyFrame = false;
        }

        // Code that runs on entering the state.
        public override void OnEnter()
        {
            GetValue();

            if(!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            GetValue();
        }

        private void GetValue()
        {
            if(string.IsNullOrEmpty(variableName.Value) || storeValue.IsNone)
            {
                return;
            }

            bool result = true;

            switch(storeValue.Type)
            {
                case VariableType.Float:
                    result = EzDataManager.GetFloatByName(variableName.Value, out storeValue.floatValue);
                    break;
                case VariableType.Int:
                    result = EzDataManager.GetIntByName(variableName.Value, out storeValue.intValue);
                    break;
                case VariableType.Bool:
                    result = EzDataManager.GetBoolByName(variableName.Value, out storeValue.boolValue);
                    break;
                case VariableType.GameObject:
                    GameObject tempGO;
                    result = EzDataManager.GetGameObjectByName(variableName.Value, out tempGO);
                    storeValue.gameObjectValue = result ? tempGO : storeValue.gameObjectValue;
                    break;
                case VariableType.String:
                    result = EzDataManager.GetStringByName(variableName.Value, out storeValue.stringValue);
                    break;
                case VariableType.Vector2:
                    Vector2 tempV2;
                    result = EzDataManager.GetVector2ByName(variableName.Value, out tempV2);
                    storeValue.vector2Value = result ? tempV2 : storeValue.vector2Value;
                    break;
                case VariableType.Vector3:
                    Vector3 tempV3;
                    result = EzDataManager.GetVector3ByName(variableName.Value, out tempV3);
                    storeValue.vector3Value = result ? tempV3 : storeValue.vector3Value;
                    break;
                case VariableType.Color:
                    Color tempColor;
                    result = EzDataManager.GetColorByName(variableName.Value, out tempColor);
                    storeValue.colorValue = result ? tempColor : storeValue.colorValue;
                    break;
                case VariableType.Rect:
                    Rect tempRect;
                    result = EzDataManager.GetRectByName(variableName.Value, out tempRect);
                    storeValue.rectValue = result ? tempRect : storeValue.rectValue;
                    break;
                case VariableType.Material:
                    Material tempMat;
                    result = EzDataManager.GetMaterialByName(variableName.Value, out tempMat);
                    storeValue.materialValue = result ? tempMat : storeValue.materialValue;
                    break;
                case VariableType.Texture:
                    Texture tempTexture;
                    result = EzDataManager.GetTextureByName(variableName.Value, out tempTexture);
                    storeValue.textureValue = result ? tempTexture : storeValue.textureValue;
                    break;
                case VariableType.Quaternion:
                    Quaternion tempQ;
                    result = EzDataManager.GetQuaternionByName(variableName.Value, out tempQ);
                    storeValue.quaternionValue = result ? tempQ : storeValue.quaternionValue;
                    break;
                case VariableType.Object:
                    Object tempObj;
                    result = EzDataManager.GetObjectByName(variableName.Value, out tempObj);
                    storeValue.objectReference = result ? tempObj : storeValue.objectReference;
                    break;
                case VariableType.Unknown:
                    result = false;
                    break;
                case VariableType.Array:
                    result = false;
                    break;
                case VariableType.Enum:
                    result = false;
                    break;
                default:
                    result = false;
                    break;
            }

            if(!result)
            {
                LogError(@"Could not get the Ez Data Manager variable """ + variableName.Value + @""" of type " + storeValue.Type);
                return;
            }

            var targetVariable = Fsm.Variables.GetVariable(storeValue.variableName);
            if(targetVariable.VariableType != storeValue.Type)
            {
                LogError(@"Could not get the Ez Data Manager variable """ + variableName.Value + @"""! PlayMaker variable has a different type!");
                return;
            }
            storeValue.ApplyValueTo(targetVariable);
        }
    }
}
