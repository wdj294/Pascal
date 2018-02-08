using UnityEditor;

namespace Ez.Examples
{
    [CustomEditor(typeof(CycleMaterials))]
    public class CycleMaterialsEditor : EBaseEditor
    {
        CycleMaterials cycleMaterials { get { return (CycleMaterials)target; } }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(!EditorApplication.isPlaying) { return; }
            EGUI.Space(SPACE_4);
            EGUI.Label("Cycle Controller", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), WIDTH_420);
            EGUI.Space(SPACE_2);
            EGUI.BeginHorizontal(WIDTH_420);
            {
                EGUI.Space(SPACE_2);
                if(EGUI.Button("Start Cycle", EStyles.GetStyle(EStyles.ButtonStyle.ButtonGreen))) { cycleMaterials.StartCycle(); }
                EGUI.Space(SPACE_2);
                if(EGUI.Button("Stop Cycle", EStyles.GetStyle(EStyles.ButtonStyle.ButtonRed))) { cycleMaterials.StopCycle(); }
                EGUI.Space(SPACE_2);
            }
            EGUI.EndHorizontal();
            EGUI.Space(SPACE_4);
        }
    }
}
