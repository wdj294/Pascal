using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SBLS {
	[System.Serializable]
	public class SBLSSkill {
		public static SBLSSkill instance;
		public string skillName;
		public int level;
		public int xp;
		public int totalXp;
		public int nextXp;
		public int maxLevel;
		public int xpLevelAdjustment;
		public SBLSCharacter character;
		public bool isTimeBased = false;
		public bool inUse = false;
		public float timeUsed = 0.0f;
		public float totalTimeUsed = 0.0f;
		public int timeXp = 0;
		public int timeUpdateEvery = 0;
		public int timeUpdateType = 0; // seconds, minutes, hours
		public List<SBLSLevel> levels = new List<SBLSLevel>();

		// For editor
		public bool displayLevels = false;


		public SBLSSkill(string newName, int newLevel, int newXp, int newNextXp, SBLSCharacter newCharacter) {
			skillName = newName;
			level = newLevel;
			xp = newXp;
			nextXp = newNextXp;
			character = newCharacter;
			instance = this;
		}

		public SBLSSkill(string newName, int newLevel, int newXp, int newNextXp) {
			skillName = newName;
			level = newLevel;
			xp = newXp;
			nextXp = newNextXp;
			instance = this;
		}

		public string getName() {
			return skillName;
		}

		public void setName(string newName) {
			skillName = newName;
		}

		public int getXp() {
			return xp;
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

		public int getNextXp() {
			return nextXp;
		}

		public void setNextXp(int newXp) {
			nextXp = newXp;
		}

		public void setLevel(int newLevel) {
			level = newLevel;
		}

		public int getLevel() {
			return level;
		}

		private void updateLevel() {
			SBLSConfiguration config;

			config = character.getConfig ();

			if (xp >= nextXp) {
				if (level != config.levelLimit) {
					// Update the character
					character.adjustXp(level);

					// Update the skill level
					level++;

					// Adjust for any extra XP that we need
					setXp (xp - nextXp);

					// Set Next Level XP
					if (config.useLevelMultiplier) {
						float next = getLevel() * config.multiplier;
						setNextXp((int)next);
					} else {
						setNextXp (levels[getLevel ()].xpToReachLevel);
					}

					// Send message to other scripts that the skill has updated
					character.characterSkillUpdated(this);
				}
			}
		}

		public void updateTime(float timeUpdate) {
			timeUsed += timeUpdate;
			totalTimeUsed += timeUpdate;

			checkTime ();
		}

		public void setTimeUsed(float newTime) {
			timeUsed = newTime;
		}

		public float getTimeUsed() {
			return timeUsed;
		}

		public void setTotalTimeUsed(float totalTime) {
			totalTimeUsed = totalTime;
		}

		public float getTotalTimeUsed() {
			return totalTimeUsed;
		}

		public void setCharacter(SBLSCharacter newCharacter) {
			character = newCharacter;
		}

		public SBLSCharacter getCharacter() {
			return character;
		}

		private void checkTime() {
			float tmpTime = 0;
			switch (timeUpdateType) {
				case 1 :
					tmpTime = timeUsed * 60;
				break;
				case 2 :
					tmpTime = timeUsed * 60 * 60;
				break;
				default:
					tmpTime = timeUsed;
				break;
			}
			if ((int)tmpTime >= timeUpdateEvery) {
				adjustXp(timeXp);
				timeUsed = 0.0f;
			}
		}

		public int getProgress(int width) {
			float progress = (float)getXp () / (float)getNextXp () * width;

			return (int)progress;
		}


	}
}
