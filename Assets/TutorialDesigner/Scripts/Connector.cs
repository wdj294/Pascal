using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TutorialDesigner
{
	/// <summary>
	/// Definition of the Connector's align position. Can be aligned to one of 4 corners of the Node's rect
	/// </summary>
	public enum ConCorner { top_left, top_right, bottom_left, bottom_right };

	/// <summary>
	/// A Connector is an important part of a Node. It can hold connections to other Nodes.
	/// There are 2 types. Input- and Output Connectors, which mean the direction of processing.
	/// F. I.: If Node1 should forward to Node2 -> Output of Node1 has to be connected to Input of Node2.
	/// An Input can hold multiple connections, while Output can only hold one.
	/// </summary>
	public class Connector : ScriptableObject{
		/// <summary>
		/// The Node that belongs to the Connector
		/// </summary>
		public Node homeNode;

		/// <summary>
		/// The connections that the Conenctor holds
		/// </summary>
		public List<Connector> connections;

		#if UNITY_EDITOR
		/// <summary>
		/// Visibility of the Connector. Used at Nodes where some Connectors are optional
		/// </summary>
		public bool visible;

		/// <summary>
		/// true = Input; false = Output
		/// </summary>
		public bool input;

		/// <summary>
		/// Drawing position of the Connector on EditorWindow
		/// </summary>
		public Rect rect;

		/// <summary>
		/// To which corner the Connector aligns
		/// </summary>
		public ConCorner corner;

		/// <summary>
		/// Basic Initialization
		/// </summary>
		public void Init() {
			connections = new List<Connector> ();
			visible = true;
		}

		/// <summary>
		/// Advanced Initialization
		/// </summary>
		/// <param name="c">Connector Alignment on its homeNode</param>
		/// <param name="r">Drawing position, relative to homeNode</param>
		/// <param name="n">Connector's homeNode</param>
		/// <param name="i">Input or Output. <c>true</c> - Input; <c>false</c> - Output</param>
		public void Init (ConCorner c, Rect r, Node n, bool i) {
			Init();
			corner = c;
			rect = r;
			homeNode = n;
			input = i;
		}

		/// <summary>
		/// Routine where connector is drawn onto EditorWindow
		/// </summary>
		public void Draw() {
			// If not visible, stop here
			if (!visible) return;

			// Check if this Connector has something connected. Important for Texture
			bool hasConnection = false;
			if (connections != null) {			
				if (connections.Count > 0) hasConnection = true;
			}

			// Draw different textures for Connectors with, and without Connections
			GUI.DrawTexture(GetRect (), TutorialEditor.ConTex[hasConnection ? 1 : 0]);
		}

		/// <summary>
		/// Get Rect with absolute Coordinates in the EditorWindow
		/// </summary>
		/// <returns>Connectors drawing position</returns>
		public Rect GetRect() {
			Rect r = new Rect (
				homeNode.rect.x + rect.x,
				homeNode.rect.y + rect.y,
				rect.width, rect.height);
			
			switch (corner) {
				case ConCorner.top_left:
					// No offset at x or y
				break;
				case ConCorner.top_right:
					// No y offset
					r.x += homeNode.rect.width;				
				break;
				case ConCorner.bottom_left:
					// No x offset
					r.y += homeNode.rect.height;
				break;
				case ConCorner.bottom_right:				
					r.x += homeNode.rect.width;
					r.y += homeNode.rect.height;
				break;
			}

			return r;
		}

		/// <summary>
		/// Break Connection of this one. Works only if it has one single Connection. For multiple, use BreakOpposite
		/// </summary>
		/// <returns>If avaliable, return other Connector of this Connection</returns>
		public Connector Break() {
			Connector otherConnector = null;
			if (connections.Count == 1) {
				otherConnector = connections[0];

				if (input) {
					// otherConnector is an Output Connector
					otherConnector.connections.Clear(); // Remove it's only connection
					this.connections.Remove(otherConnector); // Remove the Output C. from this Input C.
				} else {
					// otherConnector is an Input Connector
					otherConnector.connections.Remove(this); // Remove this Output C. from the Input C.
					this.connections.Clear(); // Remove our only connection
				}
			}		
			return otherConnector;
		}

		/// <summary>
		/// Forced breaking of all Connections of this one
		/// </summary>
		public void BreakOpposite() {
			foreach (Connector c in connections) {
				c.connections.Remove(this);
			}
		}

		/// <summary>
		/// Connect this one to another Connector. Works in both ways
		/// </summary>
		/// <param name="otherConnector">Can be an Input- or Output Connector</param>
		public void ConnectTo(Connector otherConnector) {
			if (this.input && !otherConnector.input && !this.connections.Contains (otherConnector)) {
				// Input -> Output Connectors
				this.connections.Add(otherConnector);
				if (otherConnector.connections.Count == 1) otherConnector.connections[0].Break(); // Clear Outputs. Max 1 Output!
				otherConnector.connections.Clear();
				otherConnector.connections.Add(this);
			} else if (!this.input && otherConnector.input && !otherConnector.connections.Contains(this)) {
				//Output -> Input Connectors
				otherConnector.connections.Add(this);
				this.connections.Clear(); // Clear Outputs. Max 1 Output!
				this.connections.Add(otherConnector);		
			}
		}

		/// <summary>
		/// Mirrors this Connector horizontaly to the other side of the Node. Used for
		/// preventing curve spaghetti, where connection curves run all over the Node
		/// </summary>
		public void SwitchSide() {
			rect.x = -rect.x - rect.width;

			switch (corner) {
				case ConCorner.bottom_left:
					corner = ConCorner.bottom_right;
				break;
				case ConCorner.bottom_right:
					corner = ConCorner.bottom_left;
				break;
				case ConCorner.top_right:
					corner = ConCorner.top_left;
				break;
				case ConCorner.top_left:
					corner = ConCorner.top_right;
				break;
			}
		}

		/// <summary>
		/// Gets the curve tangent. Used for smooth out connection curves
		/// </summary>
		/// <returns>Either Vector3.left or Vector3.right. Depending on ConCorner</returns>
		public Vector3 getCurveTangent() {
			return (corner == ConCorner.bottom_left || corner == ConCorner.top_left) ?
				Vector3.left : Vector3.right;
		}

		#endif
	}
}