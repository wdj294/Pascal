using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SBLS;

[CustomEditor(typeof(SBLSConfiguration))]
public class SBLSSkillEditor : Editor {
	
	private int skillNodeCount;
	private SBLSConfiguration config;
	private bool displayLevels;
#if UNITY_EDITOR
	private List<bool> skillDisplayLevel = new List<bool>();
#endif
	private string[] timeOptions = { "Seconds", "Minutes", "Hours" };

	public void OnEnable() {
		config = target as SBLSConfiguration;
		skillNodeCount = config.skills.Count;
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();
	
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		GUILayout.Label("SBLS Initial Settings", "HeaderLabel");
		EditorGUILayout.Space();
		config.isDefault = EditorGUILayout.Toggle ("Default Configuration", config.isDefault);
		config.useLevelMultiplier = EditorGUILayout.Toggle ("Calculate Next Level XP", config.useLevelMultiplier);
		if (config.useLevelMultiplier) {
			EditorGUI.indentLevel = 2;
			EditorGUILayout.LabelField("Next level XP will be calculated by PreviousLevelXP * Multiplier");
			config.multiplier = EditorGUILayout.FloatField ("Number to multiply by ", config.multiplier);
			EditorGUI.indentLevel = 0;
		}

		EditorGUILayout.Separator ();
		config.levelLimit = EditorGUILayout.IntField ("Limit Levels to", config.levelLimit);

		// Handle levels if not using multiplier
		if (!config.useLevelMultiplier) {
			if (config.levelLimit > config.levels.Count) {
				for (int i = 0; i < config.levelLimit - config.levels.Count; i++) {
					config.levels.Add ( new SBLSLevel());
				}
			}

			// Delete a level
			if(config.levelLimit < config.levels.Count){
				if(EditorUtility.DisplayDialog("Delete levels?","Changing this will remove levels.  This can not be undone.  Are you sure you want to continue?", "OK", "Cancel")){
					int removeCount = config.levels.Count - config.levelLimit;
					config.levels.RemoveRange(config.levels.Count-removeCount,removeCount);
				}else{
					skillNodeCount = config.skills.Count;	
				}
			}

			// Display level
			EditorGUI.indentLevel = 2;
			displayLevels = EditorGUILayout.Foldout(displayLevels, "Show Levels");
			if (displayLevels) {
				EditorGUILayout.LabelField("Enter the XP required to reach each level");
				for (int i = 0; i < config.levels.Count; i++) {
					config.levels[i].xpToReachLevel = EditorGUILayout.IntField("Level "+ (i + 1), config.levels[i].xpToReachLevel);

				}
			}

		}

		EditorGUI.indentLevel = 0;
		EditorGUILayout.Space();
		GUILayout.Label("Skills", "HeaderLabel");
		EditorGUILayout.Space();

		// Display skill numbers
		EditorGUILayout.BeginHorizontal();
			skillNodeCount = Mathf.Max(2, EditorGUILayout.IntField("Number of Skills", skillNodeCount));
		EditorGUILayout.EndHorizontal();
		
		// Check to see if we need to add a skill
		if(skillNodeCount > config.skills.Count){
			for (int i = 0; i < skillNodeCount - config.skills.Count; i++) {
				config.skills.Add(new SBLSSkill("", 1, 0, 100));	
			}
		}

		// Check to see if we need to delete a skill
		if(skillNodeCount < config.skills.Count){
			if(EditorUtility.DisplayDialog("Delete skills?","Changing this will remove skills.  This can not be undone.  Are you sure you want to continue?", "OK", "Cancel")){
				int removeCount = config.skills.Count - skillNodeCount;
				config.skills.RemoveRange(config.skills.Count-removeCount,removeCount);
			}else{
				skillNodeCount = config.skills.Count;	
			}
		}

		////////////////////////////////////////////
		/// Display Skills                        //
		////////////////////////////////////////////
		EditorGUI.indentLevel = 1;
		EditorGUILayout.Separator();
		for (int i = 0; i < config.skills.Count; i++) {
			config.skills[i].skillName = EditorGUILayout.TextField("Skill Name", config.skills[i].skillName);

			config.skills[i].isTimeBased = EditorGUILayout.Toggle("Time based skill", config.skills[i].isTimeBased);

			if (config.skills[i].isTimeBased) {
				EditorGUI.indentLevel = 2;
				EditorGUILayout.LabelField("Time based updates XP based on how long the skill has been used");
				config.skills[i].timeXp = EditorGUILayout.IntField("XP to add", config.skills[i].timeXp);
				config.skills[i].timeUpdateEvery = EditorGUILayout.IntField("Every", config.skills[i].timeUpdateEvery);
				config.skills[i].timeUpdateType = EditorGUILayout.Popup(config.skills[i].timeUpdateType, timeOptions);
				EditorGUI.indentLevel = 1;
			}

			// Check to see if we are using a multiplier
			if (config.useLevelMultiplier) {
				config.skills[i].xpLevelAdjustment = EditorGUILayout.IntField("XP Level Multiplier", config.skills[i].xpLevelAdjustment);
				EditorGUILayout.LabelField("If this multiplier is zero, the value set in the Initial Settings will be used");
			} else {
				// Nope, we aren't.  Let's display all the levels so we can manually set our levels
				// Check to see if we are opening the foldout
				config.skills[i].displayLevels = EditorGUILayout.Foldout(config.skills[i].displayLevels, "Show Levels");

				// Create the levels first
				if (config.skills[i].displayLevels) {
					if (config.levelLimit > config.skills[i].levels.Count) {
						for (int j = 0; j <= config.levelLimit - config.skills[i].levels.Count; j++) {
							config.skills[i].levels.Add ( new SBLSLevel());
						}
					}
				}

				// Delete any unrequired levels
				// Delete a level
				if(config.levelLimit <= config.skills[i].levels.Count){
						int removeCount = config.skills[i].levels.Count - config.levelLimit;
						config.skills[i].levels.RemoveRange(config.skills[i].levels.Count-removeCount,removeCount);
				}

				// After all that we can display the levels....
				// Display level
				EditorGUI.indentLevel = 2;
		
				if (config.skills[i].displayLevels) {
					EditorGUILayout.LabelField("Enter the XP required to reach each level");
					for (int j = 0; j < config.levels.Count; j++) {
						config.skills[i].levels[j].xpToReachLevel = EditorGUILayout.IntField("Level "+ (j + 1) +"/"+ config.skills[i].levels.Count, config.skills[i].levels[j].xpToReachLevel);
						
					}
				}

				EditorGUI.indentLevel = 1;

			}

			// Add a seperator to make it look pretty
			EditorGUILayout.Separator();
		}

		if (GUI.changed) {
			EditorUtility.SetDirty (config);
		}

		serializedObject.ApplyModifiedProperties();

	}

}
