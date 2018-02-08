using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace TutorialDesigner
{
    [CustomEditor(typeof(SavePoint))]
    public class TDEditor : Editor {                   

        private bool help = false;
        private Texture2D logo;
        private SerializedObject sObj;
        private SerializedProperty sProp;

		 /// <summary>
		 /// Raises the enable event.
		 /// </summary>
    	public void OnEnable() {
            SavePoint sp = (SavePoint)target;
            sObj = new SerializedObject(sp);
            sProp = sObj.FindProperty("alternateEvent");
        }

		/// <summary>
		/// Overrides the inspector GUI event.
		/// </summary>
        public override void OnInspectorGUI() {
            SavePoint sp = (SavePoint)target;

            if (logo == null) logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/TutorialDesigner/Textures/InspectorLogo.png");
            EditorGUILayout.LabelField(new GUIContent(logo), GUILayout.MinHeight(64));

            EditorGUI.BeginChangeCheck();
            sp.canvas = (Canvas)EditorGUILayout.ObjectField("Canvas Object", sp.canvas, typeof(Canvas), true);
            if (EditorGUI.EndChangeCheck()) {
                if (sp.canvas != null) {
                    foreach (Node n in sp.nodes) {
                        if ((n.nodeType & 1) == 1) {
                            StepNode sn = (StepNode)n;
                            GameObject dialogue = sn.dialogue.GetGameObject();
                            if (dialogue != null) {
                                RectTransform rtBackup = dialogue.GetComponent<RectTransform>();
                                Vector2 anchoredPosition = rtBackup.anchoredPosition;
                                Vector2 offsetMax = rtBackup.offsetMax;
                                Vector2 offsetMin = rtBackup.offsetMin;
                                Vector3 localScale = rtBackup.localScale;
                                float z = rtBackup.localPosition.z;

                                dialogue.transform.SetParent(sp.canvas.transform);
                                rtBackup.anchoredPosition = anchoredPosition;
                                rtBackup.offsetMax = offsetMax;
                                rtBackup.offsetMin = offsetMin;
                                rtBackup.localScale = localScale;
                                rtBackup.localPosition = new Vector3(rtBackup.localPosition.x, rtBackup.localPosition.y, z);
                            }
                        }
                    }
                }
            }

            sp.tutorialName = EditorGUILayout.TextField("Tutorial Name", sp.tutorialName);

            sp.oneTimeTutorial = EditorGUILayout.Toggle("One-Time Tutorial", sp.oneTimeTutorial);
            if (sp.oneTimeTutorial) {
                // Help
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                help = EditorGUILayout.Toggle("Display Help", help);

                // Alternate Event
                sObj.Update();
                EditorGUILayout.PropertyField(sProp, new GUIContent("AlternateEvent"));
                sObj.ApplyModifiedProperties();

                if (help) {
                    EditorGUILayout.LabelField("Tutorial should be appearing only once. This is done by writing 'TDesigner.[TutorialName].[SceneName]' into PlayerPrefs.\r\n" +
                    "Or 'TDesigner.[TutorialName].Global' if that sould go for all scenes in this project.", EditorStyles.wordWrappedLabel);                
                    EditorGUILayout.Space();
                }

                EditorGUILayout.LabelField("Current Keys in PlayerPrefs:", EditorStyles.boldLabel);

                string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

                // Global Key
                string keyName = "TDesigner." + sp.tutorialName + ".Global";
                string globalValue = PlayerPrefs.GetString(keyName);

                EditorGUILayout.BeginHorizontal();
                if (globalValue != "") {
                    EditorGUILayout.LabelField(keyName);
                    if (GUILayout.Button("Delete", GUILayout.MaxWidth(45))) {
                        PlayerPrefs.DeleteKey(keyName);
                    }
                }
                EditorGUILayout.EndHorizontal();

                // Scene Key
                keyName = "TDesigner." + sp.tutorialName + "." + sceneName;
                string sceneValue = PlayerPrefs.GetString(keyName);

                EditorGUILayout.BeginHorizontal();
                if (sceneValue != "") {
                    EditorGUILayout.LabelField(keyName);
                    if (GUILayout.Button("Delete", GUILayout.MaxWidth(45))) {
                        PlayerPrefs.DeleteKey(keyName);
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (sceneValue == "" && globalValue == "") EditorGUILayout.LabelField("No keys set");

                if (help) {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("This keys can be assigned during the Game. Drag TutorialSystem to a StepNode's Action, and chose " +
                        "'SavePoint.WriteTutorialDone'.\r\n  - true: for all scenes (global)\r\n  - false: only this scene", EditorStyles.wordWrappedLabel);
                }

                EditorGUILayout.EndVertical();
            }
        }
    }
}
