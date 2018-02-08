using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TutorialDesigner
{
	/// <summary>
	/// Central class of this TutorialDesigner. Container for all Nodes, also all gamelogic runs from here
	/// </summary>
	public class SavePoint : MonoBehaviour
	{
		/// <summary>
		/// List of all Nodes in this tutorial
		/// </summary>
        [HideInInspector]
		public List<Node> nodes;

		[SerializeField]
        [HideInInspector]
		private Node _startNode;
		/// <summary>
		/// The Node from which the tutorial starts. Ist must be selected before starting the Game.
		/// </summary>
		public Node startNode {
			get {				
				return _startNode;
			}
			set {
				#if UNITY_EDITOR
				foreach (Node n in nodes) {
					if ((n.nodeType & 1) == 1) {
						n.title = "Step";
					} else if ((n.nodeType & 2) == 2) {
						n.title = "Event";
					}				

					if (n == value) {						
						n.title += " (Start)";
					}
				}

				UnityEditor.Undo.RecordObject(this, "Alter Startnode");
				_startNode  = value;
				#endif
			}
		}

        /// <summary>
        /// This determines if the Tutorial should appear only one time
        /// </summary>
        public bool oneTimeTutorial;

        /// <summary>
        /// The name of the tutorial. Used for Im-/Exporting and for saving PlayerPrefs in case of One-Time Tutorial is checked
        /// </summary>
        public string tutorialName;

        /// <summary>
        /// The alternate event that should be executed if the Tutorial was already done
        /// </summary>
        public UnityEvent alternateEvent;

		#if UNITY_EDITOR	
		/// <summary>
		/// UI Canvas where dialogues will be added to. Can be changed if multiple canvases are in the scene.
		/// </summary>
		public Canvas canvas; 

		/// <summary>
		/// Zoom factor for the EditorWindow. Stored here to keep the value permanent
		/// </summary>
        [HideInInspector]
		public float zoomFactor = 1.0f;
		#endif

		void Start (){
			EventManager.Initialize();

			// Because Startnode and all global Nodes can run at the same time, separate them into paths
			int workingPathCount = 0;

            // For Builds
            HideAllDialogues ();

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.playmodeStateChanged += checkNodesBeforePlay;
            #endif

            // If this is a One-Time Tutorial, check if it's already done
            if (oneTimeTutorial) {
                if (IsTutorialDone()) {
                    alternateEvent.Invoke();
                    this.enabled = false;
                    Debug.Log("This One-Time Tutorial was already done");
                    return;
                }
            }

            // Search for Startnode and start it
			if (nodes != null) {
				if (nodes.Count == 0) {				
					Debug.LogWarningFormat ("<b><color=red><size=13>There are no nodes in this Tutorial</size></color></b>\n\r" +
						"Create some by right-clicking the Tutorial Editor Window");
				} else if (startNode == null) {
					Debug.LogWarningFormat ("<b><color=red><size=13>No Startnode defined</size></color></b>\n\r" +
						"Right click a Node and choose 'Start here'");
				} else {
					startNode.workingPath = workingPathCount++;
					startNode.StartWorking(this);
				}
			}           

			// Search for global Events and start them
			foreach (Node n in nodes) {
				if (n.GetType() == typeof(EventNode)) {
					EventNode eNode = (EventNode)n;
					if (eNode.IsGlobal()) {
						eNode.workingPath = workingPathCount++;
						eNode.StartWorking(this);
					}
				}
			}
		}

        /// <summary>
        /// Reactivation after One-Time Tutorial was done
        /// </summary>
        public void Reactivate() {
            this.enabled = true;
            Start();
        }

		#if UNITY_EDITOR
		/// <summary>
		/// Initialize this instance. Happens at creation of a new Tutorial
		/// </summary>
		public void Initialize() {
			if (nodes == null) {
				nodes = new List<Node>();
			}

			// Set Canvas for UI Dialogues
			if (canvas == null) {
				canvas = FindObjectOfType<Canvas>();
                if (canvas == null) {                    
                    GameObject go = new GameObject("Canvas");
                    go.layer = LayerMask.NameToLayer("UI");
                    Canvas can = go.AddComponent<Canvas>();
                    can.renderMode = RenderMode.ScreenSpaceOverlay;
                    go.AddComponent<UnityEngine.UI.CanvasScaler>();
                    go.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                    canvas = can;


                    Debug.Log("Created a Canvas because didn't find one");
                }
			}

            // Check for EventSystem Object in Scene
            if (GameObject.FindObjectOfType<EventSystem>() == null) {                        
                GameObject es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
                Debug.Log("Created EventSystem because no one found in Scene");
            }
		}

		// Check a few things before running the game
		private void checkNodesBeforePlay() {	
			// TutorialEditor is only null if it is not currently focused
            if (TutorialEditor.self == null) {		
				// Prevent 2 TutorialEditors being opened
				TutorialEditor[] tuts = Resources.FindObjectsOfTypeAll<TutorialEditor>();
				if (tuts.Length > 1) {
					for (int i = 1; i < tuts.Length; i++) {
						tuts [i].Close ();
					}
				}

				return;
			}

			// If User presses Play
			if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {					
				if (nodes != null) {							
					UnityEditor.Selection.activeObject = null;

					foreach (Node n in nodes) {
                        //if (!n.IsWorking()) n.ResetSkin();
						if ((n.nodeType & 2) == 2) {
							// Check if an Eventnode has empty Output Connectors
							/*foreach (Connector c in n.connectors) {
								if (c.visible && !c.input && c.connections.Count == 0) {
									// Send a warning
									Debug.LogWarningFormat ("<b><color=red><size=13>EventNode is missing Output</size></color></b>\n\r" +
										"Without something connected, this Event has no function");
									break;
								}
							}*/
						} else if ((n.nodeType & 1) == 1) {
							// If some Step is not the Startnode and has no Input, Warn that it will never be processed
							if (n == startNode) {
								// If Start Step has no Output connected
								bool inputsConnected = false;
								foreach (Connector c in n.connectors) {
									if (!c.input && c.connections.Count > 0) {
										inputsConnected = true;
										break;
									}
								}

								if (!inputsConnected) {
									Debug.LogWarningFormat ("<b><color=red><size=13>Startnode is missing Output</size></color></b>\n\r" +
										"Connect a Node to Startnode's Output");
								}
							}
						}
					}
				}
            } else {
                HideAllDialogues ();
                TutorialEditor.self.sideWindow.currentNode = "";
                TutorialEditor.self.sideWindow.globalNode = "";
            }

			TutorialEditor.self.Repaint();
		}

		#endif

		/// <summary>
		/// Main interface for Node working processes during the game. When the Nodes start / stop working,
		/// or calling an event, they send a command to this interface. NodeControl determines further actions
		/// about how the game process is handled.
		/// </summary>
		/// <param name="command">Node command</param>
		/// <param name="sourceNode">Node that called this function</param>
		/// <param name="targetNode">The calling node (sourceNode) can refer to the next Node in line (targetNode)</param>
		public void NodeControl(byte command, Node sourceNode, Node targetNode) {
			#if UNITY_EDITOR
			string text;
			#endif

			switch (command) {
				#if UNITY_EDITOR				
				case 0: // Node Execution has started
					if (TutorialEditor.self != null) {
						text = sourceNode.title + " (delay)\n\r" + sourceNode.description;
						if (sourceNode.workingPath == 0)
							TutorialEditor.self.sideWindow.currentNode = text;
						else 
							TutorialEditor.self.sideWindow.globalNode = text;
						TutorialEditor.self.Repaint();
					}
				break;
				case 1: // Activate Window here
                    sourceNode.nodeType = (byte)(sourceNode.nodeType | 4);                      
					if (TutorialEditor.self != null) {
						sourceNode.nodeStyle = TutorialEditor.customSkin.GetStyle("Activewindow");				
						// Set sourceNode active for Curve-Drawing
                        text = sourceNode.title + "\n\r" + sourceNode.description;
						if (sourceNode.workingPath == 0)
							TutorialEditor.self.sideWindow.currentNode = text;
						else 
							TutorialEditor.self.sideWindow.globalNode = text;
						TutorialEditor.self.Repaint();
					}
				break;
				case 2: // Show Dialogue (StepNode only)					
					if (TutorialEditor.self != null) {
						text = sourceNode.title + " (Dialogue)\n\r" + sourceNode.description;
						if (sourceNode.workingPath == 0)
							TutorialEditor.self.sideWindow.currentNode = text;
						else 
							TutorialEditor.self.sideWindow.globalNode = text;
					}
				break;
				#endif
			    case 10: // Switch to next Node					
					#if UNITY_EDITOR
					if (TutorialEditor.self != null) {
						sourceNode.ResetSkin ();
						// Set Activation Bits nodes. Used for highlighting the Curve Connection between active Nodes in the Editor
						foreach(Node n in nodes) {
							if (!TutorialEditor.IsNodeForeverEvent(n) && sourceNode.workingPath == n.workingPath) {
								// Remove the Active-Bit everywhere on current workingPath
								n.nodeType = (byte)(n.nodeType & ~4);
							}
						}							
					}
					#endif	

					if (targetNode != null) {					
                        if (targetNode.GetType () == typeof(StepNode)) {  
                            StepNode sn = (StepNode)targetNode; 
                            if (sn.delay == 0) 
                                HideAllDialogues (sourceNode.workingPath); 
                            else { 
                                StartCoroutine(FunctionCall(delegate() { 
                                    HideAllDialogues (sourceNode.workingPath); 
                                }, sn.delay)); 
                            } 
                        } 
						targetNode.workingPath = sourceNode.workingPath;
						targetNode.StartWorking(this);

                        #if UNITY_EDITOR
						// Set sourceNode and targetNode active for Curve-Drawing
						sourceNode.nodeType = (byte)(sourceNode.nodeType | 4);	
						targetNode.nodeType = (byte)(targetNode.nodeType | 4);
						#endif
					} else {
						HideAllDialogues(sourceNode.workingPath);	
						
						#if UNITY_EDITOR
						if (TutorialEditor.self != null) {
							if (sourceNode.workingPath == 0)
								TutorialEditor.self.sideWindow.currentNode = "";
							else 
								TutorialEditor.self.sideWindow.globalNode = "";
						}
						#endif
					}					
				break;
				case 11: // Forever-Event calling					
					if (targetNode != null) {
						if (!targetNode.IsWorking()) {							
							foreach(Node n in nodes) {								
								if (sourceNode != n && sourceNode.workingPath == n.workingPath) {									
									n.StopWorking(this);
									
									#if UNITY_EDITOR
									n.ResetSkin();
									// Remove the Active-Bit everywhere on current workingPath
									n.nodeType = (byte)(n.nodeType & ~4);
									#endif
								}
							}

							#if UNITY_EDITOR
							targetNode.nodeType = (byte)(targetNode.nodeType | 4);
							#endif
							
							HideAllDialogues (sourceNode.workingPath);
							targetNode.workingPath = sourceNode.workingPath;
							targetNode.StartWorking(this);
						}
					}
                break;
			}
		}

        private delegate void functionCall(); 
        private IEnumerator FunctionCall(functionCall call, float delay) { 
            yield return new WaitForSeconds(delay * Time.timeScale); 
            call(); 
        } 

		/// <summary>
		/// Same as above, without targetNode
		/// </summary>
		/// <param name="command">Node Command</param>
		/// <param name="sourceNode">Node that called this function</param>
        public void NodeControl(byte command, Node sourceNode) {
            NodeControl(command, sourceNode, null);
        }

		/// <summary>
		/// Hides all dialogues (GameObjects) on specific workingPath, which are children of canvas.
		/// </summary>
		/// <param name="path">workingPath to which this function will be constrained</param>
        public void HideAllDialogues (int path = -1, float delay = 0f) {
			foreach (Node n in nodes) {
                if (n.GetType() == typeof(StepNode)) { // Stepnode				
                    if (path == -1 || n.workingPath == path) {
                        // Deactivate Dialogues on either path, or everywhere (path = -1)
                        if (delay > 0)
                            StartCoroutine(DeactivateWithDelay(((StepNode)n).dialogue, delay));
                        else
                            ((StepNode)n).dialogue.SetActive(false);
					} 
				}
			}
		}

        private IEnumerator DeactivateWithDelay(Dialogue d, float delay) {
            yield return new WaitForSeconds(delay);
            d.SetActive(false);
        }

        /// <summary>
        /// Sets the key in PlayerPrefs if this Tutorial was done. This is only interesting if this should be a One-Time Tutorial
        /// </summary>
        /// <param name="forAllScenes">If set to <c>true</c> this goes for all scenes (global)</param>
        public void WriteTutorialDone(bool forAllScenes) {
            string value = "TDesigner." + tutorialName + ".";
            value += forAllScenes ? "Global" : UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            PlayerPrefs.SetString(value, "Done");
        }

        /// <summary>
        /// Deletes the key in PlayerPrefs if this Tutorial was done
        /// </summary>
        /// <param name="forAllScenes">If set to <c>true</c> this goes for all scenes (global)</param>
        public void UnsetTutorialDone(bool forAllScenes) {
            string value = "TDesigner." + tutorialName + ".";
            value += forAllScenes ? "Global" : UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            PlayerPrefs.DeleteKey(value);
        }

        /// <summary>
        /// Check PlayerPrefs if this Tutorial is already done
        /// </summary>
        /// <returns><c>true</c> if a global- or a scene-key exists
        public bool IsTutorialDone() {
            string value = "TDesigner." + tutorialName + ".";
            string globalVal = value + "Global";
            string sceneVal = value + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            if (PlayerPrefs.GetString(globalVal) != "" || PlayerPrefs.GetString(sceneVal) != "")
                return true;
            else
                return false;
        }
	}
}
