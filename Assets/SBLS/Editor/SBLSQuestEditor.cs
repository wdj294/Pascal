using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SBLS;

[CustomEditor(typeof(SBLSQuestConfiguration))]
public class SBLSQuestEditor : Editor {

	private int questStepNodeCount;
	private SBLSQuestConfiguration config;
	private SBLSQuest quest;

	public void OnEnable() {
		config = target as SBLSQuestConfiguration;
		questStepNodeCount = config.quest.steps.Count;
		quest = config.quest;
	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		GUILayout.Label("SBLS Quest Settings", "HeaderLabel");
		EditorGUILayout.Space();
		quest.questName = EditorGUILayout.TextField ("Quest Name", quest.questName);
		EditorGUILayout.LabelField ("Quest Description");
		quest.questDescription = EditorGUILayout.TextArea (quest.questDescription);
		quest.requiredLevel = EditorGUILayout.IntField ("Level Required to Start", quest.requiredLevel);
		quest.rewardXp = EditorGUILayout.IntField ("Reward XP", quest.rewardXp);
		
		EditorGUILayout.Separator ();
		GUILayout.Label("Quest Steps", "HeaderLabel");
		EditorGUILayout.BeginHorizontal();
		questStepNodeCount = Mathf.Max(2, EditorGUILayout.IntField("Number of Skills", questStepNodeCount));
		EditorGUILayout.EndHorizontal();

		// Check to see if we need to add a skill
		if(questStepNodeCount > quest.steps.Count){
			for (int i = 0; i < questStepNodeCount - quest.steps.Count; i++) {
				quest.steps.Add(new SBLSQuestStep("", ""));	
			}
		}
		
		// Check to see if we need to delete a skill
		if(questStepNodeCount < quest.steps.Count){
			if(EditorUtility.DisplayDialog("Delete skills?","Changing this will remove skills.  This can not be undone.  Are you sure you want to continue?", "OK", "Cancel")){
				int removeCount = quest.steps.Count - questStepNodeCount;
				quest.steps.RemoveRange(quest.steps.Count-removeCount,removeCount);
			}else{
				questStepNodeCount = quest.steps.Count;	
			}
		}

		////////////////////////////////////////////
		/// Display Skills                        //
		////////////////////////////////////////////

		EditorGUILayout.Separator();
		for (int i = 0; i < quest.steps.Count; i++) {
			EditorGUI.indentLevel = 0;
			EditorGUILayout.LabelField("Step "+ (i+1));

			EditorGUI.indentLevel = 1;
			quest.steps[i].stepName = EditorGUILayout.TextField("Step Name", quest.steps[i].stepName);
			EditorGUILayout.LabelField ("Step Description");
			quest.steps[i].stepDescription = EditorGUILayout.TextArea (quest.steps[i].stepDescription);
			quest.steps[i].active = EditorGUILayout.Toggle("Is Active", quest.steps[i].active);
			quest.steps[i].completed = EditorGUILayout.Toggle("Is Completed", quest.steps[i].completed);
			quest.steps[i].optional  = EditorGUILayout.Toggle("Optional Step (not required to complete quest)", quest.steps[i].optional);
			quest.steps[i].rewardXP = EditorGUILayout.IntField("Reward XP on step completion", quest.steps[i].rewardXP);
			
			// Add a seperator to make it look pretty
			EditorGUILayout.Separator();
		}


		config.quest = quest;
		if (GUI.changed) {
			EditorUtility.SetDirty (config);
		}


		serializedObject.ApplyModifiedProperties();
	}
}
