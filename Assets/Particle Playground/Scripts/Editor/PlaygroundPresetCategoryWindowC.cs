using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using ParticlePlayground;
using ParticlePlaygroundLanguage;

public class PlaygroundPresetManagerWindowC : EditorWindow {

	public static EditorWindow window;
	Vector2 scrollPosition;
	string createCategoryName = "New Category";
	string[] categoryNames;
	char[] invalidChars;

	public static PlaygroundSettingsC playgroundSettings;
	public static PlaygroundLanguageC playgroundLanguage;

	public static void ShowWindow () 
	{
		playgroundSettings = PlaygroundSettingsC.GetReference();
		playgroundLanguage = PlaygroundSettingsC.GetLanguage();
		window = EditorWindow.GetWindow<PlaygroundPresetManagerWindowC>(true, playgroundLanguage.presetManager);
		window.Show();
	}

	void OnEnable () 
	{
		Initialize();
	}

	public void Initialize ()
	{
		if (PlaygroundParticleWindowC.presetCategories == null)
			return;
		invalidChars = Path.GetInvalidPathChars();
	}

	bool requestFocusControl = false;
	string focusControlName = "";

	void OnGUI ()
	{
		if (window == null)
		{
			this.Close();
			return;
		}

		EditorGUILayout.BeginVertical();

		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField(playgroundLanguage.presetManager, EditorStyles.largeLabel, GUILayout.Height(20));
		EditorGUILayout.Separator();

		GUILayout.BeginVertical("box");
		EditorGUILayout.HelpBox(playgroundLanguage.categoriesInfo, MessageType.Info);
		EditorGUILayout.Separator();

		for (int i = 0; i<PlaygroundParticleWindowC.presetCategories.Count; i++)
		{
			GUILayout.BeginVertical("box");

			GUILayout.BeginHorizontal();

			PresetCategory thisCategory = PlaygroundParticleWindowC.presetCategories[i];

			bool isProtectedFolder = thisCategory.categoryName == "Uncategorized" || thisCategory.categoryName == "Resources";

			// Category unfold
			thisCategory.foldout = GUILayout.Toggle(thisCategory.foldout, "", EditorStyles.foldout, GUILayout.MaxWidth(16f));

			// Category name
			GUI.enabled = !isProtectedFolder;
			GUI.SetNextControlName (thisCategory.categoryName);
			thisCategory.tmpNewName = EditorGUILayout.TextField(thisCategory.tmpNewName, EditorStyles.toolbarTextField);
			GUI.enabled = true;
			if (!isProtectedFolder && thisCategory.tmpNewName != thisCategory.categoryName)
			{
				if (GUILayout.Button (playgroundLanguage.rename, GUILayout.ExpandWidth(false)))
				{
					bool isValidFolderName = IsValidFolderName(thisCategory.tmpNewName);
					if (isValidFolderName)
					{
						if (AssetDatabase.RenameAsset("Assets/"+thisCategory.categoryLocation, thisCategory.tmpNewName).Length == 0)
						{
							AssetDatabase.Refresh();

							// Succesfully renamed
							thisCategory.categoryName = thisCategory.tmpNewName;
							requestFocusControl = true;
							focusControlName = thisCategory.categoryName;
							return;
						}
						else
						{
							thisCategory.tmpNewName = thisCategory.categoryName;
						}
					}
					else
					{
						thisCategory.tmpNewName = thisCategory.categoryName;
					}
				}
			}

			// Show amount of selected presets
			int selectedPresetsInCategory = 0;
			for (int x = 0; x<thisCategory.presetObjects.Count; x++)
			{
				if (thisCategory.presetObjects[x].selected)
					selectedPresetsInCategory++;
			}
			GUI.enabled = selectedPresetsInCategory > 0;
			GUILayout.Label("("+selectedPresetsInCategory.ToString()+"/"+thisCategory.presetObjects.Count.ToString()+")");
			GUI.enabled = true;

			EditorGUILayout.Separator();

			// Remove category
			GUI.enabled = !isProtectedFolder;
			if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)}) && !isProtectedFolder){
				if (EditorUtility.DisplayDialog(playgroundLanguage.removeCategory, 
				                                thisCategory.categoryName+" "+playgroundLanguage.removeCategoryText, 
				                                playgroundLanguage.yes, 
				                                playgroundLanguage.no)) 
				{
					// Move all presets contained to "Uncategorized" (parent folder) before removing the category folder
					string[] presetsInCategory = Directory.GetFiles (Application.dataPath+"/"+thisCategory.categoryLocation);
					foreach (string presetLoc in presetsInCategory)
					{
						string convertedPresetPath = presetLoc.Substring(Application.dataPath.Length-6);
						Object presetPathObject = (Object)AssetDatabase.LoadAssetAtPath(convertedPresetPath, typeof(Object));
						if (presetPathObject!=null && (presetPathObject.GetType().Name)=="GameObject") 
						{
							AssetDatabase.MoveAsset(convertedPresetPath, "Assets/"+playgroundSettings.playgroundPath+playgroundSettings.examplePresetPath+presetPathObject.name);
						}
					}
					AssetDatabase.MoveAssetToTrash("Assets/"+thisCategory.categoryLocation);
					AssetDatabase.Refresh();
					GUI.FocusControl(null);
					return;
				}
			}
			GUI.enabled = true;

			GUILayout.EndHorizontal();

			// List of presets
			if (thisCategory.foldout)
			{
				EditorGUILayout.Separator();

				// Mixed selection
				if (thisCategory.presetObjects.Count>0)
				{
					EditorGUI.showMixedValue = selectedPresetsInCategory > 0 && selectedPresetsInCategory < thisCategory.presetObjects.Count;
					bool multiSelect = selectedPresetsInCategory > 0;
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space (4f);
					multiSelect = EditorGUILayout.Toggle (multiSelect);
					EditorGUILayout.EndHorizontal();
					if (EditorGUI.EndChangeCheck())
					{
						for (int x = 0; x<thisCategory.presetObjects.Count; x++)
						{
							thisCategory.presetObjects[x].selected = multiSelect;
						}
					}
					EditorGUI.showMixedValue = false;
				}

				EditorGUI.indentLevel++;

				if (thisCategory.presetObjects.Count>0)
				{
					for (int x = 0; x<thisCategory.presetObjects.Count; x++)
					{
						GUILayout.BeginHorizontal("box");
						PresetObjectC thisPreset = thisCategory.presetObjects[x];
						thisPreset.selected = GUILayout.Toggle (thisPreset.selected, "", GUILayout.MaxWidth(24f));
						if (GUILayout.Button (thisPreset.presetImage, EditorStyles.label, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(24)}) ||
						    GUILayout.Button (thisPreset.presetObject.name, EditorStyles.label))
						{
							EditorGUIUtility.PingObject(thisPreset.presetObject);
						}
						GUILayout.EndHorizontal();
					}
				}
				else
				{
					GUI.enabled = false;
					GUILayout.Label (playgroundLanguage.empty);
					GUI.enabled = true;
				}
				EditorGUI.indentLevel--;
			}

			GUILayout.EndVertical();
		}

		// New category creation
		EditorGUILayout.Separator();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) 
		{
			if (IsValidFolderName(createCategoryName))
			{
				createCategoryName = createCategoryName.Trim();
				string createdFolder = AssetDatabase.CreateFolder("Assets/"+playgroundSettings.playgroundPath+(playgroundSettings.examplePresetPath).Substring(0, playgroundSettings.examplePresetPath.Length-1), createCategoryName);
				if (createdFolder != null && createdFolder.Length > 0)
				{
					AssetDatabase.Refresh();
					foreach (PresetCategory cat in PlaygroundParticleWindowC.presetCategories)
					{
						if (AssetDatabase.AssetPathToGUID("Assets/"+cat.categoryLocation) == createdFolder)
						{
							cat.foldout = true;
							requestFocusControl = true;
							focusControlName = cat.categoryName;
							break;
						}
					}
					createCategoryName = "New Category";
					return;
				}
			}
		}
		createCategoryName = EditorGUILayout.TextField(createCategoryName, EditorStyles.toolbarTextField);
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();

		GUILayout.EndScrollView();

		int selectedPresets = 0;
		bool isResourcesPresetsOnly = true;
		for (int i = 0; i<PlaygroundParticleWindowC.presetCategories.Count; i++)
		{
			for (int x = 0; x<PlaygroundParticleWindowC.presetCategories[i].presetObjects.Count; x++)
			{

				if (PlaygroundParticleWindowC.presetCategories[i].presetObjects[x].selected)
				{
					if (PlaygroundParticleWindowC.presetCategories[i].presetObjects[x].example)
						isResourcesPresetsOnly = false;
					selectedPresets++;
				}
			}
		}

		// Bottom toolbar for selected presets
		if (selectedPresets > 0)
		{
			EditorGUILayout.BeginHorizontal("box");
			EditorGUILayout.LabelField(playgroundLanguage.selected+": "+selectedPresets.ToString(), GUILayout.MaxWidth(90));

			// Move selected presets
			if (categoryNames == null || categoryNames.Length != PlaygroundParticleWindowC.presetCategories.Count+1)
			{
				categoryNames = new string[PlaygroundParticleWindowC.presetCategories.Count+1];
				categoryNames[0] = playgroundLanguage.move+"...";
				for (int i = 0; i<PlaygroundParticleWindowC.presetCategories.Count; i++)
					categoryNames[i+1] = PlaygroundParticleWindowC.presetCategories[i].categoryName;
			}
			int selectedMoveCategory = 0;
			EditorGUI.BeginChangeCheck();
			selectedMoveCategory = EditorGUILayout.Popup(selectedMoveCategory, categoryNames, EditorStyles.toolbarPopup);
			if (EditorGUI.EndChangeCheck())
			{
				if (selectedMoveCategory > 0)
				{
					List<Object> movedObjects = new List<Object>();
					foreach (PresetCategory cat in PlaygroundParticleWindowC.presetCategories)
					{
						foreach (PresetObjectC obj in cat.presetObjects)
						{
							if (obj.selected)
							{
								movedObjects.Add(obj.presetObject);
								AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(obj.presetObject), "Assets/"+PlaygroundParticleWindowC.presetCategories[selectedMoveCategory-1].categoryLocation+"/"+obj.presetObject.name);
							}
						}
					}
					AssetDatabase.Refresh();

					// Select the moved objects once again for less confusion
					foreach (PresetCategory cat in PlaygroundParticleWindowC.presetCategories)
						foreach (PresetObjectC obj in cat.presetObjects)
							if (movedObjects.Contains(obj.presetObject))
								obj.selected = true;

					// Open the index, correct if 'Uncategorized' got removed in the process
					int openIndex = selectedMoveCategory-1 < PlaygroundParticleWindowC.presetCategories.Count? selectedMoveCategory-1 : selectedMoveCategory-2;
					PlaygroundParticleWindowC.presetCategories[openIndex].foldout = true;
				}
			}

			GUILayout.Space (4f);
			
			// Convert selected presets to Resources
			GUI.enabled = !isResourcesPresetsOnly;
			if (GUILayout.Button (playgroundLanguage.convertTo+" "+playgroundLanguage.resources, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
			{
				if (EditorUtility.DisplayDialog(playgroundLanguage.convertPresetsIntoResources, 
				                                playgroundLanguage.convertPresetsIntoResourcesText, 
				                                playgroundLanguage.yes, 
				                                playgroundLanguage.no)) 
				{
					List<Object> convertedObjects = new List<Object>();

					foreach (PresetCategory cat in PlaygroundParticleWindowC.presetCategories)
					{
						foreach (PresetObjectC obj in cat.presetObjects)
						{
							if (obj.selected)
							{
								if (!Directory.Exists (Application.dataPath+"/"+playgroundSettings.playgroundPath+playgroundSettings.presetPath))
								{
									Directory.CreateDirectory(Application.dataPath+"/"+playgroundSettings.playgroundPath+playgroundSettings.presetPath);
									AssetDatabase.Refresh();
								}
								convertedObjects.Add (obj.presetObject);
								AssetDatabase.MoveAsset (AssetDatabase.GetAssetPath(obj.presetObject), "Assets/"+playgroundSettings.playgroundPath+playgroundSettings.presetPath + obj.presetObject.name);
							}
						}
					}

					// Select the converted objects once again for less confusion
					foreach (PresetCategory cat in PlaygroundParticleWindowC.presetCategories)
						foreach (PresetObjectC obj in cat.presetObjects)
							if (convertedObjects.Contains(obj.presetObject))
								obj.selected = true;

					PlaygroundParticleWindowC.presetCategories[PlaygroundParticleWindowC.presetCategories.Count-1].foldout = true;
				}
			}
			GUI.enabled = true;

			GUILayout.Space (4f);

			// Remove selected presets
			if (GUILayout.Button (playgroundLanguage.remove, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
			{
				if (EditorUtility.DisplayDialog(playgroundLanguage.removeSelectedPresets, 
				                                playgroundLanguage.removeSelectedPresetsText, 
				                                playgroundLanguage.yes, 
				                                playgroundLanguage.no)) 
				{
					foreach (PresetCategory cat in PlaygroundParticleWindowC.presetCategories)
					{
						foreach (PresetObjectC obj in cat.presetObjects)
						{
							if (obj.selected)
							{
								AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(obj.presetObject));
							}
						}
					}
				}
			}

			GUILayout.Space (4f);

			// Publish selected presets
			if (GUILayout.Button (playgroundLanguage.publish, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
			{
				PlaygroundCreatePresetWindowC.ShowWindowPublish();
			}

			EditorGUILayout.Separator();

			// Deselect selected presets
			if (GUILayout.Button (playgroundLanguage.deselectAll, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
			{
				for (int i = 0; i<PlaygroundParticleWindowC.presetCategories.Count; i++)
					for (int x = 0; x<PlaygroundParticleWindowC.presetCategories[i].presetObjects.Count; x++)
						PlaygroundParticleWindowC.presetCategories[i].presetObjects[x].selected = false;
			}
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndVertical();

		if (requestFocusControl)
		{
			EditorGUI.FocusTextInControl (focusControlName);
			requestFocusControl = false;
		}
	}

	bool IsValidFolderName (string folderName)
	{
		folderName = folderName.Trim();
		return (!string.IsNullOrEmpty(folderName) && folderName.IndexOfAny(invalidChars) < 0);
	}
}
