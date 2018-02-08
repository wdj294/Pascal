using UnityEngine;
using UnityEditor;

namespace TutorialDesigner
{
	[CustomEditor(typeof(EventNode))]
	/// <summary>
	/// Inspector override for an EventNode. It simplifies the Editor view after clicking on an EventNode.
	/// Doesn't process any user input.
	/// </summary>
	public class EventNodeEditor : Editor {
		
		private EventNode eNode;

		/// <summary>
		/// Inspector Draw call
		/// </summary>
		public override void OnInspectorGUI()
		{
			eNode = (EventNode)target;
			EditorGUILayout.LabelField(eNode.title, EditorStyles.boldLabel);
			EditorGUILayout.LabelField (eNode.description);
			EditorGUILayout.Space ();

			EditorGUILayout.LabelField ("Triggers:");

			for (int i=0; i<eNode.triggerName.Count; i++) {
				string t = "  " + (i + 1) + ") \"";
				t += eNode.triggerName [i] + "\": " + eNode.eventCounter [i];
				EditorGUILayout.LabelField (t);
			}

			if (eNode.delay > 0) {
				EditorGUILayout.Space ();
				EditorGUILayout.LabelField ("Event will wait " + eNode.delay + " sec. afterwards"); 
			}
		}
	}
}