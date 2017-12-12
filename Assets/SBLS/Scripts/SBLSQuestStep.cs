using UnityEngine;
using System.Collections;
using SBLS;


namespace SBLS {
	[System.Serializable]
	public class SBLSQuestStep {
		public string stepName;
		public string stepDescription;
		public bool   active = false;
		public bool   completed = false;
		public bool   optional = false;
		public int    rewardXP = 0;	


		public SBLSQuestStep(string name, string description) {
			stepName = name;
			stepDescription = description;
		}
	}
}
