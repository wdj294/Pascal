using Ez.Binding;
using Ez.Binding.Vars;
using UnityEngine;

namespace Ez.Examples
{
    public class ModifyScale : MonoBehaviour
    {
        public bool Enabled = true;
        [Range(-10, 10)]
        public float Multiplier = 1;

        [Space(20)]
        public bool getValueFromBind = false;
        public string bindName;
        private Bind bind = null;

        private Vector3 targetScale = Vector3.one;
        public Vector3 TargetScale
        {
            get
            {
                if (!getValueFromBind) { return targetScale; }
                if (bind == null) { bind = EzBind.FindBindByName(bindName); }
                if (bind == null) { return targetScale; }
                return (Vector3)bind.Value;
            }
            set { targetScale = value; }
        }

        private Vector3 offsetScale { get { return TargetScale * Multiplier; } }
        private Vector3 startScale;

        public Vector3 ModifiedScale { get { return TargetScale; } set { targetScale = value; UpdateScale(); } }

        private void Start()
        {
            startScale = transform.localScale;
        }

        public void UpdateScale()
        {
            if (!Enabled) { transform.localScale = startScale; return; }
            transform.localScale = offsetScale;
        }
    }
}
