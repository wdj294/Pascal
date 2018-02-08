using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TutorialDesigner
{
	/// <summary>
	/// Base Class for all types of Nodes
	/// </summary>
	public abstract class Node : ScriptableObject{

		/// <summary>
		/// The ID of a working path. This is supposed to "group" Nodes that are connected.
		/// If one Node moves on to the next, it will get the same ID. This allows tracing
		/// back the way through Nodes on this working path 
		/// </summary>
		public int workingPath;

		/// <summary>
		/// A List of connectors of this Node
		/// </summary>
		public List<Connector> connectors;

		/// <summary>
		/// Optional delay after the Node gets activated, and before it starts working
		/// </summary>
		public float delay;

		/// <summary>
		/// The working process of the Node that happens during gamePlay
		/// </summary>
		protected IEnumerator workRoutine;

		#if UNITY_EDITOR
		/// <summary>
		/// Rect of the Node's draw position in Unity's EditorWindow
		/// </summary>
		public Rect rect;

		/// <summary>
		/// Title of the Node
		/// </summary>
	    public string title = "";

		/// <summary>
		/// The User can give the Node a description
		/// </summary>
	    public string description = "Description";

		/// <summary>
		/// Nodes consist of GUILayout.Window, which is customized by this style
		/// </summary>
		public GUIStyle nodeStyle;

		/// <summary>
		/// The type of the node
		/// </summary>
		public byte nodeType; // Bit1=StepNode (1), Bit2=EventNode (2), Bit3=CurrentNode (4)

		/// <summary>
		/// Basic Initialization of Nodes. This goes for all types and can be extended by children
		/// </summary>
		public virtual void Init() {
			rect = new Rect ();        
			connectors = new List<Connector> ();
			if (TutorialEditor.savePoint != null) {
				TutorialEditor.savePoint.nodes.Add (this);
			}
		}

        /// <summary>
        /// Inits the node after importing from JSON
        /// </summary>
		public abstract void InitAfterImport();

	    // Draw function, called by Editor
	    public virtual void DrawNode() {				
			// Overriden
	    }

		/// <summary>
		/// Draws all connectors of the Node into EditorWindow
		/// </summary>
		public virtual void DrawConnectors() {    
			foreach (Connector c in connectors) {
				if (c != null) c.Draw();
			}
	    }

		/// <summary>
		/// Function for duplicating the Node. Must be overridden by specific Node Class
		/// </summary>
		public abstract Node Copy();

		/// <summary>
		/// Function for removing the Node. Must be overridden by specific Node Class
		/// </summary>
		public abstract void Remove();

		/// <summary>
		/// Sets the Node active in UnityEditor and shows its parameters in Inspector
		/// </summary>
		public virtual void Activate() {		
			UnityEditor.Selection.activeObject = this;
		}

		/// <summary>
		/// Sets the default nodeStyle. F.i. after it was active during gameplay
		/// </summary>
		public abstract void ResetSkin();

        /// <summary>
        /// Routines to check after an Undo or Redo has been done
        /// </summary>
        public abstract void UndoChecks();
		#endif

		/// <summary>
		/// Work process that will happen during the Game. Must be overridden
		/// </summary>
		/// <param name="sp">Reference to SavePoint with wich Node will communicate (SavePoint.NodeControl)</param>
		public abstract IEnumerator Work(SavePoint sp);

		/// <summary>
		/// Starts the working process
		/// </summary>
		/// <param name="sp">Reference to SavePoint</param>
		public void StartWorking(SavePoint sp) {
			if (workRoutine != null) StopWorking (sp);
			workRoutine = Work (sp);
			sp.StartCoroutine (workRoutine);
		}

		/// <summary>
		/// Starts the working process
		/// </summary>
		/// <param name="sp">Reference to SavePoint</param>
		public void StopWorking(SavePoint sp) {
			if (workRoutine != null) {
				sp.StopCoroutine (workRoutine);
				workRoutine = null;
			}
		}

		/// <summary>
		/// Determines whether this Node is working
		/// </summary>
		/// <returns><c>true</c> if this Node is working; otherwise, <c>false</c>.</returns>
		public bool IsWorking() {
			return (workRoutine != null);
		}

	}
}