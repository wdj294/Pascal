using UnityEngine;

namespace Ez.Examples
{
    public class SetMaterialColor : MonoBehaviour
    {
        public Color Color;
        public Renderer TargetRenderer;

        private void Start()
        {
            if(TargetRenderer == null) { TargetRenderer = GetComponent<Renderer>(); }
        }

        private void Update()
        {
            if(TargetRenderer == null) { return; }
            TargetRenderer.material.color = Color;
        }
    }
}
