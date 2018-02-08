#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TutorialDesigner
{
	/// <summary>
	/// Class for creating a custom Context Menu in the Editor.
	/// </summary>
	public class CustomMenu {	
		private Rect rect;
		private float buttonHeight = 20f;
		private List<Rect> buttonRects;
		private List<string> buttonCaptions;
		GUIStyle menu, butt, buttHover;

		/// <summary>
		/// Visibility of the Context Menu
		/// </summary>
		public bool visible = false;
		public delegate void MenuCallback(object obj);
		private MenuCallback callbackReference;

		/// <summary>
		/// Initializes a new instance of the <see cref="TutorialDesigner.CustomMenu"/> class
		/// </summary>
		public CustomMenu() {	
			menu = new GUIStyle ();
			menu.normal.background = Utilities.SkinOperations.ColorToTex (new Color (0.2f, 0.2f, 0.2f, 0.7f));

			buttonRects = new List<Rect> ();
			buttonCaptions = new List<string> ();

			butt = new GUIStyle ();
			butt.normal.textColor = Color.white;
			butt.padding = new RectOffset (5, 0, 3, 3);

			buttHover = new GUIStyle (butt);
			buttHover.normal.background = Utilities.SkinOperations.ColorToTex (new Color(0.5f, 0.5f, 0.5f, 0.5f));
		}

		/// <summary>
		/// Creates and displayes a Context Menu with a variable list of items
		/// </summary>
		/// <param name="menuCallback">Menu callback delegate. It gets the clicked Menu item</param>
		/// <param name="items">An optional array of strings. They will be Menu items</param>
		public void Call(MenuCallback menuCallback, params string[] items) {
			buttonRects.Clear ();
			buttonCaptions.Clear ();
			if (TutorialEditor.savePoint == null) return;

			rect = new Rect (Event.current.mousePosition * TutorialEditor.savePoint.zoomFactor, new Vector2 (150f, buttonHeight * items.Length));
			// Configure Buttons
			for (int i=0; i<items.Length; i++) {
				buttonRects.Add(new Rect (rect.x, rect.y + (buttonHeight * i), rect.width, buttonHeight));
				buttonCaptions.Add (items [i]);
			}
			callbackReference = menuCallback;

			// EditorWindow needs to trigger OnGUI also on mouseMove while ContextMenu is open - for hover effect
			TutorialEditor.self.wantsMouseMove = true;
			visible = true;
		}

		/// <summary>
		/// Draws the created Context Menu
		/// </summary>
		/// <param name="e">This is used by the Main Editor, it passes the current Event, so it can be checked here</param>
		public void Draw(Event e) {		
			if (!visible) return;

			GUI.Box (rect, "", menu);
			for (int i=0; i<buttonRects.Count; i++) {			
				if (GUI.Button (buttonRects [i], buttonCaptions [i], buttonRects [i].Contains (e.mousePosition) ? buttHover : butt)) {
					if (e.button == 0) {
						callbackReference (buttonCaptions [i]);
						Close ();
					}
				}
			}
			TutorialEditor.self.Repaint ();

			if (e.type == EventType.MouseDown) {							
				Close ();
			}
		}

		/// <summary>
		/// Closes the Context menu
		/// </summary>
		private void Close () {
			TutorialEditor.self.wantsMouseMove = false;
			visible = false;
		}
	}
}
#endif