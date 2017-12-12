using UnityEngine;
using System.Collections;
// Include the SBLS namespace
using SBLS;

public class GameControllerGUI : MonoBehaviour {
	// We need a character
	private SBLSCharacter character;

	public Texture2D progress;
	public Texture2D progressUnder;
	public Texture2D progressTop;
	public Texture2D mainLevelProgress;
	public Texture2D logo;
	public Texture2D crosshair;
	public Texture2D questAdded;
	public Texture2D questUpdate;
	public Texture2D questComplete;

	public AudioClip questStartedClip;
	public AudioClip questStepCompleteClip;
	public AudioClip questFinishedClip;

	public AudioClip skillLevelUpClip;
	public AudioClip levelUpClip;

	public AudioClip dialogStartClip;

	private bool isSkillUpdated = false;
	private SBLSSkill updatedSkill;
	private int skillUpdatedBoxX;
	private bool isQuestStarted = false;
	private string startedQuestName;
	private int questStartedBoxX;
	private bool isQuestUpdated = false;
	private bool isQuestCompleted = false;

	private Camera camera;
	private RaycastHit hit;
	private NPC npc; // Npc we are looking at;
	int mainLevelProgressWidth;
	// Use this for initialization
	void Start () {
		// Let's get the character information for the player
		character = GameObject.FindGameObjectWithTag ("Player").GetComponent<SBLSCharacter> ();

		// For the sake of the demo, let's reset the character
		character.reset ();
		// Get the width of the main level progress bar
		mainLevelProgressWidth = Screen.width / 2;

		skillUpdatedBoxX = Screen.width;
		questStartedBoxX = Screen.width;
		Cursor.visible = false;
		camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		int itemLayer = 1 << 9;
		
		Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
		if (Physics.Raycast (ray, out hit, 5, itemLayer)) {
			checkRay (hit);
		} else {
			npc = null;
		}

		if (npc != null) {
			if (Input.GetMouseButton(0)) {
				if (!npc.inDialog) {
					AudioSource.PlayClipAtPoint(dialogStartClip, transform.position);
					npc.startDialog();
				}
			}
		}
	
	}

	void checkRay(RaycastHit h) {
		if (h.collider.tag == "NPC") {
			npc = h.collider.GetComponent<NPC>();
		
		}
	}

