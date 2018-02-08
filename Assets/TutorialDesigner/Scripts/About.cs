#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace TutorialDesigner {
	
	// Help Window with Support Links. Opened by SideWindow
	public class About : EditorWindow {

	    void OnGUI() {
	        float heightStack = 0f;

	        EditorGUI.LabelField(new Rect(10f, heightStack += 10f, 250f, 20f), "Tutorial Designer - ©Rebound Games", EditorStyles.boldLabel);

	        if (GUI.Button(new Rect(10f, heightStack += 30f, 150f, 20f), "Website")) {
	            Application.OpenURL("https://www.rebound-games.com/?page_id=1041");
	        }

	        if (GUI.Button(new Rect(10f, heightStack += 25f, 150f, 20f), "Code Reference")) {
	            Application.OpenURL("https://www.rebound-games.com/docs/tdesigner");
	        }

	        if (GUI.Button(new Rect(10f, heightStack += 25f, 150f, 20f), "YouTube")) {
	            Application.OpenURL("https://www.youtube.com/user/ReboundGamesTV");
	        }

	        if (GUI.Button(new Rect(10f, heightStack += 25f, 150f, 20f), "Twitter")) {
	            Application.OpenURL("https://twitter.com/Rebound_G");
	        }

	        EditorGUI.LabelField(new Rect(10f, heightStack += 30f, 260f, 20f), "Docs: TutorialDesigner/Documentation.pdf");
	        EditorGUI.LabelField(new Rect(10f, heightStack += 30f, 260f, 20f), "Support:", EditorStyles.boldLabel);

	        if (GUI.Button(new Rect(10f, heightStack += 20f, 150f, 20f), "Support Forum")) {
	            Application.OpenURL("https://www.rebound-games.com/forum/index.php?board=11.0");
	        }

	        if (GUI.Button(new Rect(10f, heightStack += 25f, 150f, 20f), "Unity Forum")) {
	            Application.OpenURL("https://forum.unity3d.com/threads/437506/");
	        }

	        EditorGUI.LabelField(new Rect(10f, heightStack += 35f, 250f, 20f), "Support us!", EditorStyles.boldLabel);
	        if (GUI.Button(new Rect(10f, heightStack += 20f, 150f, 20f), "Review")) {
	            Application.OpenURL("https://goo.gl/5qE4Uk");
	        }
	    }
	}

}
#endif