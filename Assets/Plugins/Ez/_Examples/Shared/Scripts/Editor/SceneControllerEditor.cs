using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Ez.Examples
{
    [CustomEditor(typeof(SceneController))]
    public class SceneControllerEditor : EBaseEditor
    {
        SceneController sceneController { get { return (SceneController)target; } }

        SerializedProperty rootCanvas, transitionSpeed, zones;
        List<AnimBool> showZone;

        private void InitSerializedProperties()
        {
            rootCanvas = serializedObject.FindProperty("rootCanvas");
            zones = serializedObject.FindProperty("zones");
            transitionSpeed = serializedObject.FindProperty("transitionSpeed");
        }

        private void OnEnable()
        {
            InitSerializedProperties();
            showZone = new List<AnimBool>();
            if (zones.arraySize == 0) { return; }
            for (int i = 0; i < zones.arraySize; i++) { showZone.Add(new AnimBool(false, Repaint)); }
        }

        private void RenameUIAndContainers()
        {
            if (sceneController.zones == null || sceneController.zones.Count == 0) { return; }
            for (int i = 0; i < sceneController.zones.Count; i++)
            {
                if (sceneController.zones[i] == null) { continue; }
                if (sceneController.zones[i].canvas != null) { sceneController.zones[i].canvas.name = "Zone " + (i + 1) + " - " + sceneController.zones[i].name; }
                if (sceneController.zones[i].container != null) { sceneController.zones[i].container.name = "Zone " + (i + 1) + " - " + sceneController.zones[i].name; }
            }
        }

        public override void OnInspectorGUI()
        {
            RenameUIAndContainers();
            serializedObject.Update();
            EGUI.Space(SPACE_2);
            EGUI.Label("Scene Controller", EStyles.GetStyle(EStyles.TextStyle.ComponentTitle), WIDTH_420);
            EGUI.Space(SPACE_2);
            EGUI.BeginHorizontal(WIDTH_420);
            {
                EGUI.Label("Root Canvas", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), 90);
                EGUI.PropertyField(rootCanvas);
            }
            EGUI.EndHorizontal();
            EGUI.Space(SPACE_2);
            EGUI.BeginHorizontal(WIDTH_420);
            {
                EGUI.Label("Transition Speed", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), 120);
                transitionSpeed.floatValue = EditorGUILayout.Slider(transitionSpeed.floatValue, 0f, 5f);
            }
            EGUI.EndHorizontal();
            EGUI.Space(SPACE_8);
            if (EditorApplication.isPlaying && zones.arraySize == 0)
            {
                EGUI.BeginHorizontal(WIDTH_420);
                {
                    EGUI.FlexibleSpace();
                    EGUI.Label("No zones have been found!");
                    EGUI.FlexibleSpace();
                }
                EGUI.EndHorizontal();
                serializedObject.ApplyModifiedProperties();
                return;
            }
            EGUI.BeginHorizontal(WIDTH_420);
            {
                EGUI.Label("Zones", EStyles.GetStyle(EStyles.TextStyle.ComponentSubtitle), 60, HEIGHT_16);
                EGUI.Space(SPACE_2);
                if (!EditorApplication.isPlaying)
                {

                    if (EGUI.Button("Add Zone", EStyles.GetStyle(EStyles.ButtonStyle.ButtonGreen), WIDTH_420 - 2 - 60))
                    {
                        if (EditorApplication.isPlaying) { Debug.Log("Cannot add a new zone in Play Mode!"); return; }
                        showZone.Add(new AnimBool(true, Repaint));
                        sceneController.zones.Add(new Zone());
                        EGUI.MarkSceneDirty();
                    }
                }
                else
                {
                    if (EGUI.Button("Previous Zone", EStyles.GetStyle(EStyles.ButtonStyle.ButtonOrange), (WIDTH_420 - 2 - 60) / 2))
                    {
                        if (zones.arraySize == 0) { Debug.Log("No zones have been found!"); return; }
                        sceneController.PreviousZone(!EditorApplication.isPlaying);
                    }
                    EGUI.Space(SPACE_2);
                    if (EGUI.Button("Next Zone", EStyles.GetStyle(EStyles.ButtonStyle.ButtonOrange), (WIDTH_420 - 2 - 60) / 2))
                    {
                        if (zones.arraySize == 0) { Debug.Log("No zones have been found!"); return; }
                        sceneController.NextZone(!EditorApplication.isPlaying);
                    }
                }
            }
            EGUI.EndHorizontal();
            EGUI.Space(SPACE_4);
            if (zones.arraySize > 0) { for (int i = 0; i < zones.arraySize; i++) { DrawZone(i); } }
            EGUI.Space(SPACE_4);
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawZone(int index)
        {
            EGUI.Space(SPACE_2);
            if (EGUI.Button("   " + (index + 1) + " - " + zones.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue,
                            (index == sceneController.currentZoneIndex && EditorApplication.isPlaying)
                                    ? EStyles.GetStyle(EStyles.ButtonStyle.LeftButtonOrange)
                                    : EStyles.GetStyle(EStyles.ButtonStyle.LeftButtonGreyMild),
                            WIDTH_420))
            {
                showZone[index].target = !showZone[index].target;
            }
            if (EGUI.BeginFadeGroup(showZone[index].faded))
            {

                EGUI.Space(SPACE_2);
                EGUI.BeginHorizontal(WIDTH_420);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.Label("Name", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), 80);
                    EGUI.Space(-60);
                    EGUI.PropertyField(zones.GetArrayElementAtIndex(index).FindPropertyRelative("name"));
                    if (EGUI.ButtonMinus())
                    {
                        if (index == zones.arraySize - 1)
                        {
                            sceneController.zones.RemoveAt(index);
                            serializedObject.ApplyModifiedProperties();
                            EGUI.EndHorizontal();
                            EGUI.EndFadeGroup();
                            return;
                        }
                        else
                        {
                            zones.DeleteArrayElementAtIndex(index);
                        }
                        EGUI.MarkSceneDirty();
                    }
                }
                EGUI.EndHorizontal();
                EGUI.BeginHorizontal(WIDTH_420);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.Label("Canvas", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), 80);
                    EGUI.Space(-60);
                    EGUI.PropertyField(zones.GetArrayElementAtIndex(index).FindPropertyRelative("canvas"));
                }
                EGUI.EndHorizontal();
                EGUI.BeginHorizontal(WIDTH_420);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.Label("Container", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), 80);
                    EGUI.Space(-60);
                    EGUI.PropertyField(zones.GetArrayElementAtIndex(index).FindPropertyRelative("container"));
                }
                EGUI.EndHorizontal();
                EGUI.BeginHorizontal(WIDTH_420);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.Label("Camera Position", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), 120, HEIGHT_16);
                    EGUI.PropertyField(zones.GetArrayElementAtIndex(index).FindPropertyRelative("cameraPosition"));
                    if (EGUI.Button("Get", EStyles.GetStyle(EStyles.ButtonStyle.ButtonBlue), 40, HEIGHT_16)) { zones.GetArrayElementAtIndex(index).FindPropertyRelative("cameraPosition").vector3Value = Camera.main.transform.position; }
                    EGUI.Space(1);
                    if (EGUI.Button("Set", EStyles.GetStyle(EStyles.ButtonStyle.ButtonBlue), 40, HEIGHT_16)) { Camera.main.transform.position = zones.GetArrayElementAtIndex(index).FindPropertyRelative("cameraPosition").vector3Value; }
                }
                EGUI.EndHorizontal();
                EGUI.BeginHorizontal(WIDTH_420);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.Label("Camera Rotation", EStyles.GetStyle(EStyles.TextStyle.ComponentNormal), 120, HEIGHT_16);
                    EGUI.PropertyField(zones.GetArrayElementAtIndex(index).FindPropertyRelative("cameraRotation"));
                    if (EGUI.Button("Get", EStyles.GetStyle(EStyles.ButtonStyle.ButtonBlue), 40, HEIGHT_16)) { zones.GetArrayElementAtIndex(index).FindPropertyRelative("cameraRotation").vector3Value = Camera.main.transform.rotation.eulerAngles; }
                    EGUI.Space(1);
                    if (EGUI.Button("Set", EStyles.GetStyle(EStyles.ButtonStyle.ButtonBlue), 40, HEIGHT_16)) { Camera.main.transform.rotation = Quaternion.Euler(zones.GetArrayElementAtIndex(index).FindPropertyRelative("cameraRotation").vector3Value); }

                }
                EGUI.EndHorizontal();
                EGUI.BeginHorizontal(WIDTH_420);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.PropertyField(zones.GetArrayElementAtIndex(index).FindPropertyRelative("OnShow"), "OnShow", true);
                }
                EGUI.EndHorizontal();
                EGUI.BeginHorizontal(WIDTH_420);
                {
                    EGUI.Space(SPACE_16);
                    EGUI.PropertyField(zones.GetArrayElementAtIndex(index).FindPropertyRelative("OnHide"), "OnHide", true);
                }
                EGUI.EndHorizontal();
            }
            EGUI.EndFadeGroup();
            EGUI.Space(SPACE_2);
        }
    }
}