	void OnGUI() {

		// Running Skill
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.skin.label.fontSize = 12;


		GUI.DrawTexture (new Rect (35, 60, 200, 25), progressUnder);
		GUI.DrawTexture (new Rect (35, 60, character.getSkill ("Running").getProgress (200), 25), progress);
		GUI.DrawTexture (new Rect (25, 50, 220, 45), progressTop);

		GUI.DrawTexture (new Rect (35, 110, 200, 25), progressUnder);
		GUI.DrawTexture (new Rect (35, 110, character.getSkill ("Jumping").getProgress (200), 25), progress);
		GUI.DrawTexture (new Rect (25, 100, 220, 45), progressTop);

		
		// This is to get some shadow 
		GUI.color = Color.black;
		GUI.Label (new Rect (27, 52, 220, 45), "Running - Level "+ character.getSkill ("Running").getLevel() + " ("+ character.getSkill ("Running").getXp () + "/" + character.getSkill ("Running").getNextXp () +")");
		GUI.color = Color.white;
		GUI.Label (new Rect (25, 50, 220, 45), "Running - Level "+ character.getSkill ("Running").getLevel() + " ("+ character.getSkill ("Running").getXp () + "/" + character.getSkill ("Running").getNextXp () +")");


		// This is to get some shadow 
		GUI.color = Color.black;
		GUI.Label (new Rect (27, 102, 220, 45), "Jumping - Level "+ character.getSkill ("Jumping").getLevel() + " ("+ character.getSkill ("Jumping").getXp () + "/" + character.getSkill ("Jumping").getNextXp () +")");
		GUI.color = Color.white;
		GUI.Label (new Rect (25, 100, 220, 45), "Jumping - Level "+ character.getSkill ("Jumping").getLevel() + " ("+ character.getSkill ("Jumping").getXp () + "/" + character.getSkill ("Jumping").getNextXp () +")");


		GUI.DrawTexture ( new Rect ((Screen.width / 2 - 20), 20, mainLevelProgressWidth, 25), progressUnder); 
		GUI.DrawTexture (new Rect ((Screen.width / 2 - 20), 20, character.getProgress (mainLevelProgressWidth), 25), mainLevelProgress);

		GUI.color = Color.black;
		GUI.Label (new Rect ((Screen.width / 2 - 18), 22, mainLevelProgressWidth, 25), "Level "+ character.getLevel () + " ("+ character.getXp () + "/" + character.getNextXp () +")");
		GUI.color = Color.white;
		GUI.Label (new Rect ((Screen.width / 2 - 20), 20, mainLevelProgressWidth, 25), "Level "+ character.getLevel () + " ("+ character.getXp () + "/" + character.getNextXp () +")");


		if (isSkillUpdated) {
			if (skillUpdatedBoxX > Screen.width - 200) {
				skillUpdatedBoxX -=4;
			}
		} else {
			if (skillUpdatedBoxX < Screen.width) {
				skillUpdatedBoxX +=4;
			}
		}

		if (updatedSkill != null) {
			GUI.DrawTexture (new Rect (skillUpdatedBoxX, Screen.height - 300, 200, 25), progressUnder); 
			GUI.Label (new Rect (skillUpdatedBoxX, Screen.height - 300, 200, 25), updatedSkill.getName () + " Level up!");
		}

		
		if (isQuestStarted) {
			GUI.DrawTexture(new Rect(Screen.width / 2 - 150, 150, 300, 150), questAdded);
			if (questStartedBoxX > Screen.width - 200) {
				questStartedBoxX -=4;
			}
		} else {
			if (questStartedBoxX < Screen.width) {
				questStartedBoxX +=4;
			}
		}


		if (isQuestUpdated) {
			GUI.DrawTexture(new Rect(Screen.width / 2 - 150, 150, 300, 150), questUpdate);
		}

		if (isQuestCompleted) {
			GUI.DrawTexture(new Rect(Screen.width / 2 - 150, 150, 300, 150), questComplete);
		}

		if (!string.IsNullOrEmpty(startedQuestName)) {

			GUI.DrawTexture (new Rect (questStartedBoxX, 300, 200, 25), progressUnder); 
			GUI.Label (new Rect (questStartedBoxX, 300, 200, 25), "<color=red>"+ startedQuestName + "</color> started");
		}

		int qy = 50;
		GUI.Label (new Rect (0, 0, 300, 30), ""+character.getActiveQuests ().Count);
		foreach (SBLSQuest q in character.getActiveQuests()) {
			GUI.Label (new Rect(Screen.width / 2 - 20, qy, 300, 25), q.getName());
			qy += 26;
			GUI.Label (new Rect(Screen.width / 2 - 10, qy, 300, 25), "- "+ q.getCurrentStep().stepName);
			qy += 26;
		}
		
		GUI.DrawTexture (new Rect (Screen.width - 256, Screen.height - 128, 256, 128), logo);

		// Draw our crosshair
		if (npc == null || !npc.inDialog) {
			GUI.DrawTexture (new Rect (Screen.width / 2 - 16, Screen.height / 2 - 16, 32, 32), crosshair);
		}

		if (npc != null && !npc.inDialog) {
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.Label ( new Rect(Screen.width / 2 - 100, Screen.height / 2 + 25, 200, 30), "Talk to "+ npc.characterName);
		}
	}

	// If a skill has been updated this will be called
	void skillUpdated(SBLSSkill sk) {
		AudioSource.PlayClipAtPoint (skillLevelUpClip, transform.position);
		Debug.Log ("Skill updated");
		isSkillUpdated = true;
		updatedSkill = sk;
		CancelInvoke ("hideSkillUpdate");
		Invoke ("hideSkillUpdate", 3.0f);
	}

	void levelUpdated(SBLSCharacter ch) {
		AudioSource.PlayClipAtPoint (levelUpClip, transform.position);
	}

	private void hideSkillUpdate() {
		isSkillUpdated = false;

	}

	// This is called when a quest is started
	void questStarted(SBLSQuest quest) {

		AudioSource.PlayClipAtPoint (questStartedClip, transform.position);
		Debug.Log ("Quest started: " + quest.getName ());
		isQuestStarted = true;
		startedQuestName = quest.getName ();
		CancelInvoke ("hideQuestStarted");
		Invoke ("hideQuestStarted", 3.0f);
	}

	// This is called when a quest is updated
	void questUpdated(SBLSQuest quest) {
		isQuestUpdated = true;
		AudioSource.PlayClipAtPoint (questStepCompleteClip, transform.position);
		CancelInvoke ("hideQuestUpdated");
		Invoke ("hideQuestUpdated", 3.0f);
	}

	// This is called when a quest is completed
	void questCompleted(SBLSQuest quest) {
		isQuestCompleted = true;
		AudioSource.PlayClipAtPoint (questFinishedClip, transform.position);
		CancelInvoke ("hideQuestCompleted");
		Invoke ("hideQuestCompleted", 3.0f);
	}

	private void hideQuestStarted() {
		isQuestStarted = false;
	}

	private void hideQuestUpdated() {
		isQuestUpdated = false;
	}

	private void hideQuestCompleted() {
		isQuestCompleted = false;
	}

}
