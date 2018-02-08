using System.Collections;
using UnityEngine;

namespace Ez.Examples
{
    public class CycleMaterials : MonoBehaviour
    {
        public Material[] Materials;
        public bool AutoStart = true;
        public float StartDelay = 2;
        public float CycleInterval = 2;
        public Renderer Target = null;

        private Coroutine cCycle = null;
        private int currentMaterialIndex = -1;
        public Color activeMaterialColor { get { return Target == null ? Color.white : Target.material.color; } set { Target.material.color = value; } }

        private void Start()
        {
            if(AutoStart) { StartCycle(StartDelay, CycleInterval); }
        }

        IEnumerator iCycle(float startDelay, float cycleInterval)
        {
            yield return new WaitForSeconds(startDelay);
            while(true)
            {
                ChangeMaterial();
                yield return new WaitForSeconds(cycleInterval);
            }
        }

        public void StartCycle()
        {
            if(cCycle != null) { StopCoroutine(cCycle); cCycle = null; }
            cCycle = StartCoroutine(iCycle(0, CycleInterval));
        }

        public void StartCycle(float startDelay, float cycleInterval)
        {
            if(cCycle != null) { StopCoroutine(cCycle); cCycle = null; }
            cCycle = StartCoroutine(iCycle(StartDelay, CycleInterval));
        }

        public void StopCycle()
        {
            if(cCycle == null) { return; }
            StopCoroutine(cCycle);
            cCycle = null;
        }

        private void ChangeMaterial()
        {
            if(Target == null) { return; }
            if(Materials == null || Materials.Length == 0) { return; }
            if(currentMaterialIndex == -1) { currentMaterialIndex = 0; Target.material = Materials[currentMaterialIndex]; return; }
            currentMaterialIndex++;
            if(currentMaterialIndex == Materials.Length) { currentMaterialIndex = 0; }
            Target.material = Materials[currentMaterialIndex];
        }
    }
}
