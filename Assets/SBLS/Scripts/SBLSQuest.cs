using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SBLS;

namespace SBLS {
	[System.Serializable]
	public class SBLSQuest {
		public string questName;
		public string questDescription;
		public List<SBLSQuestStep> steps = new List<SBLSQuestStep>();
		public int currentStep = 0;
		public int rewardXp;
		public bool complete = false;
		public bool started  = false;
		public int  requiredLevel = 1;

		public SBLSCharacter character;

		public SBLSQuest (string name, string description) {
			questName = name;
			questDescription = description;
		}

		public string getName() {
			return questName;
		}

		public string getDescription() {
			return questDescription;
		}


		public void activateQuest() {
			started = true;
			steps [0].active = true;
		}

		public void updateQuest() {
			if (currentStep + 1 == steps.Count) {
				if (steps[currentStep].rewardXP > 0) {
					character.adjustXp(steps[currentStep].rewardXP);
				}
				steps[currentStep].active = false;
				steps[currentStep].completed = true;

				completeQuest();
			} else {
				int nextStep = getNextAvailableStepId ();

				if (steps[currentStep].rewardXP > 0) {
					character.adjustXp(steps[currentStep].rewardXP);
				}
				steps[currentStep].active = false;
				steps[currentStep].completed = true;
				currentStep = nextStep;
				steps[currentStep].active = true;
			}
		}


		private int getNextAvailableStepId() {
			for (int i = currentStep + 1; i < steps.Count; i++) {
				if (!steps[i].optional) {
					return i;
				}
			}

			return 0;
		}

		public SBLSQuestStep getCurrentStep() {
			return steps[currentStep];
		}

		void completeQuest() {
			complete = true;
			started = false;
			character.adjustXp (rewardXp);
			character.questIsDone (this);
		}

//		public int[] getOptionalSteps() {
//			int nextStep = getNextAvailableStepId ();
//			int delta = nextStep - currentStep;
//			if (delta > 1) {
//
//			}
//			return 0;
//		}
	}

}