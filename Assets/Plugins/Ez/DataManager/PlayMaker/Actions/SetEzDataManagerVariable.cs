using Ez.DataManager;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("EzDataManager")]
    [Tooltip("Sets the value of a variable in Ez Data Manager.")]
    public class SetEzDataManagerVariable : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Name of the variable in EzDataManager.")]
        [UIHint(UIHint.FsmString)]
        public FsmString variableName;

        [RequiredField]
        [Tooltip("Value to be set.")]
        public FsmVar setValue;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            base.Reset();
            variableName = new FsmString() { UseVariable = false, Value = string.Empty };
            setValue = new FsmVar();
            everyFrame = false;
        }

        // Code that runs on entering the state.
        public override void OnEnter()
        {
            SetVariable();
            if(!everyFrame)
            {
                Finish();
            }
        }

        // Code that runs every frame.
        public override void OnUpdate()
        {

        }

        private void SetVariable()
        {
            if(setValue.IsNone || string.IsNullOrEmpty(variableName.Value))
            {
                return;
            }

            bool result = true;

            switch(setValue.Type)
            {
                case VariableType.Float:
                    result = EzDataManager.SetFloatByName(variableName.Value, setValue.floatValue);
                    break;
                case VariableType.Int:
                    result = EzDataManager.SetIntByName(variableName.Value, setValue.intValue);
                    break;
                case VariableType.Bool:
                    result = EzDataManager.SetBoolByName(variableName.Value, setValue.boolValue);
                    break;
                case VariableType.GameObject:
                    result = EzDataManager.SetGameObjectByName(variableName.Value, setValue.gameObjectValue);
                    break;
                case VariableType.String:
                    result = EzDataManager.SetStringByName(variableName.Value, setValue.stringValue);
                    break;
                case VariableType.Vector2:
                    result = EzDataManager.SetVector2ByName(variableName.Value, setValue.vector2Value);
                    break;
                case VariableType.Vector3:
                    result = EzDataManager.SetVector3ByName(variableName.Value, setValue.vector3Value);
                    break;
                case VariableType.Color:
                    result = EzDataManager.SetColorByName(variableName.Value, setValue.colorValue);
                    break;
                case VariableType.Rect:
                    result = EzDataManager.SetRectByName(variableName.Value, setValue.rectValue);
                    break;
                case VariableType.Material:
                    result = EzDataManager.SetMaterialByName(variableName.Value, setValue.materialValue);
                    break;
                case VariableType.Texture:
                    result = EzDataManager.SetTextureByName(variableName.Value, setValue.textureValue);
                    break;
                case VariableType.Quaternion:
                    result = EzDataManager.SetQuaternionByName(variableName.Value, setValue.quaternionValue);
                    break;
                case VariableType.Object:
                    result = EzDataManager.SetObjectByName(variableName.Value, setValue.objectReference);
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
                    break;
            }

            if(!result)
            {
                LogError(@"Could not set the Ez Data Manager variable """ + variableName.Value + @""" of type " + setValue.Type);
                return;
            }
        }
    }
}
