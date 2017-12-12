using UnityEditor;
using UnityEngine;
using SBLS;
using System.Collections;

public class SBLSWindow : EditorWindow {
	private SBLSCharacter character;

	[MenuItem("SBLS/Create New SBLS Configuration")]
	[MenuItem("Assets/Create/SBLS Configuration")]
	public static void CreateConfig() {
		string currentPath = AssetDatabase.GetAssetPath(Selection.activeObject);
		if (!string.IsNullOrEmpty(currentPath) && currentPath.EndsWith("Resources/SBLS/Config")) {
			SBLSEditorUtils.CreateAsset<SBLSConfiguration>("New SBLS Configuration");
		} else {
			SBLSEditorUtils.CreateAsset<SBLSConfiguration>("New SBLS Configuration", "Assets/Resources/SBLS/Config");
		}
		
	}

	[MenuItem("SBLS/Create New SBLS Quest Configuration")]
	[MenuItem("Assets/Create/SBLS Quest Configuration")]
	public static void CreateQuest() {
		string currentPath = AssetDatabase.GetAssetPath(Selection.activeObject);
		if (!string.IsNullOrEmpty(currentPath) && currentPath.EndsWith("Resources/SBLS/Config/Quests")) {
			SBLSEditorUtils.CreateAsset<SBLSQuestConfiguration>("New SBLS Quest Configuration");
		} else {
			SBLSEditorUtils.CreateAsset<SBLSQuestConfiguration>("New SBLS Quest Configuration", "Assets/Resources/SBLS/Config/Quests");
		}
	}

	[MenuItem("SBLS/About SBLS")]
	static void About() {
		EditorUtility.DisplayDialog("SBLS - Skill Based Leveling System",
		                            "Copyright (c) 2015 Tore Studios\n" +
		                            "Version: 1.0\n\n" +
		                            "http://www.torestudios.com/",
		                            "OK");
	}
	
}
