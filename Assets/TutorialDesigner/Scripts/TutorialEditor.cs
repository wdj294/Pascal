#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TutorialDesigner
{
    /// <summary>
    /// Main class for the EditorWindow that will represent the TutorialDesigner
    /// </summary>
	public class TutorialEditor : EditorWindow {
		    
        /// <summary>
        /// Component of the TutorialSystem GameObject. It will contain Nodes and all settings that are important during the Game
        /// </summary>
	    public static SavePoint savePoint;
	    
        /// <summary>
        /// The side window within TutorialEditor
        /// </summary>
	    public SideWindow sideWindow; // Window at the right. With Menu and help
	    	    
        /// <summary>
        /// Defines a handle for Connections between mouse cursor and a Connector
        /// </summary>
        public struct ConHandle {
            public bool active;
            public Node node;
            public Connector connector;
            public Vector2 position;
            public bool left;
        }	    

        /// <summary>
        /// The handle for current clicked Node Connector will be stored here for further actions
        /// </summary>
	    public static ConHandle clickedConnector;    

        /// <summary>
        /// Background of EditorWindow
        /// </summary>
	    public static Texture2D Background;         

        /// <summary>
        /// Texture Array with all Connector Textures
        /// </summary>
		public static Texture2D[] ConTex;

        /// <summary>
        /// Contains if the TutorialSystem GameObject has been created
        /// </summary>
		public static bool tutSystemCreated = false;

        /// <summary>
        /// A custom context menu with custom skin
        /// </summary>
		public static CustomMenu customMenu;

		/// <summary>
        /// The zoom factor. Can be 0.2, 0.4, 0.6, 0.8 or 1.0. Changed by SideWindow
        /// </summary>
		public static float zoomFactor;

        /// <summary>
        /// The original Unity skin
        /// </summary>
        public static GUISkin originalSkin;

        /// <summary>
        /// The custom TutorialEditor skin
        /// </summary>
        public static GUISkin customSkin;

        /// <summary>
        /// Currently selected group of nodes
        /// </summary>
        public static List<Node> selectedNodes; 

        /// <summary>
        /// Self reference
        /// </summary>
        public static TutorialEditor self;

        /// <summary>
        /// The path of TutorialDesigner in the project folder
        /// </summary>		/// 
        public static string TDPath;

        private bool scrollWindow = false; // If mainWindow is currently in Scroll Mode        
        private Vector2 scrollOffset; // Amount of current Scrolling
        private Vector2 mousePos; // Store current mousePos here for accessing from everywhere
        private Connector reallyClickedConnector; // Connector that was clicked before Mouse Movements
        private bool initialized = false; // If this Class was initialized
        static bool firstLayoutCallDone = false; // Bool for GUI Bug prevention, makes sure that Layout Event comes before Repaint Event.        
		private UnityEngine.SceneManagement.Scene sc; // Another GUI Bug prevention, occurs on Scene change with EditorWindow opened
        private bool moveNodes = false; // If currently the "Move Nodes" mode is active
        private Vector2 oldMousePosition; // Store old Mouse pos for calculating delta when no button is pressed

	    // Standard procedure for adding a menue item and make an Editor Window
	    [MenuItem("Window/Tutorial Designer")]
	    static void ShowEditor()
	    {			
			GetWindow<TutorialEditor>(); 
	    }

	    /// <summary>
        /// Initializes this instance
        /// </summary>
	    public void Initialize() {		
			self = this;

			// Get the Path of TutorialDesigner folder
			if (string.IsNullOrEmpty(TDPath)) {
				string mainScript = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("TutorialEditor")[0]);
				TDPath = mainScript.Substring (0, mainScript.IndexOf ("Scripts"));
			}

			// Load Connector Pics in same order as ConType Enum. That will assign the textures to the type
			ConTex = new Texture2D[] {
                (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(TDPath + "Textures/connector_empty.png"),
                (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(TDPath + "Textures/connector_full.png")
			};		     
	        
            selectedNodes = new List<Node>();

			// Get original Skin. If not there and this is the first Initialization, create it from original GUISkin			
            originalSkin = Object.Instantiate<GUISkin>(GUI.skin);
            customSkin = AssetDatabase.LoadAssetAtPath<GUISkin> (TDPath + "Skins/CustomSkin.guiskin") as GUISkin;

            // Create Replacement for Context menu that will open with right mouse button
			customMenu = new CustomMenu ();

	        // Callback after Undo / Redo was performed
	        Undo.undoRedoPerformed = delegate () { 
				UpdateNodes();

				// Finally Repaint Canvas
				Repaint();
	        };

            // Load Prefabs in Dialogues Folder
            Dialogue.dialoguePrefabs = Utilities.Various.GetFileList (TDPath + "Resources/Dialogues", "prefab", "No Dialogue");
			sideWindow = new SideWindow ();

			initialized = true;   
	    }

        // OnGUI is called for rendering and handling GUI events
	    void OnGUI() {                  
            if (!initialized || ConTex == null || customSkin == null || originalSkin == null || sideWindow == null) {
				Initialize ();	
			}

			// Input Events
			Event currentEvent = Event.current;

			// Zoom Operations
			if (savePoint == null || zoomFactor == 0) {
				// If savepoint.zoomfactor is not available
				zoomFactor = 1f;
			} else {
				// Get zoomfactor from serialized variable
				zoomFactor = savePoint.zoomFactor;
			}

			// Side Window       
			if (sideWindow != null) {									
				sideWindow.Draw (position);
			}

			// Begin scaled GUI Area
			Utilities.EditorZoomArea.Begin (zoomFactor, new Rect(0f, 11.5f, position.width - (sideWindow.opened ? 200f : 25f), position.height));

			// Background
			if (currentEvent.type == EventType.Repaint) {
				DrawBackground ();
			}

			// User Input
			if (!customMenu.visible) InputEvents (currentEvent);

			// Nodes
			if (savePoint != null) {
				if (savePoint.nodes != null) {					
					// Draw Nodes (Windows). Only if the Scene didn't change during Layout Event.
					// That would break the next Repaint Event
					if (savePoint.nodes.Count > 0 && sc == UnityEngine.SceneManagement.SceneManager.GetActiveScene()) {                                            
						// Skin Change
						Utilities.SkinOperations.ChangeSkin(customSkin, currentEvent);

						BeginWindows ();
						for (int i = 0; i < savePoint.nodes.Count; i++) {
                            if (EditorContainsNode(savePoint.nodes[i].rect)) {
							    savePoint.nodes[i].rect = GUILayout.Window (i, // Node ID
																		savePoint.nodes[i].rect, // Node Rect
							                                           	DrawNodes, // Draw Function
							                                           	savePoint.nodes [i].title, // Caption
																	   	savePoint.nodes [i].nodeStyle);  // GUIStyle	
                            }
						}	

                        EndWindows ();

						// Reset Skin back to normal
						Utilities.SkinOperations.ChangeSkin(originalSkin, currentEvent);

						// Draw Connectors of Nodes			
						foreach (Node n in savePoint.nodes) {
							// Draw Connection Knobs only
							n.DrawConnectors ();
						}
					}

					// Draw Curves between Connection Knobs
					DrawCurves ();
				}			
			} else {
				// If savePoint doesn't exist, there is no Tutorial System Object in the Scene
				GameObject tutSysObj = GameObject.Find("TutorialSystem");
				if (tutSysObj != null) {
					savePoint = tutSysObj.GetComponent<SavePoint>();
					tutSystemCreated = true;
					if (savePoint.nodes != null) foreach(Node n in savePoint.nodes) n.ResetSkin();
					if (Application.isPlaying) {
						UpdateEditorView ();
					}
				} else {
					tutSystemCreated = false;
				}
			}

			// End Scaled Area
			Utilities.EditorZoomArea.End();

			// Context Menu
			customMenu.Draw(currentEvent);

			// Prevent a Unity Bug here. If another scene is loaded with EditorWindow open,
			// EditorGUILayout will throw Exceptions if EndWindows() happens before the first Repaint Event
			if (currentEvent.type == EventType.Repaint) {
				sc = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
				Repaint ();
			}
		}

        // Checks if Node is in visible area
        bool EditorContainsNode(Rect nodeRect) {            
            Rect newRect = new Rect(position.min + nodeRect.position * zoomFactor, nodeRect.size * zoomFactor);

            return position.Overlaps(newRect);
        }

	    // Global function pointer for GUI.Window() above. Used by all nodes
	    void DrawNodes(int id) {
			if (UnityEditor.EditorApplication.isPlaying || customMenu.visible)
				GUI.enabled = false;
				
			// Prevent a GUI Bug here. If Repaint Event comes before the first Layout Event, this is gonna fail
			if (!firstLayoutCallDone) {
				if (Event.current.type == EventType.Layout) {
					firstLayoutCallDone = true;
					savePoint.nodes[id].DrawNode ();
				}
			} else {
				savePoint.nodes[id].DrawNode ();
			}

			// Make Node dragable
			if (!customMenu.visible) GUI.DragWindow();
	    }

        // Handles mouse operations
		void InputEvents(Event e) {
	        // Events        
	        mousePos = e.mousePosition;        

            // Mouse Events
	        Node clickedNode = null;       

	        if (e.type == EventType.MouseDown) {
                if (moveNodes) {
                    moveNodes = false;
                    return;
                }

				if (savePoint != null && !EditorApplication.isPlaying) {
					clickedNode = NodeAtPosition (mousePos);

					if (clickedNode == null) {
						clickedConnector = ConAtPosition (mousePos);
                    } else {
                        // Multiselection for nodes
                        if (e.button == 0) {
                            if (!e.shift) {
                                selectedNodes.Clear();
                                Selection.objects = new Object[]{clickedNode};
                            }
                            else {
                                if (selectedNodes.Contains(clickedNode)) {
                                    selectedNodes.Remove(clickedNode);
                                } else {
                                    selectedNodes.Add(clickedNode);
                                }
                                Selection.objects = selectedNodes.ToArray();
                            }
                        }
                        
                        foreach(Node n in savePoint.nodes) {                            
                            if (selectedNodes.Contains(n))
                                n.nodeStyle = customSkin.GetStyle("SelectedWindow");
                            else
                                n.ResetSkin();
                        }
                    }
				}

				if (clickedNode != null && !EditorApplication.isPlaying) {
					// Click on a Node

					if (e.button == 0) {
						// Left Click -> Window Drag. Handled by Unity   
					} else if (e.button == 1) {
						// Right click -> Node delete
                        if (selectedNodes.Count == 0)
                            customMenu.Call(ContextCallback, "Start here", "Delete Node", "Duplicate Node");
                        else 
                            customMenu.Call(ContextCallback, "Move Nodes", "Delete Nodes");
						e.Use ();
					}
				} else if (clickedConnector.active && !EditorApplication.isPlaying) {
					// Click on Connector

					// Store really clicked connector. Because clickedConnector might change below
					reallyClickedConnector = clickedConnector.connector;

					// Break old Connection(s) if there are some
					if (clickedConnector.connector.connections.Count > 0) {
						Undo.RegisterCompleteObjectUndo (clickedConnector.connector, "Disconnect Nodes");
						Connector otherConnector = clickedConnector.connector.Break ();

						if (otherConnector != null) {
							// The Curve Handle should now be detatched from Connector,
							// And still be connected with the Connector on the other Node
							clickedConnector = ConAtPosition (otherConnector.GetRect ().center);
						}
					}
				} else {
					// Click on empty Canvas (no Node, not sideWindow)
					if (e.button == 2 || e.button == 0) {
						// Left/Middle Click -> Start scrolling
						scrollWindow = true;
						e.delta = new Vector2 (0, 0);
					} else if (e.button == 1) {	
						// Right click -> Editor Context Click
						customMenu.Call(ContextCallback, "New Step", "New Event");
						e.Use ();
					}                
				}
            } else if (e.rawType == EventType.MouseUp) {
				// Left/Middle click up
				if (e.button == 2 || e.button == 0) {
					// Connect 2 Nodes, if possible
					if (clickedConnector.active) {
						ConHandle secondHandle = ConAtPosition (mousePos);
						if (secondHandle.active) {
							if (clickedConnector.node != secondHandle.node) {
								// If Mouse Click and Release on the same Connector, connect them but don't record UNDO
								if (secondHandle.connector != reallyClickedConnector) {
									Undo.RegisterCompleteObjectUndo (clickedConnector.connector, "Connect Nodes");
									Undo.RegisterCompleteObjectUndo (secondHandle.connector, "Connect Nodes");
								} 

								// Connect both Connectors to each other.
								clickedConnector.connector.ConnectTo (secondHandle.connector);
							}
						}
					}

					// Stop scrolling
					scrollWindow = false;
					clickedConnector.active = false;
				}
			} 

	        // Scroll Mainwindow
            if (scrollWindow) {
	            // Change Window and Nodes by Mouse delta (difference to last rendered position)
                scrollOffset += e.delta / 2;
				if (savePoint != null) {
		            foreach (Node n in savePoint.nodes) {
		                n.rect.position += e.delta / 2;
		            }
				}
	            Repaint();            
	        } 

            // Move Nodes
            if (moveNodes) {
                // Change Window and Nodes by Mouse delta (difference to last rendered position)
                if (savePoint != null && oldMousePosition != mousePos && selectedNodes.Count > 0) {
                    Vector2 delta = mousePos - oldMousePosition;
                    foreach (Node n in selectedNodes) {
                        n.rect.position += delta;
                    }
                }

                oldMousePosition = mousePos;
                Repaint();            
            }
	    }

        // Background drawing, incl tiling and scrolling
		private void DrawBackground() {
			// Draw Background at current Scrollposition            
			if (Background != null && tutSystemCreated) {			
				Vector2 offset = new Vector2(scrollOffset.x % Background.width - Background.width,
					scrollOffset.y % Background.height - Background.height);
				int tileX = Mathf.CeilToInt((position.width * (1 / zoomFactor) + (Background.width - offset.x)) / Background.width);
				int tileY = Mathf.CeilToInt((position.height * (1 / zoomFactor) + (Background.height - offset.y)) / Background.height);

				for (int x = 0; x < tileX; x++) {
					for (int y = 0; y < tileY; y++) {
						Rect texRect = new Rect(offset.x + x * Background.width,
							offset.y + y * Background.height,
							Background.width, Background.height);
						GUI.DrawTexture(texRect, Background);
					}
				}
			} else {
                Background = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture2D>(TDPath + "Textures/background.png");
			}
		}

        /// <summary>
        /// Draws the curves between Node-Connectors
        /// </summary>	    
	    public void DrawCurves() {
	        if (clickedConnector.active) {            
	            Vector3 startPos = new Vector3(clickedConnector.position.x, clickedConnector.position.y);
	            Vector3 endPos = new Vector3(mousePos.x, mousePos.y);
	            float distance = Mathf.Min(Vector3.Distance(startPos, endPos), 50f);
	            Vector3 startTan = startPos + (clickedConnector.left ? Vector3.left : Vector3.right) * distance;
	            Vector3 endTan = endPos + (clickedConnector.left ? Vector3.right : Vector3.left) * distance;
				Color col = new Color(1f, 1f, 0, 1f);

	            // Curve of currently clicked Connector Handle to Mouse Position
	            Handles.DrawBezier(startPos, endPos, startTan, endTan, col, null, 4f);            
	            Repaint();
	        }

			// Draw all Connections between Nodes here
			if (savePoint != null) {
		        foreach (Node n in savePoint.nodes) {
					foreach (Connector c in n.connectors) {
						if (c != null) {
							// Allways draw from OutNode to InNode	
							if (!c.input && c.connections.Count == 1) { // Output Node (can only have 1 connection)
								if (c.visible) {
					                Vector3 startPos = c.GetRect().center;
					                Vector3 endPos = c.connections[0].GetRect().center;
					                float distance = Mathf.Min(Vector3.Distance(startPos, endPos), 50f);
									Vector3 startTan = startPos + c.getCurveTangent() * distance;
									Vector3 endTan = endPos + c.connections[0].getCurveTangent() * distance;
									// Current Node will be connected with a yellow thick line. To the last active Node.
									Color col = new Color(0.608f, 0.832f, 0.804f, 1f); 
									float thickness = 3f;
									// If 2 connected nodes are marked as active, draw yellow tracking line
									if ((c.homeNode.nodeType & 4) == 4 && (c.connections [0].homeNode.nodeType & 4) == 4) {
										// Don't mark yellow the upcoming way. Could happen if two nodes have 2 different connections
										if ((!n.IsWorking() || IsNodeForeverEvent(n)) && c.connections[0].homeNode.IsWorking()) {
											col = new Color(1f, 1f, 0f, 1f);
											thickness = 5f;
										}
									}

									// If neccessary, switch side of Output Connector to avoid Curve spaghetti
									if (c.homeNode.rect.position.x > c.connections [0].homeNode.rect.position.x) {
										if (c.corner == ConCorner.bottom_right || c.corner == ConCorner.top_right)
											c.SwitchSide();
									} else {
										if (c.corner == ConCorner.bottom_left || c.corner == ConCorner.top_left)
											c.SwitchSide();
									}

					                // Curve of currently clicked Connector Handle
									Handles.DrawBezier(startPos, endPos, startTan, endTan, col, null, thickness);
					                Repaint();
								} else {
									// If not visible but still has 1 connection, it's because Dialogue
									// Buttons have changed, making one connected Con invisible
									c.Break();
								}
							}
						}
					}
		        }
			}
	    }

        /// <summary>
        /// Gets Node at a Mouseclick position
        /// </summary>
        /// <returns>Node at current mouseposition or null</returns>
        /// <param name="pos">Position of mouse cursor</param>	    
	    public Node NodeAtPosition(Vector2 pos) {
	        // Check if we clicked inside a window
	        if (savePoint != null) {
	            for (int i = savePoint.nodes.Count - 1; i >= 0; i--) { // From top to bottom because of the render order (though overwritten by active Window)
	                if (savePoint.nodes[i].rect.Contains(pos)) {
	                    // Click on Node
						savePoint.nodes[i].Activate();
	                    return savePoint.nodes[i];                    
	                } 
	            }
	        }

	        return null;
	    }   

        /// <summary>
        /// Gets Handle struct of clicked Connector of any Node
        /// </summary>
        /// <returns>ConHandle at current mouseposition. If there's no Connector at current position, the returned ConHandle is not active</returns>
        /// <param name="pos">Position of mouse cursor</param>
	    public ConHandle ConAtPosition(Vector2 pos) {
	        ConHandle handle = new ConHandle {
	            active = false
	        };        

			foreach (Node n in savePoint.nodes) {
				foreach (Connector c in n.connectors) {
					if (c.visible && c.GetRect().Contains(pos)) {
						handle.active = true;
						handle.node = n;
						handle.connector = c;
						handle.position = c.GetRect().center;
						handle.left = c.input;
						break;
					}
				}
			}

	        return handle;
	    }

	    // Callback from Context Menu
	    private void ContextCallback(object obj) {
			if (savePoint == null) Initialize();

	        switch (obj.ToString()) {
	            case "New Step":
					Undo.RecordObject(savePoint, "Create Node");
	                StepNode sNode = ScriptableObject.CreateInstance<StepNode>();				
					sNode.Init ();
					sNode.rect.x = mousePos.x;
					sNode.rect.y = mousePos.y;
					sNode.Activate();
				break;
				case "New Event":
					Undo.RecordObject(savePoint, "Create Node");
	                EventNode eNode = ScriptableObject.CreateInstance<EventNode>();				
					eNode.Init ();
					eNode.rect.x = mousePos.x;
					eNode.rect.y = mousePos.y;
				break;
	            case "Delete Node":
	                Node d_node = NodeAtPosition(mousePos);
	                if (d_node != null) {
	                    Undo.RegisterCompleteObjectUndo(savePoint, "Delete Node");						
						// Break opposite connections of this Nodes Connectors
						foreach (Connector c in d_node.connectors) {
							c.BreakOpposite();
						}
						d_node.Remove();
						savePoint.nodes.Remove(d_node);   
                        if (d_node == savePoint.startNode) savePoint.startNode = null;
						Undo.DestroyObjectImmediate(d_node);																		
						UpdateNodes();
	                }
	            break;
                case "Delete Nodes":
                    if (selectedNodes.Count > 0) {
                        Undo.RegisterCompleteObjectUndo(savePoint, "Delete Nodes");
                        foreach (Node node in selectedNodes) {
                            foreach (Connector c in node.connectors) {
                                c.BreakOpposite();
                            }
                            node.Remove();
                            savePoint.nodes.Remove(node);
                            Undo.DestroyObjectImmediate(node);
                        }

                        selectedNodes.Clear();
                        UpdateNodes();                        
                    }
                break;
				case "Duplicate Node":
					Node dup_node = NodeAtPosition(mousePos);
	                if (dup_node != null) {
						Undo.RecordObject(savePoint, "Duplicate Node");					
						Node nodeClone = dup_node.Copy();			
						EditorUtility.CopySerialized(dup_node, nodeClone);	

						nodeClone.Init();
						nodeClone.rect.x = mousePos.x;
						nodeClone.rect.y = mousePos.y;											

						// Special functions depending on node type
						if ((dup_node.nodeType & 1) == 1) { // Stepnode	
							((StepNode)dup_node).dialogue.CopySettingsTo(((StepNode)nodeClone).dialogue);						
							((StepNode)nodeClone).Activate();
						}
					}
				break;
				case "Start here":
					Node start_node = NodeAtPosition (mousePos);
					if (start_node != null) {				
						savePoint.startNode = start_node;
					}
				break;
                case "Move Nodes":
                    moveNodes = true;
                    oldMousePosition = mousePos;
                break;
	        }
	    }
	    
		// Update Node Settings after Undo, deletion etc...
		void UpdateNodes() {			
			// Update glogal Node Count
			if (savePoint != null) {
				if (savePoint.nodes.Count > 0) {
					// Check connections between Nodes
					foreach (Node n in savePoint.nodes) {
                        n.ResetSkin();
                        n.UndoChecks();
						foreach (Connector c in n.connectors) {
							// Mark a connector for removal here. Delete if afterwards if necessary, so the foreach loop doens't break
							Connector conToRemove = ScriptableObject.CreateInstance<Connector>();

							// Restore Homenode of Connector
							if (c.homeNode == null)
								c.homeNode = n;
							
							// Restore Connections between Connectors
							if (c.connections.Count > 0 && c.visible) {
								foreach (Connector oppositeConnector in c.connections) {
									// If opposite Connector does not have this in his connection list
									if (!oppositeConnector.connections.Contains (c)) {
										// Input can have > 1 connections. Output only one!
										if (oppositeConnector.input || (!oppositeConnector.input && oppositeConnector.connections.Count == 0)) {
											oppositeConnector.connections.Add (c);
										} else {
											// If opposite is an Output with > 1 Connections, don't connect this connector to it. Because
											// it already has one connection. Mark it for safe removal after the foreach-loop
											conToRemove = oppositeConnector;
										}
									} else {										
										// If opposite Node was deleted, delete also the connection to it. Otherwise the curve will be drawn into emptiness
										if (oppositeConnector.homeNode == null) {
											conToRemove = oppositeConnector;
										}
									}
								}
							}

                            // Safely remove Connector from List
							c.connections.Remove(conToRemove);
						}
					}
				}
			}
		}

        /// <summary>
        /// Create Gameobject that will be storing and tracking the Nodes system
        /// </summary>
		public static void CreateTutorialSystem() {
			GameObject tut = new GameObject ("TutorialSystem");
			savePoint = tut.AddComponent<SavePoint> ();
			savePoint.Initialize ();
			tutSystemCreated = true;
		}

        /// <summary>
        /// Determines if n is an EventNode wich has "forever" toggeled
        /// </summary>
        /// <param name="n">Any Nodetype</param>
		public static bool IsNodeForeverEvent(Node n) {
			// Check if current Node is an Forever-Event
			bool nodeIsForeverEvent = false;
			if ((n.nodeType & 2) == 2)
			if (((EventNode)n).forEver)
				nodeIsForeverEvent = true;

			return nodeIsForeverEvent;
		}

		// Update Editor while Game is already running and player switches to Tutorial Designer
		private void UpdateEditorView() {			
			if (savePoint != null) if (savePoint.nodes != null) {                 
				foreach (Node n in savePoint.nodes) {
					if (n.IsWorking ()) {                         
						n.nodeStyle = TutorialEditor.customSkin.GetStyle("Activewindow");
						if (n.workingPath == 0) {
							sideWindow.currentNode = n.title + "\n\r" + n.description;
						} else if (n.workingPath > 0) {
							sideWindow.globalNode = n.title + "\n\r" + n.description;
						}
					}
				}
			}
			Repaint ();
		}
	}
}
#endif