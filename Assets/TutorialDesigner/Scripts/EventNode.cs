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
    /// EventNodes are intended to wait for an userdefined Event, that will happen during the Game.
    /// It can be choosen how often this Event should happen, before the Tutorial will continue with
    /// the next Step.
    /// </summary>
	public class EventNode : Node {
        /// <summary>
        /// Names of the Events, this event is listening to
        /// </summary>
		public List<string> triggerName;

        /// <summary>
        /// Number of Counts of the Events, how often they should occur until something happens
        /// </summary>
		public List<int> eventCounter;

        /// <summary>
        /// If this is true, this EventNode will be listening for Events infinitely.
        /// This Node will never be deactivated. Can only be set if this is a global EventNode.
        /// Global means, no other Node is connected to its Input Connectors
        /// </summary>
		public bool forEver;

        private List<int> counterHits; // how often the event trigger has been triggered during the game
		private int eventResult = -1; // If an event reached its counter value, this will be the resulting way out

		#if UNITY_EDITOR
        /// <summary>
        /// The trashcan Texture, used as delete icon
        /// </summary>
		public static Texture2D trashcan;

        /// <summary>
        /// Constructor for initializing this class
        /// </summary>
		public override void Init() {
			base.Init ();

			title = "Event";

			triggerName = new List<string>();
			eventCounter = new List<int> ();
			triggerName.Add ("");
			eventCounter.Add (1);

			// Create 2 Connectors
			for (int i=0; i<3; i++) connectors.Add (ScriptableObject.CreateInstance<Connector>());

			//Input Connector
			connectors[0].Init(
				ConCorner.top_left, // Corner in the Rect, where the connector will be
				new Rect(-18f, 33f, 18f, 15f), // Connector Rect. Relative to Corner
				this, // Home Node
				true); // true = Input Connector, false = Output Connector

			//Input Connector 2
			connectors[1].Init(ConCorner.top_right, new Rect(0f, 33f, 18f, 15f), this, true);

			// First Output Connector
			connectors[2].Init(ConCorner.top_right, new Rect (0f, 90f, 18f, 15f), this, false);

			nodeStyle = TutorialEditor.customSkin.GetStyle("Eventwindow");
			nodeType = 2; // Second Bit stands for EventNode
		}

        /// <summary>
        /// Inits the node after importing from JSON
        /// </summary>
		public override void InitAfterImport() {
			nodeStyle = new GUIStyle(TutorialEditor.customSkin.GetStyle("Eventwindow"));
            title = "Event";
		}

        /// <summary>
        /// Overridden Draw function from Node. This handles the whole appearance and user interactions of an EventNode
        /// </summary>  
		public override void DrawNode() {        
			base.DrawNode ();

			if (trashcan == null) {
				trashcan = (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D> ("Assets/TutorialDesigner/Textures/trashcan.png");
			}

			// Input Connector Label
			string InputName = (connectors[0].connections.Count > 0 || connectors[1].connections.Count > 0) ?
				"On Connected Node" : "Global";
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField(new GUIContent(InputName), TutorialEditor.customSkin.label, GUILayout.Width(140f)); 
			if (InputName == "Global") {
				EditorGUILayout.LabelField("Forever", TutorialEditor.customSkin.label, GUILayout.Width(50f)); 
				forEver = EditorGUILayout.Toggle (forEver);
            } else {
                forEver = false;
            }
			EditorGUILayout.EndHorizontal ();

			// Description Textarea
			description = EditorGUILayout.TextArea(description, TutorialEditor.customSkin.textArea, GUILayout.Height(30), GUILayout.MinWidth(200));  

			// Define an int for defining item-Actions during for-loop
			// -1 = nothing happens; < triggerName.Count = mark item for deletion; == triggerName.Count = Add item
			int itemAction = -1;
			// Draw triggerNames + Counter from their lists. Also buttons for adding / removing
			for (int i = 0; i < triggerName.Count; i++) {
				// Name of Event that gets triggered
				Rect area = EditorGUILayout.BeginHorizontal ();
				if (triggerName.Count > 1) {
					if (GUILayout.Button (trashcan, TutorialEditor.customSkin.button)) {
						itemAction = i;
					}
				} else {
					// Space for button Replacement
					GUILayout.Space (32f);
				}
				EditorGUILayout.LabelField ("Trigger", TutorialEditor.customSkin.label, GUILayout.Width (50f));
				triggerName[i] = EditorGUILayout.TextField (triggerName[i]);

				if (area.y > 0)	connectors [i + 2].rect.y = area.y + 2f;

				EditorGUILayout.EndHorizontal ();

				// Counter for Event Trigger. Not allowed to be < 0
				EditorGUILayout.BeginHorizontal ();
				if (i == triggerName.Count - 1) {
					if (GUILayout.Button ("+", TutorialEditor.customSkin.button, GUILayout.Width (28f))) {
						itemAction = triggerName.Count;
					}
				} else {
					// Space for button Replacement
					GUILayout.Space (32f);
				}
				EditorGUILayout.LabelField ("Counter", TutorialEditor.customSkin.label);
				eventCounter[i] = Mathf.Clamp(EditorGUILayout.IntField (eventCounter[i], GUILayout.MaxWidth (50)), 1, 99999);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.Space ();
			}

			// Perform Item Actions, executed in buttons of previous for-loop 
			if (itemAction > -1) {
				if (itemAction < triggerName.Count) {
					// Delete Item
					Undo.RecordObject(this, "Delete Item");
					triggerName.RemoveAt (itemAction);
					eventCounter.RemoveAt (itemAction);
					connectors [itemAction + 1].Break ();
					connectors.RemoveAt (itemAction + 1);
				} else {
					// Add Item
					Undo.RecordObject(this, "Add Item");
					triggerName.Add("");
					eventCounter.Add (1);
					connectors.Add(ScriptableObject.CreateInstance<Connector>());
					connectors[connectors.Count - 1].Init(ConCorner.top_right, new Rect(0f, 0f, 18f, 15f), this, false);
				}
				rect = new Rect (rect.position, Vector2.zero);
			}
		}

        /// <summary>
        /// Overrides Copy function from Node. Used to Duplicate an EventNode
        /// </summary>
		public override Node Copy() {
			EventNode eNode = ScriptableObject.CreateInstance<EventNode> ();

			return eNode;
		}

        /// <summary>
        /// Resets the skin of this Node to normal. I. E. after it was drawn active
        /// </summary>
		public override void ResetSkin() {
            if (TutorialEditor.self != null)
			    nodeStyle = TutorialEditor.customSkin.GetStyle("Eventwindow");
		}

        /// <summary>
        /// Routines to check after an Undo or Redo has been done
        /// </summary>
        public override void UndoChecks() {

        }

        /// <summary>
        /// Overrides Remove function from Node
        /// </summary>
		public override void Remove ()
		{
			// Nothing to be done in current version
		}
		#endif

        /// <summary>
        /// Work process that will happen during the Game, as long as this EventNode is active
        /// </summary>
        /// <param name="sp">Reference to SavePoint with wich Node will communicate (SavePoint.NodeControl)</param>
		public override IEnumerator Work(SavePoint sp) {
			counterHits = new List<int>();
			eventResult = -1;
			for (int i=0; i<triggerName.Count; i++) counterHits.Add(0);

            EventManager.AddListener(EventReceiver);

			#if UNITY_EDITOR
			// Wait until custom skin was initialized
			if (TutorialEditor.self != null) while (TutorialEditor.customSkin == null) yield return null;
			
            sp.NodeControl(1, this);
            #endif            		

            while (true) {
    			// Next Node by default is the to Output connected Node
                if (eventResult != -1) {
                    Node nextNode = null;
        			if (connectors[2 + eventResult].connections.Count > 0) {
        				nextNode = connectors[2 + eventResult].connections [0].homeNode;
        			}

                    if (!forEver) {
                        // One Time Event
                        EventManager.RemoveListener (EventReceiver);
                        sp.NodeControl (10, this, nextNode);        			
                        break;     
                    } else {
                        // Forever Event
						for (int i=0; i<counterHits.Count; i++) counterHits[i] = 0;
                        eventResult = -1;
                        sp.NodeControl (11, this, nextNode);
                    }
                }

                yield return null;
            }

			workRoutine = null;
		}

        // After received Event, increase the hits of triggered Event
		private void EventReceiver(string eventName) {
			for (int i=0; i<triggerName.Count; i++) {
				if (eventName == triggerName[i]) {	
					counterHits[i]++;

					#if UNITY_EDITOR
                    // Update Tutorial Monitor in SideWindow
					if (TutorialEditor.self != null) {
						if (workingPath == 0)
							TutorialEditor.self.sideWindow.currentNode = EventsToString ();
						else 
							TutorialEditor.self.sideWindow.globalNode = EventsToString ();
					}
					#endif

					if (counterHits[i] >= eventCounter[i]) {
						eventResult = i;
						break;
					}
				}
			}
		}

        // Convert Event name and trigger hits to one string, suitable for SideWindow's monitor
		private string EventsToString() {
			string result = "Event\n\r";

			if (counterHits != null) {			
				for (int i = 0; i < triggerName.Count; i++) {
					result += triggerName [i] + ": " + counterHits [i];
					if (i != triggerName.Count - 1)	result += "\n\r";
				}
			}

			return result;
		}

        /// <summary>
        /// Determines whether this is a global EventNode. That means, it has nothing connected to its Input Connectors.
        /// Therefore it is not constrained to a previous connected Node
        /// </summary>
        /// <returns><c>true</c> if EventNode is global; otherwise, <c>false</c>.</returns>
        public bool IsGlobal() {
            return (connectors[0].connections.Count == 0 && connectors[1].connections.Count == 0);
        }
	}
}