using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using SBLS;

namespace SBLS {
	[System.Serializable]
	public class SBLSConfiguration : ScriptableObject {

		// Skills
		public List<SBLSSkill> skills = new List<SBLSSkill>();
		// Overall Levels
		public List<SBLSLevel> levels = new List<SBLSLevel> ();
		// Do we use a multiplier to determine the next level xp?
		public bool useLevelMultiplier = true;
		// Number to multiply by for next level xp
		public float multiplier = 1.25f;
		// First Level XP if using multiplier
		public int firstLevelXp;
		// Maximum number of levels for character and skills
		public int levelLimit = 50;

		// For default configs
		public bool defaultConfig;

		public bool isDefault
		{
			get {
				return defaultConfig;
			}

			set {
				defaultConfig = value;

				if (value) {
					disableOtherConfigs();
				}
			}
		}

		// If we are setting this as default we want to disable the others
		private void disableOtherConfigs() 
		{
			var allConfigs = FindAll();
			foreach (var confs in allConfigs) {
				if (confs != this) {
					confs.isDefault = false;
				}
			}
		}


		// Find all other configs
		public static SBLSConfiguration[] FindAll() {
			List<SBLSConfiguration> results = new List<SBLSConfiguration>();
			var allConfigs = Resources.LoadAll("SBLS/Config", typeof(SBLSConfiguration));
			
			foreach (var confs in allConfigs) {
				results.Add(confs as SBLSConfiguration);
			}
			
			return results.ToArray();
		}
		
		private void dirty() {
#if UNITY_EDITOR
EditorUtility.SetDirty(this);
#endif
		}

	}
}
