using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using ParticlePlayground;
using ParticlePlaygroundLanguage;

class PlaygroundCreatePresetWindowC : EditorWindow {

	public static GameObject particleSystemObject;
	public static Texture2D particleSystemIcon;
	public static string particleSystemName;
	public static bool childConnected = false;
	
	public static EditorWindow window;
	public static Vector2 scrollPosition;
	
	public int presetOrPublish = 0;
	public int selectedPreset = 0;
	public int selectedCategory = 0;
	public string[] categories;
	public int[] categoryValues;
	public Vector2 presetScroll;

	int totalSelected = 0;

	public bool showError1 = false;
	static bool setToPublishMenu = false;

	public static PlaygroundSettingsC playgroundSettings;
	public static PlaygroundLanguageC playgroundLanguage;
	
	public static void ShowWindow () {
		playgroundSettings = PlaygroundSettingsC.GetReference();
		playgroundLanguage = PlaygroundSettingsC.GetLanguage();
		window = EditorWindow.GetWindow<PlaygroundCreatePresetWindowC>(true, playgroundLanguage.presetWizard);
        window.Show();
	}

	public static void ShowWindowPublish ()
	{
		setToPublishMenu = true;
		ShowWindow();
	}
	
	void OnEnable () {
		Initialize();
	}
	
	public void Initialize () {
		if (PlaygroundParticleWindowC.presetCategories == null)
			return;

		if (Selection.activeGameObject!=null) {
			particleSystemObject = Selection.activeGameObject;
			particleSystemName = Selection.activeGameObject.name;
		}

		List<string> tmpCategories = new List<string>();
		List<int> tmpCategoryValues = new List<int>();
		bool containsResources = false;
		bool containsUncategorized = false;
		for (int i = 0; i<PlaygroundParticleWindowC.presetCategories.Count; i++) {
			tmpCategories.Add (PlaygroundParticleWindowC.presetCategories[i].categoryName);
			tmpCategoryValues.Add (i);
			if (tmpCategories[i] == "Resources")
				containsResources = true;
			if (tmpCategories[i] == "Uncategorized")
				containsUncategorized = true;
		}

		if (!containsResources) {
			tmpCategories.Add ("Resources");
			tmpCategoryValues.Add (tmpCategories.Count-1);
		}

		if (!containsUncategorized) {
			tmpCategories.Add ("Uncategorized");
			tmpCategoryValues.Add (tmpCategories.Count-1);
		}

		categories = tmpCategories.ToArray();
		categoryValues = tmpCategoryValues.ToArray();
	}
	
