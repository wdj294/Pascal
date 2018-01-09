using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace LeTai.Asset.TranslucentImage.Editor
{
    [CustomEditor(typeof(TranslucentImageSource))]
    [CanEditMultipleObjects]
    public class TranslucentImageSourceEditor : UnityEditor.Editor
    {
        int tab, previousTab;
        AnimBool advanced = new AnimBool(false);

        #region constants

        const int Min = 0;
        const int MaxIteration = 6;
        const int MaxDownsample = 6;

        readonly GUIContent sizeLabel = new GUIContent(
            "Size",
            "Blurriness. Does NOT affect performance"
        );

        readonly GUIContent iterLabel = new GUIContent(
            "Iteration",
            "The number of times to run the algorithm to increase the smoothness of the effect. Can affect performance when increase"
        );

        readonly GUIContent dsLabel = new GUIContent(
            "Downsample",
            "Reduce the size of the screen before processing. Increase will improve performance but create more artifact."
        );

        readonly GUIContent depthLabel = new GUIContent(
            "Max Depth",
            "Decrease will reduce flickering, blurriness and performance");

        #endregion

        void Awake()
        {
            LoadTab();
            advanced.value = tab > 0;
        }

        void OnEnable()
        {
            //Smoothly switch tab
            advanced.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            var source = (TranslucentImageSource) target;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            tab = GUILayout.Toolbar(tab, new[] {"Simple", "Advanced"}, GUILayout.MaxWidth(192));
            if (tab != previousTab)
                SaveTab();
            previousTab = tab;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            advanced.target = tab > 0;

            if (EditorGUILayout.BeginFadeGroup(1 - advanced.faded))
            {
                //Simple tab
                source.Strength = Mathf.Max(0, EditorGUILayout.FloatField("Strength", source.Strength));
            }
            EditorGUILayout.EndFadeGroup();
            if (EditorGUILayout.BeginFadeGroup(advanced.faded))
            {
                //Advanced tab
                source.Size = EditorGUILayout.FloatField(sizeLabel, source.Size);

                source.Iteration = EditorGUILayout.IntSlider(
                    new GUIContent(iterLabel),
                    source.Iteration,
                    Min,
                    MaxIteration);

                source.Downsample = EditorGUILayout.IntSlider(
                    new GUIContent(dsLabel),
                    source.Downsample,
                    Min,
                    MaxDownsample);
            }
            EditorGUILayout.EndFadeGroup();

            //Common properties

            source.MaxDepth = EditorGUILayout.IntField(depthLabel, source.MaxDepth);
            source.maxUpdateRate = EditorGUILayout.FloatField("Max Update Rate", source.maxUpdateRate);
            source.preview = EditorGUILayout.Toggle("Preview", source.preview);

            EditorUtility.SetDirty(target);
            Undo.RecordObject(target, "Change Translucent Image Source property");
        }

        //Persist selected tab between sessions and instances
        void SaveTab()
        {
            EditorPrefs.SetInt("tab", tab);
        }

        void LoadTab()
        {
            if (EditorPrefs.HasKey("tab"))
                tab = EditorPrefs.GetInt("tab");
        }
    }
}