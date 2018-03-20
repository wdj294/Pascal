/*
 * TODO:
 * 1) Change to 'targets' and allow multi object editing
 */
#region Namespace Imports
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace UI.ThreeDimensional
{
    [CustomEditor(typeof(UIObject3D)), CanEditMultipleObjects]
    public class UIObject3DEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Dictionary<UIObject3D, Transform> targetPrefabs = new Dictionary<UIObject3D, Transform>();
            targetPrefabs = targets.ToDictionary(k => k as UIObject3D, v => (v as UIObject3D).ObjectPrefab);            

            EditorGUI.BeginChangeCheck();

            if (GUILayout.Button("Force Render"))
            {
                foreach (var t in targetPrefabs)
                {
                    t.Key.HardUpdateDisplay();
                }
            }

            EditorGUILayout.Space();

            base.OnInspectorGUI();

            if (!EditorGUI.EndChangeCheck()) return;
            
            foreach (var t in targetPrefabs)
            {
                if (t.Key.ObjectPrefab != t.Value)
                {
                    t.Key.HardUpdateDisplay();
                }
                else
                {
                    t.Key.UpdateDisplay(true);
                }
            }            

        }
    }
}