	void OnGUI () {
		if (window == null)
		{
			this.Close();
			return;
		}

		if (setToPublishMenu)
		{
			presetOrPublish = 1;
			setToPublishMenu = false;
		}
		EditorGUILayout.BeginVertical();
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField(playgroundLanguage.playgroundPresetWizard, EditorStyles.largeLabel, GUILayout.Height(20));
		EditorGUILayout.Separator();
		
		GUILayout.BeginVertical("box");
		int tmpPresetOrPublish = presetOrPublish;
		presetOrPublish = GUILayout.Toolbar (presetOrPublish, new string[]{playgroundLanguage.preset,playgroundLanguage.publish}, EditorStyles.toolbarButton);
		if (presetOrPublish==0)
			EditorGUILayout.HelpBox(playgroundLanguage.presetText, MessageType.Info);
		else
			EditorGUILayout.HelpBox(playgroundLanguage.publishPresetText, MessageType.Info);
		if (tmpPresetOrPublish!=presetOrPublish && presetOrPublish==1)
			RefreshFromPresetList();
		if (presetOrPublish==1) {
			GUILayout.BeginHorizontal();
			EditorGUILayout.Space ();
			if (GUILayout.Button (playgroundLanguage.publishingGuide, EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
				Application.OpenURL("http://polyfied.com/products/particle-playground/playground-resources/publishing-guide/");
			GUILayout.EndHorizontal();
			EditorGUILayout.Separator();
		}
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
		
		if (presetOrPublish==0) {
			EditorGUILayout.PrefixLabel(playgroundLanguage.particleSystem);
		
			// Particle System to become a preset
			GameObject selectedObj = particleSystemObject;
			particleSystemObject = EditorGUILayout.ObjectField(particleSystemObject, typeof(GameObject), true) as GameObject;
			if (particleSystemObject!=selectedObj) {
				
				// Check if this is a Particle Playground System
				if (particleSystemObject!=null) {
				
					// Set new name if user hasn't specified one
					if (particleSystemName=="")
						particleSystemName = particleSystemObject.name;
						
					showError1 = false;
				} else {
					showError1 = true;
				}
			}
		
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(playgroundLanguage.icon);
			particleSystemIcon = EditorGUILayout.ObjectField(particleSystemIcon, typeof(Texture2D), false) as Texture2D;
			GUILayout.EndHorizontal();
			particleSystemName = EditorGUILayout.TextField(playgroundLanguage.nameText, particleSystemName);
			selectedCategory = EditorGUILayout.IntPopup(playgroundLanguage.category, selectedCategory, categories, categoryValues);
			childConnected = EditorGUILayout.Toggle (playgroundLanguage.childConnected, childConnected);
		} else {
			GUILayout.EndHorizontal();
			EditorGUILayout.PrefixLabel(playgroundLanguage.presets);

			// Selection of presets to export
			totalSelected = 0;
			presetScroll = EditorGUILayout.BeginScrollView(presetScroll);
			for (int c = 0; c<PlaygroundParticleWindowC.presetCategories.Count; c++)
			{
				for (int i = 0; i<PlaygroundParticleWindowC.presetCategories[c].presetObjects.Count; i++)
				{
					GUILayout.BeginHorizontal();
					PresetObjectC thisPreset = PlaygroundParticleWindowC.presetCategories[c].presetObjects[i];
					thisPreset.selected = GUILayout.Toggle (thisPreset.selected, "", GUILayout.MaxWidth(24f));
					if (GUILayout.Button (thisPreset.presetImage, EditorStyles.label, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(24)}))
						thisPreset.selected = !thisPreset.selected;
					if (GUILayout.Button (thisPreset.presetObject.name, EditorStyles.label))
						thisPreset.selected = !thisPreset.selected;
					if (thisPreset.selected)
						totalSelected++;
					GUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndScrollView();
		}
		EditorGUILayout.Separator();

		GUI.enabled = presetOrPublish==0 || presetOrPublish==1 && totalSelected>0;
		GUILayout.BeginHorizontal();
		if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
			if (particleSystemName != null && particleSystemName != "")
				particleSystemName = particleSystemName.Trim();
			if (presetOrPublish==0 && particleSystemObject!=null && particleSystemName!="") {
				string tmpName = particleSystemObject.name;
				particleSystemObject.name = particleSystemName;
				CreatePreset();
				particleSystemObject.name = tmpName;
			} else
			if (presetOrPublish==1) {
				CreatePublicPreset();
			}
		}
		EditorGUILayout.Separator();
		if (presetOrPublish==1)
			EditorGUILayout.LabelField(playgroundLanguage.selected+": "+totalSelected+"/"+PlaygroundParticleWindowC.presetNames.Count, EditorStyles.miniLabel, new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.MaxWidth((playgroundLanguage.selected.Length * 6f)+30f)});
		GUILayout.EndHorizontal();
		GUI.enabled = true;
		GUILayout.EndVertical();
		
		// Space for error messages
		if (showError1 && particleSystemObject!=null)
			EditorGUILayout.HelpBox(playgroundLanguage.gameObjectIsNotPlayground, MessageType.Error);
		
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
	}

	public void CreatePreset () {

		if (childConnected) {

			// Try to child all connected objects to the particle system
			PlaygroundParticlesC[] ppScript = particleSystemObject.GetComponentsInChildren<PlaygroundParticlesC>();

			int i=0;
			for (int x = 0; x<ppScript.Length; x++) {
				for (; i<ppScript[x].manipulators.Count; i++)
					if (ppScript[x].manipulators[i].transform.available)
						ppScript[x].manipulators[i].transform.transform.parent = particleSystemObject.transform;
				for (i = 0; i<ppScript[x].paint.paintPositions.Count; i++)
					if (ppScript[x].paint.paintPositions[i].parent)
						ppScript[x].paint.paintPositions[i].parent.parent = particleSystemObject.transform;
				for (i = 0; i<ppScript[x].states.Count; i++)
					if (ppScript[x].states[i].stateTransform)
						ppScript[x].states[i].stateTransform.parent = particleSystemObject.transform;
				if (ppScript[x].sourceTransform)
					ppScript[x].sourceTransform.parent = particleSystemObject.transform;
				if (ppScript[x].worldObject.transform)
					ppScript[x].worldObject.transform.parent = particleSystemObject.transform;
				if (ppScript[x].skinnedWorldObject.transform)
					ppScript[x].skinnedWorldObject.transform.parent = particleSystemObject.transform;
			}
		}

		// Determine if this is resources, uncategorized or categorized
		if (categories[selectedCategory] == "Resources") 
		{
			string pathName = "Assets/"+playgroundSettings.playgroundPath+playgroundSettings.presetPath+particleSystemObject.name+".prefab";
			if (!OkToCreate(pathName)) return;
			GameObject particleSystemPrefab = PrefabUtility.CreatePrefab(pathName, particleSystemObject, ReplacePrefabOptions.ReplaceNameBased);
			AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(particleSystemIcon as UnityEngine.Object), "Assets/"+playgroundSettings.playgroundPath+playgroundSettings.iconPath+particleSystemPrefab.name+".png");
			AssetDatabase.ImportAsset("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.iconPath+particleSystemPrefab.name+".png");
		}
		else if (categories[selectedCategory] == "Uncategorized")
		{
			string pathName = "Assets/"+playgroundSettings.playgroundPath+playgroundSettings.examplePresetPath+particleSystemObject.name+".prefab";
			if (!OkToCreate(pathName)) return;
			PrefabUtility.CreatePrefab(pathName, particleSystemObject, ReplacePrefabOptions.ReplaceNameBased);
		}
		else
		{
			string pathName = "Assets/"+playgroundSettings.playgroundPath+playgroundSettings.examplePresetPath+categories[selectedCategory]+"/"+particleSystemObject.name+".prefab";
			if (!OkToCreate(pathName)) return;
			PrefabUtility.CreatePrefab(pathName, particleSystemObject, ReplacePrefabOptions.ReplaceNameBased);
		}


		// Refresh the project
		AssetDatabase.Refresh();

		// Close window
		window.Close();
	}

	bool OkToCreate (string pathName) {
		if (AssetDatabase.LoadAssetAtPath(pathName, typeof(GameObject))) {
			return (EditorUtility.DisplayDialog (
				playgroundLanguage.presetWithSameNameFound, 
			    particleSystemName+" "+playgroundLanguage.presetWithSameNameFoundText, 
			    playgroundLanguage.yes, 
				playgroundLanguage.no));
		}
		return true;
	}

	int matched = 0;
	public void CreatePublicPreset () {
			
		// List all dependencies
		List<string> presets = new List<string>();
		List<string> presetIcons = new List<string>();
		List<string> presetDependencies = new List<string>();
		for (int i = 0; i<PlaygroundParticleWindowC.presetObjects.Count; i++) {

			// Match the mask to i
			if ((PlaygroundParticleWindowC.presetObjects[i].selected)) {

				// Add the preset object (prefab)
				presets.Add (AssetDatabase.GetAssetPath(PlaygroundParticleWindowC.presetObjects[i].presetObject));

				// Add the icon if existing
				if (PlaygroundParticleWindowC.presetObjects[i].presetImage!=null)
					presetIcons.Add (AssetDatabase.GetAssetPath(PlaygroundParticleWindowC.presetObjects[i].presetImage));

				matched = i;
			}
		}
		if (presets.Count==0)
			return;
		string[] tmpPresetDependencies = AssetDatabase.GetDependencies(presets.ToArray());

		// Check that the operation won't disturb any of the unnecessary files from the framework
		for (int i = 0; i<tmpPresetDependencies.Length; i++) {

			if (!ContainsDisallowedAsset(tmpPresetDependencies[i])) {

				// Add to dependencies list
				presetDependencies.Add(tmpPresetDependencies[i]);
			}
		}

		// Add the icons
		for (int i = 0; i<presetIcons.Count; i++)
			presetDependencies.Add (presetIcons[i]);
		
		// Check that necessary main assets are in
		if (!Contains(presetDependencies, "PlaygroundC.cs"))
			presetDependencies.Add("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.scriptPath+"PlaygroundC.cs");
		if (!Contains(presetDependencies, "PlaygroundParticlesC.cs"))
			presetDependencies.Add("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.scriptPath+"PlaygroundParticlesC.cs");
		if (!Contains(presetDependencies, "Playground Manager"))
			presetDependencies.Add("Assets/"+playgroundSettings.playgroundPath+"Resources/Playground Manager.prefab");

		// Add necessary extension components
		if (!Contains(presetDependencies, "PlaygroundSpline.cs"))
			presetDependencies.Add("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.extensionPath+"Playground Splines/"+"PlaygroundSpline.cs");

		// Add modular extension components
		if (Contains(presetDependencies, "PlaygroundTrails.cs"))
		{
			if (!Contains(presetDependencies, "ParticlePlaygroundTrail.cs"))
				presetDependencies.Add("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.extensionPath+"Playground Trails/Playground Trail Assets/Scripts/"+"ParticlePlaygroundTrail.cs");
			if (!Contains(presetDependencies, "ParticlePlaygroundTrailPoint.cs"))
				presetDependencies.Add("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.extensionPath+"Playground Trails/Playground Trail Assets/Scripts/"+"ParticlePlaygroundTrailPoint.cs");
			if (!Contains(presetDependencies, "PlaygroundTrailParent.cs"))
				presetDependencies.Add("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.extensionPath+"Playground Trails/Playground Trail Assets/Scripts/"+"PlaygroundTrailParent.cs");
		}
		if (Contains(presetDependencies, "PlaygroundRecorder.cs"))
		{
			if (!Contains(presetDependencies, "PlaygroundCompression.cs"))
				presetDependencies.Add("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.extensionPath+"Playground Recorder/Scripts/"+"PlaygroundCompression.cs");
			if (!Contains(presetDependencies, "PlaygroundRecorderData.cs"))
				presetDependencies.Add("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.extensionPath+"Playground Recorder/Scripts/"+"PlaygroundRecorderData.cs");
			if (!Contains(presetDependencies, "RecordedFrame.cs"))
				presetDependencies.Add("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.extensionPath+"Playground Recorder/Scripts/"+"RecordedFrame.cs");
			if (!Contains(presetDependencies, "SerializedFrame.cs"))
				presetDependencies.Add("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.extensionPath+"Playground Recorder/Scripts/"+"SerializedFrame.cs");
			if (!Contains(presetDependencies, "SerializedParticle.cs"))
				presetDependencies.Add("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.extensionPath+"Playground Recorder/Scripts/"+"SerializedParticle.cs");
		}
		
		// Create a package
		string assetPackagePath = EditorUtility.SaveFilePanel ("Save Preset", "", presets.Count>1?"Playground Preset Bundle ("+presets.Count+" presets).unitypackage":"Playground Preset - "+PlaygroundParticleWindowC.presetObjects[matched].presetObject.name+".unitypackage", "unitypackage");
		if (assetPackagePath.Length!=0)
			AssetDatabase.ExportPackage(presetDependencies.ToArray(), assetPackagePath, ExportPackageOptions.Interactive);

		// Refresh the project
		AssetDatabase.Refresh();
		
		// Close window
		if (assetPackagePath.Length!=0)
			window.Close();
	}

	public bool ContainsDisallowedAsset (string asset) {
		return 
				asset.Contains("PlaygroundBrushPresetInspectorC.cs") || 
				asset.Contains("PlaygroundCreateBrushWindowC.cs") || 
				asset.Contains("PlaygroundCreatePresetWindowC.cs") || 
				asset.Contains("PlaygroundInspectorC.cs") || 
				asset.Contains("PlaygroundParticleSystemInspectorC.cs") ||
				asset.Contains("PlaygroundParticleWindowC.cs") ||
				asset.Contains("PlaygroundBrushPresetC.cs") ||
				asset.Contains("PlaygroundSplineInspector.cs") ||
				asset.Contains("PlaygroundRecorderInspector.cs") ||
				asset.Contains("PlaygroundTrailInspector.cs");
	}

	public bool Contains (List<string> list, string s)
	{
		for (int i = 0; i<list.Count; i++)
			if (list[i].Contains(s))
				return true;
		return false;
	}
	
	public void RefreshFromPresetList () {
		if (PlaygroundParticleWindowC.presetNames.Count==0) return;
		particleSystemIcon = AssetDatabase.LoadAssetAtPath(playgroundSettings.playgroundPath+playgroundSettings.iconPath+PlaygroundParticleWindowC.presetNames[selectedPreset]+".png", typeof(Texture2D)) as Texture2D;
		particleSystemName = PlaygroundParticleWindowC.presetNames[selectedPreset];
	}
}