using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace TutorialDesigner
{
    [CanEditMultipleObjects]
	[CustomEditor(typeof(StepNode))]
	/// <summary>
	/// Inspector override for a StepNode. Uses dialogue class as an interface to all components of the actual Gameobject,
	/// which will represent the dialogue. All customizations can be done by this editor
	/// </summary>
	public class StepNodeEditor : Editor {
		private StepNode sNode; // Target object
		private SerializedObject sObj; // Helper to get access to all objects inside the class

		// Parameters for normal and advanced Inspector view. Choosen by Dialogue.advancedSettings
		private string[] normalDialoguePanelSettings = {"Sprite"};
		private string[] advancedDialoguePanelSettings = {"Sprite", "Material", "Color", "Type", "Fill Center"};
		private string[] normalTextSettings = {"Font Size", "Alignment", "Color"};
		private string[] advancedTextSettings = {"Font", "Font Style", "Font Size", "Line Spacing", "Alignment", "Horizontal Overflow", "Vertical Overflow", "Best Fit", "Color", "Material"};

		/// <summary>
		/// Inspector Draw call
		/// </summary>
		public override void OnInspectorGUI()
		{		
			StepNode sNode = (StepNode)target;
            Dialogue dialogue = sNode.dialogue;

            if (targets.Length == 1) {
                // Single Editing
    			EditorGUILayout.LabelField(sNode.title, EditorStyles.boldLabel);
    			EditorGUILayout.LabelField (sNode.description);

    			EditorGUILayout.Space();

    			// The user settings will be taken over to the dialogues Gameobject here    			
    			if (dialogue.selectedDialogueID != 0) {    				
                    dialogue.InitComponents();
    				// Dialoguepanel Settings
    				EditorGUILayout.LabelField("Dialoguepanel Settings", EditorStyles.boldLabel);
    				EditorGUILayout.BeginVertical(EditorStyles.helpBox);
    				dialogue.panelPosition = (DialoguePanelPosition)EditorGUILayout.EnumPopup ("Panel Position", dialogue.panelPosition);
    				dialogue.panelOffset = EditorGUILayout.Vector2Field("Panel Offset", dialogue.panelOffset);
                    if (dialogue.advancedSettings) {
                        EditorGUILayout.LabelField("Panel Padding");
                        DrawSerializedObject(dialogue.panelLayout, new string[]{"Left", "Right", "Top", "Bottom"});
                    }
    				DrawSerializedObject(dialogue.dialogueBackgrImg, dialogue.advancedSettings ? advancedDialoguePanelSettings : normalDialoguePanelSettings);
    				if (dialogue.advancedSettings) dialogue.animate = EditorGUILayout.Toggle("Animated Popup", dialogue.animate);
    				EditorGUILayout.EndVertical();

    				EditorGUILayout.Space();

    				// Text Settings
    				EditorGUILayout.LabelField("Text Settings", EditorStyles.boldLabel);
    				EditorGUILayout.BeginVertical(EditorStyles.helpBox);
    				EditorGUI.BeginChangeCheck();
    				DrawSerializedObject(dialogue.textObj, dialogue.advancedSettings ? advancedTextSettings : normalTextSettings);
    				// If Text Settings are changed, apply them also to TextObjects of Response Buttons
    				if (EditorGUI.EndChangeCheck ()) dialogue.ApplyTextSettingsToButtons();

    				EditorGUILayout.EndVertical();

    				EditorGUILayout.Space();

    				// Image Settings
    				EditorGUILayout.LabelField("Image Settings", EditorStyles.boldLabel);
    				EditorGUILayout.BeginVertical(EditorStyles.helpBox);

    				EditorGUI.BeginChangeCheck();
    				dialogue.sprite = (Sprite)EditorGUILayout.ObjectField("Source Image", dialogue.sprite, typeof(Sprite), true);
    				if (dialogue.sprite != null) {				
    					dialogue.imagePosition = (DialogueImagePosition)EditorGUILayout.EnumPopup("Image Position", dialogue.imagePosition);
    					// If width/height values change here, set Editor dirty for applying changes to Scene-/Gameview immediately
    					dialogue.imgObj.color = EditorGUILayout.ColorField ("Color", dialogue.imgObj.color);
    					dialogue.imgSize = EditorGUILayout.Slider("Image Size", dialogue.imgSize, 0.2f, 5f);
                        dialogue.imgLayout.preferredWidth = dialogue.sprite.textureRect.width * dialogue.imgSize * dialogue.imgRatio;
                        dialogue.imgLayout.preferredHeight = dialogue.sprite.textureRect.height * dialogue.imgSize * dialogue.imgRatio;
    				}
    				if (EditorGUI.EndChangeCheck())	EditorUtility.SetDirty(dialogue.imgLayout);

    				EditorGUILayout.EndVertical();

    				// Button Settings
    				if (dialogue.buttonCount > 0) {
    					EditorGUILayout.LabelField ("Button Settings", EditorStyles.boldLabel);
    					EditorGUILayout.BeginVertical (EditorStyles.helpBox);
    					EditorGUI.BeginChangeCheck();

    					// Button Size Changing in 2 Directions: GUI -> Button's LayoutElements; LayoutElements -> GUI
    					Vector2 oldButtonSize = dialogue.buttonSize;
    					Vector2 newButtonSize = EditorGUILayout.Vector2Field ("Button Size", dialogue.buttonSize);
    					if (oldButtonSize != newButtonSize) {
    						// Size is changed by GUI here, from Inspector
    						dialogue.buttonSize = newButtonSize;
    						dialogue.UpdateButtonSizes (false, newButtonSize);
    					} else {
    						// This check is made every frame, adjust real size to the GUI
    						// LayoutElement is dynamic and can be changed by other Elements like Text 
    						dialogue.UpdateButtonSizes (true, Vector2.zero);
    					}

    					dialogue.buttonBackgroundImg = (Sprite)EditorGUILayout.ObjectField("Button Background", dialogue.buttonBackgroundImg, typeof(Sprite), true);
                        dialogue.buttonBackgroundColor = EditorGUILayout.ColorField("Button Color", dialogue.buttonBackgroundColor);
                        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(dialogue.bPanel);
    					EditorGUILayout.EndVertical ();

    					EditorGUILayout.Space ();
    				}

    				dialogue.advancedSettings = EditorGUILayout.Toggle("Advanced Settings", dialogue.advancedSettings);
    			}
            } else {
                bool allHaveDialogues = true;
                foreach(StepNode sn in targets) if (sn.dialogue.selectedDialogueID == 0) {allHaveDialogues = false; break;}

                if (allHaveDialogues) {
                    // Multi Editing
                    EditorGUILayout.LabelField("Multi-Editing Dialogues", EditorStyles.boldLabel);
                    EditorGUILayout.Space ();

					if (dialogue.dialogueBackgrImg == null)	dialogue.InitComponents();

                    // Dialoguepanel Settings
                    EditorGUILayout.LabelField("Dialoguepanel Settings", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUI.BeginChangeCheck();
                    dialogue.dialogueBackgrImg.sprite = (Sprite)EditorGUILayout.ObjectField("Background Image", dialogue.dialogueBackgrImg.sprite, typeof(Sprite), true);
                    if (EditorGUI.EndChangeCheck()) {
                        for (int i=1; i<targets.Length; i++) {
                            StepNode currentTarget = (StepNode)targets[i];
                            currentTarget.dialogue.dialogueBackgrImg.sprite = dialogue.dialogueBackgrImg.sprite;
                            EditorUtility.SetDirty(currentTarget.dialogue.dialogueBackgrImg);
                        }
                    }

                    EditorGUI.BeginChangeCheck();
                    dialogue.dialogueBackgrImg.color = EditorGUILayout.ColorField("Background Color", dialogue.dialogueBackgrImg.color);
                    if (EditorGUI.EndChangeCheck()) {
                        for (int i=1; i<targets.Length; i++) {
                            StepNode currentTarget = (StepNode)targets[i];
                            ((StepNode)targets[i]).dialogue.dialogueBackgrImg.color = dialogue.dialogueBackgrImg.color;
                            EditorUtility.SetDirty(currentTarget.dialogue.dialogueBackgrImg);
                        }
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space ();

                    // Text Settings
                    EditorGUILayout.LabelField("Text Settings", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUI.BeginChangeCheck();
                    dialogue.textObj.color = EditorGUILayout.ColorField("Text Color", dialogue.textObj.color);
                    if (EditorGUI.EndChangeCheck()) {
                        dialogue.ApplyTextSettingsToButtons();
                        for (int i=1; i<targets.Length; i++) {
                            StepNode currentTarget = (StepNode)targets[i];
                            currentTarget.dialogue.textObj.color = dialogue.textObj.color;
                            currentTarget.dialogue.ApplyTextSettingsToButtons();
                            EditorUtility.SetDirty(currentTarget.dialogue.textObj);
                        }
                    }

                    EditorGUI.BeginChangeCheck();
                    dialogue.textObj.fontSize = EditorGUILayout.IntField("Text Size", dialogue.textObj.fontSize);
                    if (EditorGUI.EndChangeCheck()) {
                        dialogue.ApplyTextSettingsToButtons();
                        for (int i=1; i<targets.Length; i++) {
                            StepNode currentTarget = (StepNode)targets[i];
                            currentTarget.dialogue.textObj.fontSize = dialogue.textObj.fontSize;
                            currentTarget.dialogue.ApplyTextSettingsToButtons();
                            EditorUtility.SetDirty(currentTarget.dialogue.textObj);
                        }
                    }

                    EditorGUI.BeginChangeCheck();
                    dialogue.textObj.font = (Font)EditorGUILayout.ObjectField("Font", dialogue.textObj.font, typeof(Font), true);
                    if (EditorGUI.EndChangeCheck()) {
                        dialogue.ApplyTextSettingsToButtons();
                        for (int i=1; i<targets.Length; i++) {
                            StepNode currentTarget = (StepNode)targets[i];
                            currentTarget.dialogue.textObj.font = dialogue.textObj.font;
                            currentTarget.dialogue.ApplyTextSettingsToButtons();
                            EditorUtility.SetDirty(currentTarget.dialogue.textObj);
                        }
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();

                    // Button Background
                    EditorGUILayout.LabelField("Button Background", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUI.BeginChangeCheck();
                    dialogue.buttonBackgroundImg = (Sprite)EditorGUILayout.ObjectField("Background Image", dialogue.buttonBackgroundImg, typeof(Sprite), true);
                    if (EditorGUI.EndChangeCheck()) {
                        for (int i=1; i<targets.Length; i++) {
                            StepNode currentTarget = (StepNode)targets[i];
                            currentTarget.dialogue.buttonBackgroundImg = dialogue.buttonBackgroundImg;

                        }
                    }

                    EditorGUI.BeginChangeCheck();
                    dialogue.buttonBackgroundColor = EditorGUILayout.ColorField("Background Color", dialogue.buttonBackgroundColor);
                    if (EditorGUI.EndChangeCheck()) {
                        for (int i=1; i<targets.Length; i++) {
                            StepNode currentTarget = (StepNode)targets[i];
                            currentTarget.dialogue.buttonBackgroundColor = dialogue.buttonBackgroundColor;
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
		}

		/// <summary>
		/// Draws UI elements by EditorGUI.PropertyField, given by a list of serialized properties
		/// </summary>
		/// <param name="obj">Object that is converted into a SerializedObject</param>
		/// <param name="s">List of parameters. Can be components, values, etc...</param>
		public void DrawSerializedObject(Object obj, params string[] s) {		
			if (obj == null)
				return;
			
			sObj = new SerializedObject(obj); 
			SerializedProperty prop = sObj.GetIterator();
			while (prop.NextVisible(true)) {
				for (int i=0; i<s.Length; i++) {				
					if (s[i] == prop.displayName) {					
						EditorGUILayout.PropertyField(prop);
						break;
					}
				}
			}
			sObj.ApplyModifiedProperties();
		}
	}
}