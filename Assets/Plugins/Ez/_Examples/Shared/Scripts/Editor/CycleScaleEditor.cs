using UnityEditor;

namespace Ez.Examples
{
    [CustomEditor(typeof(CycleScale))]
    public class CycleScaleEditor : EBaseEditor
    {
        CycleScale cycleScale { get { return (CycleScale)target; } }

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
                if(EGUI.Button("Start Cycle", EStyles.GetStyle(EStyles.ButtonStyle.ButtonGreen))) { cycleScale.StartCycle(); }
                EGUI.Space(SPACE_2);
                if(EGUI.Button("Stop Cycle", EStyles.GetStyle(EStyles.ButtonStyle.ButtonRed))) { cycleScale.StopCycle(); }
                EGUI.Space(SPACE_2);
            }
            EGUI.EndHorizontal();
            EGUI.Space(SPACE_2);
            EGUI.BeginHorizontal(WIDTH_420);
            {
                EGUI.Space(SPACE_2);
                if(EGUI.Button("Min Scale", EStyles.GetStyle(EStyles.ButtonStyle.ButtonOrange))) { cycleScale.ScaleToMinScale(); }
                EGUI.Space(SPACE_2);
                if(EGUI.Button("Max Scale", EStyles.GetStyle(EStyles.ButtonStyle.ButtonOrange))) { cycleScale.ScaleToMaxScale(); }
                EGUI.Space(SPACE_2);
                if(EGUI.Button("Start Scale", EStyles.GetStyle(EStyles.ButtonStyle.ButtonOrange))) { cycleScale.ScaleToStartScale(); }
                EGUI.Space(SPACE_2);
            }
            EGUI.EndHorizontal();
            EGUI.Space(SPACE_4);
        }

    }
}
