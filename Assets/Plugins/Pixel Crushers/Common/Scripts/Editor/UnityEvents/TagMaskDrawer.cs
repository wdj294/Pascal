// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers
{

    [CustomPropertyDrawer(typeof(TagMask), true)]
    public class TagMaskDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            try
            {
                var allTags = UnityEditorInternal.InternalEditorUtility.tags;
                var tagsProperty = property.FindPropertyRelative("m_tags");
                int mask = 0;
                for (int i = 0; i < tagsProperty.arraySize; i++)
                {
                    var tag = tagsProperty.GetArrayElementAtIndex(i).stringValue;
                    for (int j = 0; j < allTags.Length; j++)
                    {
                        if (string.Equals(tag, allTags[j]))
                        {
                            mask |= (1 << j);
                            continue;
                        }
                    }
                }
                var newMask = EditorGUI.MaskField(position, label, mask, allTags);
                if (newMask != mask)
                {
                    tagsProperty.ClearArray();
                    for (int j = 0; j < allTags.Length; j++)
                    {
                        if ((newMask & (1 << j)) != 0)
                        {
                            tagsProperty.arraySize++;
                            tagsProperty.GetArrayElementAtIndex(tagsProperty.arraySize - 1).stringValue = allTags[j];
                        }
                    }
                }
            }
            finally
            {
                EditorGUI.EndProperty();
            }
        }

    }
}
