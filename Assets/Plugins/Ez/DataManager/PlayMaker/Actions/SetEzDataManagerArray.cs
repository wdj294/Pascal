using Ez.DataManager;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("EzDataManager")]
    [Tooltip("Stores the values from an FSM array into an array from Ez Data Manager. Array elements must be of the same type.")]
    public class SetEzDataManagerArray : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Name of the array from EzDataManager.")]
        [UIHint(UIHint.FsmString)]
        public FsmString arrayName;

        [RequiredField]
        [Tooltip("FSM array from where to copy item values.")]
        [UIHint(UIHint.Variable)]
        public FsmArray setArray;

        public override void Reset()
        {
            base.Reset();
            arrayName = new FsmString() { UseVariable = false, Value = string.Empty };
            setArray = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter()
        {
            
            StoreArray();
            Finish();
        }

        private void StoreArray()
        {
            if(setArray == null || setArray.IsNone || string.IsNullOrEmpty(arrayName.Value))
            {
                return;
            }
            if(!EzDataManager.SetArrayByName(arrayName.Value, setArray.Values))
            {
                LogError(@"Could not set the Ez Data Manager array """ + arrayName.Value + @""" of type " + setArray.ElementType);
            }
        }
    }
}
