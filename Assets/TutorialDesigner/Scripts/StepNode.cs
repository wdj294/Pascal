using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TutorialDesigner
{
    /// <summary>
    /// StepNodes are intended to do Tutorial steps. They contain custom Actions that can be executed,
    /// and a customizable dialogue with optional buttons, to choose a way how the Tutorial continues
    /// </summary>
	public class StepNode : Node {

		/// <summary>
		/// Customizable Actions that will be executed on this Step
		/// </summary>
		public UnityEvent Actions;

        /// <summary>
        /// Reference to this steps dialogue Class
        /// </summary>
		public Dialogue dialogue;

		#if UNITY_EDITOR
        /// <summary>
        /// In order to allow access to SerializedProperties of this class, it has to be converted in
        /// a SerializedObject first
        /// </summary>
        public SerializedObject serialObj;

        /// <summary>
        /// For drawing a UnityEvent (Actions) on the StepNode, Actions must be available as SerializedProperty.
        /// It can then be drawn by EditorGUILayout.PropertyField
        /// </summary>
		public SerializedProperty ActionsSerial;


	    /// <summary>
        /// Constructor for initializing this class
        /// </summary>
		public override void Init(){
			base.Init ();
			title = "Step";

			// Create 6 StepNode Connectors
			for (int i=0; i<6; i++) connectors.Add (ScriptableObject.CreateInstance<Connector>());

			//Input
			connectors[0].Init(
				ConCorner.top_left, // Corner in the Rect, where the connector will be
				new Rect(-18f, 35f, 18f, 15f), // Connector Rect. Relative to Corner
			    this, // Home Node
				true); // true = Input Connector, false = Output Connector

			// Second Input
			connectors[1].Init(ConCorner.top_right, new Rect(0f, 35f, 18f, 15f), this, true);

			// Output
			connectors[2].Init (ConCorner.bottom_right, new Rect (0f, -25f, 18f, 15f), this, false);

			// 3 Output Connectors for Dialogue Buttons
			for (int i=3; i<6; i++) {
				connectors[i].Init(ConCorner.top_right, new Rect (0f, 0f, 18f, 15f), this, false);
				connectors[i].visible = false;
			}

            // Set skin for this Node
			nodeStyle = TutorialEditor.customSkin.window;
			nodeType = 1; // First Bit stands for StepNode

            // Create a new Dialogue
			dialogue = new Dialogue();
		}

        /// <summary>
        /// Inits the node after importing from JSON
        /// </summary>
		public override void InitAfterImport() {
			nodeStyle = new GUIStyle (TutorialEditor.customSkin.window);
            title = "Step";
		}

		void OnEnable() {
			serialObj = new SerializedObject (this);
			ActionsSerial = serialObj.FindProperty("Actions");
		}

        /// <summary>
        /// Overridden Draw function from Node. This handles the whole appearance and user interactions of a StepNode
        /// </summary>	    
	    public override void DrawNode() {   		
			base.DrawNode ();

            // Make checks if something is not yet initialized. Abort this draw function in this case
			if (nodeStyle == null || connectors.Count == 0) {
				return;
			}
			if (connectors [0] == null || connectors [1] == null || connectors [2] == null) return;

            // Begin drawing the StepNode here
			EditorGUILayout.LabelField ("->                  Input                  <-", TutorialEditor.customSkin.label);  

			// Description Textarea
			description = EditorGUILayout.TextArea (description, TutorialEditor.customSkin.textArea, GUILayout.Height(36));        

			// Delay of this Node during Runtime
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Delay: ", TutorialEditor.customSkin.label);
			delay = EditorGUILayout.FloatField (delay, GUILayout.MaxWidth(50));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
			if (TutorialEditor.zoomFactor < 0.8f) GUI.enabled = false;

			// Turning this Node into a serialized Object to access its properties
			if (serialObj != null) serialObj.Update();
			EditorGUILayout.PropertyField(ActionsSerial);

			EditorGUILayout.Space ();
			
            // Dialogue
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField (new GUIContent ("Dialogue:"), TutorialEditor.customSkin.label, GUILayout.MaxWidth(92));
			int dialoguePrevious = dialogue.selectedDialogueID; // For checking changes
			int newDialogue = EditorGUILayout.Popup(dialogue.selectedDialogueID, Dialogue.dialoguePrefabs);
			EditorGUILayout.EndHorizontal ();

            // If zoom ist too small, disable the GUI elements for user interaction
			if (TutorialEditor.zoomFactor < 0.8f) GUI.enabled = true;

            // If dialogue selection has changed
			if (dialoguePrevious != newDialogue) {
				// Special Case where 2 Undo's has to be Recorded
				Undo.RegisterCompleteObjectUndo (this, "Dialogue change"); // To keep the GameObject after Undo
				Undo.RecordObject(this, "Dialogue change"); // To keep all Component References in Dialogue after Undo
				
                // Check if there are connected buttons, in this case don't change the dialogueID
                bool connectedButtons = false;
                if (newDialogue == 0) {
                    for (int i=3;i<connectors.Count;i++) {
                        if (connectors[i].visible) {
                            if (connectors[i].connections.Count > 0) {
                                connectedButtons = true;
                                break;
                            }
                        }
                    }
                }

                if (!connectedButtons) 
                    dialogue.selectedDialogueID = newDialogue;
                else
                    Debug.LogWarningFormat ("<b><color=red><size=13>" + title + " has connected buttons!</size></color></b>\n\r" +
                        "Disconnect them before changing the dialogue.");

				// Initialize new selected DialoguePanel
                UpdateConnectors (dialogue.selectedDialogueID != 0 ? dialogue.buttonCount : 0);
			}

            // If a dialogue has been selected, draw its GUI elements
			if (dialogue.selectedDialogueID != 0) {
				EditorGUILayout.Space ();
				EditorGUILayout.BeginVertical (TutorialEditor.customSkin.GetStyle("RL Header"));        // Frame around DialogueBoxEdit

				EditorGUILayout.LabelField (new GUIContent ("Dialoguetext"));
                				
                // Dialogue Text from static Textfield
				dialogue.text = EditorGUILayout.TextArea (dialogue.text, TutorialEditor.customSkin.textArea, GUILayout.MinHeight (40));
				
				// Int Field for ButtonCaption Count
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (new GUIContent ("Response Buttons:"), GUILayout.MaxWidth (150));
				int newButtonCount = Mathf.Clamp (EditorGUILayout.IntField (dialogue.buttonCount, GUILayout.MaxWidth (35)), 0, 3);
				EditorGUILayout.EndHorizontal ();

				if (newButtonCount > 0) {
					dialogue.pauseGame = EditorGUILayout.Toggle ("Pause Game?", dialogue.pauseGame);
				} else {
					dialogue.pauseGame = false;
				}

				UpdateConnectors (newButtonCount);

				EditorGUILayout.EndVertical ();             // End of DialogueBoxEdit Frame
            } 
            	        
			// Output Label for normal Output Connector. Only if there are no DialogueButtons with Outputs
			if (dialogue.buttonCount == 0) {
				EditorGUILayout.LabelField ("<-                  Output               ->", TutorialEditor.customSkin.label);
			}
		
            // Apply values changes by user, to the serial Object of this. This is necessary because Actions variable,
            // which was turned into a SerializedProperty
			serialObj.ApplyModifiedProperties();

			// Window expands automatically but does not Shrink. Make sure it always has the right size here
			if (GUI.changed) {
				rect.height = 0f;
			}
		}

        // Update states of this Node's Connectors
		private void UpdateConnectors(int newButtonCount) {		
			int previousButtonCount = dialogue.buttonCount;
			dialogue.buttonCount = newButtonCount;

			// Prevent Change of Dialoguebuttons Count if there are connections. Log warning messages then
			if (previousButtonCount != newButtonCount) {
				// If Dialoguebuttons changed from 0 -> X
				if (previousButtonCount == 0) {
					if (connectors [2].connections.Count > 0) {
						Debug.LogWarningFormat ("<b><color=red><size=13>Output Connector on " + title + " connected!</size></color></b>\n\r" +
							"To use Dialogue Buttons, you have to break the Output connection on " + title + " first.");
						dialogue.buttonCount = previousButtonCount;
						GUIUtility.keyboardControl = 0; // Remove Focus from input fields
					}
					// If Dialoguebuttons changed from X to X. And only if they became less
				} else if (newButtonCount < previousButtonCount) {
					bool result = false;
					// Check if one of the to-be-removed Dialogue Buttons have Connections
					for (int i=3 + newButtonCount; i<3 + previousButtonCount; i++) {
						if (connectors [i].connections.Count > 0) {
							result = true;
							break;
						}
					}

					if (result) {
						Debug.LogWarningFormat ("<b><color=red><size=13>Dialogue Buttons on " + title + " connected!</size></color></b>\n\r" +
							"Before removing Dialogue Buttons on " + title + ", remove its connections first.");
						dialogue.buttonCount = previousButtonCount;
						GUIUtility.keyboardControl = 0; // Remove Focus from input fields
					}
				}
			}

			// Caption Text for every ButtonCaption in Dialogue
			if (dialogue.buttonCount > 0) {
				// Hide normal Output Connector of this window when DialogueButtons are active
				connectors [2].visible = false; 
				// Draw Dialoguebuttons with connectors
				for (int i = 0; i < dialogue.buttonCount; i++) {
					Rect area = EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField (new GUIContent ("Caption " + i + ":"), GUILayout.MaxWidth (70));
					// Textfield with button caption
					string oldButtonCaption = dialogue.buttonCaptions[i];
                    string newButtonCaption = EditorGUILayout.TextField (dialogue.buttonCaptions [i]);;
					
					if (!string.IsNullOrEmpty(newButtonCaption)) {
						if (newButtonCaption != oldButtonCaption) {
							dialogue.buttonCaptions [i] = newButtonCaption;
							dialogue.UpdateButtonCaptions (i);
						}
					}
					// Output Connector for this dialogueButton
					connectors [3 + i].visible = true;
					if (area.y > 0)	connectors [3 + i].rect.y = area.y + 2f;
					EditorGUILayout.EndHorizontal ();
				}
			} else {
				// If no DialogueButtons are in this window, show normal Output Connector
				connectors [2].visible = true;

				// If Dialoguebuttons changed from X -> 0, refresh connections
				if (previousButtonCount != newButtonCount) {
					// Break Dialogue Buttons
					connectors [3].Break ();
					connectors [4].Break ();
					connectors [5].Break ();
				}
			}

			// Hide other Output Connectors for DialogueButtons
			for (int i=3 + dialogue.buttonCount; i < 6; i++) {
				connectors [i].visible = false;
			}
		}

        /// <summary>
        /// Overrides Remove function from Node
        /// </summary>
		public override void Remove() {
			if (dialogue.selectedDialogueID != 0) {
				Undo.DestroyObjectImmediate(dialogue.GetGameObject());
			}
		}

        /// <summary>
        /// Overrides Copy function from Node. Used to Duplicate a StepNode
        /// </summary>
		public override Node Copy() {
			StepNode sNode = ScriptableObject.CreateInstance<StepNode> ();

			return sNode;
		}

        /// <summary>
        /// Resets the skin of this Node to normal. I. E. after it was drawn active
        /// </summary>
		public override void ResetSkin() {
            if (TutorialEditor.self != null)
                nodeStyle = TutorialEditor.customSkin.window;
		}

        /// <summary>
        /// Routines to check after an Undo or Redo has been done
        /// </summary>
        public override void UndoChecks() {
            // If "No Dialogue" is selected after Undo, clear it's components. Otherwise they won't be initialized
            // the next time, the user selects a Dialogue
            if (dialogue.selectedDialogueID == 0) {
                dialogue.ClearComponents();

                // Also hide the Button Connectors, if there are some
                for (int i=3; i<connectors.Count; i++) connectors[i].visible = false;
            }
        }

        /// <summary>
        /// Overrides Activate from Node. Here it also shows its dialogue in Scene-/Gameview, while hiding others.
        /// </summary>
		public override void Activate ()
		{
			// Deactivate current active object, if there is one
			TutorialEditor.savePoint.HideAllDialogues();

			// Activate this Node
			base.Activate ();
			dialogue.SetActive(true);
		}
		#endif

		/// <summary>
        /// Work process that will happen during the Game, as long as this StepNode is active
        /// </summary>
        /// <param name="sp">Reference to SavePoint with wich Node will communicate (SavePoint.NodeControl)</param>
		public override IEnumerator Work(SavePoint sp) {
			#if UNITY_EDITOR
			// Wait until custom skin was initialized
			if (TutorialEditor.self != null) while (TutorialEditor.customSkin == null) yield return null;
			#endif

            // Reset dialogue result (will be set by dialogue buttons and control the progress of the Tutorial
			dialogue.buttonResult = -1;
			sp.NodeControl(0, this);

            // Wait for predefined delay
			yield return new WaitForSeconds (delay * Time.timeScale);
			sp.NodeControl(1, this);

			Node nextNode = null;

			// Actions
			Actions.Invoke();

			// Dialogue
			if (dialogue.selectedDialogueID != 0) {					
				float ts = Time.timeScale;

                #if UNITY_EDITOR
                if (TutorialEditor.savePoint == null) yield return null;
                #endif

				dialogue.SetActive(true);
				sp.NodeControl(2, this);

				if (dialogue.buttonCount > 0) {
					dialogue.AddButtonListeners();

					// Wait until a Dialoguebutton was pressed
					while (dialogue.buttonResult == -1) yield return null;

                    // Some button was pressed
					dialogue.SetActive(false);
					if (dialogue.pauseGame) Time.timeScale = ts;

					if (connectors [3 + dialogue.buttonResult].connections.Count > 0) {
                        // Define next node in this Tutorial, depending on which button was pressed
						nextNode = connectors [3 + dialogue.buttonResult].connections [0].homeNode;
					}
				} else {
					// If no dialogue buttons are available, default Output Connector will be used
					if (connectors [2].connections.Count > 0) {
						nextNode = connectors [2].connections [0].homeNode;
					}
				}
			} else {
				// If no dialogue buttons are available, default Output Connector will be used
				if (connectors [2].connections.Count > 0)
					nextNode = connectors [2].connections [0].homeNode;
			}

			// Move to the next Node
			sp.NodeControl(10, this, nextNode);
			
			workRoutine = null;
		}
	}
}
