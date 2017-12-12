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
	public class SBLSQuestConfiguration : ScriptableObject {

		// Quest
		public SBLSQuest quest = new SBLSQuest("", "");

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		private void dirty() {
			#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
			#endif
		}
	}
}
