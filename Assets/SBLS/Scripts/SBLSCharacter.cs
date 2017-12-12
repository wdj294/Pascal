using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SBLS {
	[System.Serializable]
	public class SBLSCharacter : MonoBehaviour {
		public bool useDefaultConfig = true;
		public SBLSConfiguration customConfig;


		public int xp;
		public int nextXp;
		public int totalXp;
		public int level = 1;

		[SerializeField]
		private int xpLevelAdjustment;

		private static SBLSConfiguration config;
		public List<SBLSSkill> skills = new List<SBLSSkill>();
		public List<SBLSLevel> levels = new List<SBLSLevel>();
		public List<SBLSQuest> quests = new List<SBLSQuest>();


		void Awake() {
			// Try to load config
			if (useDefaultConfig) {
				config = SBLSUtils.FindDefault();
			} else {
				config = customConfig;
			}

			// If the config was found, let's process it
			if (config) {
				skills = config.skills;
				levels = config.levels;


			} else {
				// Ooops.... Someone didn't do something correctly
				if (useDefaultConfig) {
					throw new UnityException("SBSL: There is no default configuration set.  Make sure you set one as default");
				} else {
					throw new UnityException ("SBSL: Character is set to use custom configuration, but none is set.");
				}
			}

			if (config.useLevelMultiplier) {
				nextXp = level * config.firstLevelXp;
			} else {
				nextXp = levels[level].xpToReachLevel;
			}
		}
		// Use this for initialization
		void Start () {
			foreach (SBLSSkill skill in skills) {
				skill.setCharacter(this);
				skill.setLevel(1);
				skill.setNextXp(skill.levels[skill.level].xpToReachLevel);
			}
		}
		
		// Update is called once per frame
		void Update () {
			// Loop through skills to see if we need to update the time
			foreach (SBLSSkill sk in skills) {
				if (sk.isTimeBased && sk.inUse) {
					sk.updateTime(Time.deltaTime);
				}
			}
		}
		
		public void setXp(int newXp) {
			xp = newXp;
			updateLevel ();
		}
		
		public void adjustXp(int xpAdjustment) {
			xp += xpAdjustment;
			totalXp += xpAdjustment;
			updateLevel ();
		}
		
		public int getXp() {
			return xp;
		}
		
		public void setNextXp(int newNextXp) {
			nextXp = newNextXp;
			updateLevel ();
		}
		
		public int getNextXp() {
			return nextXp;
		}
		
		public SBLSSkill getSkill(int skillNo) {
			return skills [skillNo];
		}
		
		public void updateSkill(int skillNo, int xpAdjustment) {
			skills [skillNo].adjustXp (xpAdjustment);
		}

		public SBLSSkill getSkill(string skillName) {
			return skills.Find (i => i.getName () == skillName);;
		}
		
		public void updateSkill(string skillName, int xpAdjustment) {
			var sk = skills.Find (i => i.getName () == skillName);
			sk.adjustXp (xpAdjustment);
		}
		
		private void updateLevel() {
			if (xp >= nextXp) {
				if (level != config.levelLimit) {
					level++;


					setXp (xp - nextXp);
					if (config.useLevelMultiplier) {
						float next = getLevel() * config.multiplier;
						setNextXp((int)next);
					} else {
						setNextXp(levels[getLevel()].xpToReachLevel);
					}

					SendMessage("levelUpdated", this, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		
		public void setLevel(int newLevel) {
			level = newLevel;
		}
		
		public int getLevel() {
			return level;
		}

		public SBLSConfiguration getConfig() {
			return config;
		}

		public void characterSkillUpdated(SBLSSkill sk) {
			SendMessage ("skillUpdated", sk, SendMessageOptions.DontRequireReceiver);
		}

		public int getProgress(int width) {
			float progress = (float)getXp () / (float)getNextXp () * width;
			
			return (int)progress;
		}

		public void reset() {
			xp = 0;
			level = 1;
			if (config.useLevelMultiplier) {
				nextXp = level * config.firstLevelXp;
			} else {
				nextXp = levels[level].xpToReachLevel;
			}


			foreach (SBLSSkill skill in skills) {
				skill.setLevel(1);
				skill.setXp (0);
				skill.setNextXp(skill.levels[skill.level].xpToReachLevel);
				skill.setTimeUsed(0.0f);
				skill.setTotalTimeUsed(0.0f);
			}	
		}

		public void startQuest(SBLSQuestConfiguration questConfig) {
			SBLSQuest quest = questConfig.quest;

			//#if UNITY_EDITOR

				quest.started = false;
				quest.complete = false;
				quest.currentStep = 0;
				
				foreach (SBLSQuestStep qs in quest.steps) {
					qs.completed = false;
					qs.active = false;
				}
			//#endif

			// We don't want to add it if the quest is already there
			if (findQuest (quest.getName ()) == null) {
				quest.activateQuest ();
				quest.character = this;
				quest.currentStep = 0;
				quests.Add (quest);
				SendMessage ("questStarted", quest, SendMessageOptions.DontRequireReceiver);


			} else {
				Debug.Log ("Quest \""+ quest.getName() +"\" has already been started");
			}
		}

		public SBLSQuest findQuest(string questName) {
				return quests.Find (i => i.getName () == questName);
		}

		public List<SBLSQuest> getActiveQuests() {
			return quests.FindAll(i => !i.complete && i.started);
		}

		public List<SBLSQuest> getCompletedQuests() {
			return quests.FindAll ( i => i.complete);
		}

		public void questStepCompleted(string questName) {
			SBLSQuest q = findQuest (questName);

			if (q != null) {
				q.updateQuest ();
				SendMessage ("questUpdated", q, SendMessageOptions.DontRequireReceiver);
			}
		}

		public void questIsDone(SBLSQuest q) {
			SendMessage ("questCompleted", q, SendMessageOptions.DontRequireReceiver);
		}
	}

}