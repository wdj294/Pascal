using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SBLS;


// This is a simple NPC class designed specifically for the demo.  Don't have time to implement a full dialog system, nor is it in the scope of the project
public class NPC : MonoBehaviour {
	public string characterName;
	public SBLSQuestConfiguration questConfig;
	public Texture2D dialogBackgound;
	public AudioClip mouseClickClip;


	public bool inDialog = false;
	public List<string> dialogOptions = new List<string>();
	private GameObject player;
	private SBLSCharacter pc;
	private int dialogStep = 0;
	private SBLSQuest quest;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		pc = player.GetComponent<SBLSCharacter> ();
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		if (inDialog) {
			quest = pc.findQuest(questConfig.quest.getName());
			GUI.skin.label.fontSize = 40;
	
			GUI.DrawTexture( new Rect(Screen.width / 2 - (Screen.width / 4), Screen.height / 2 - (Screen.height / 4), (Screen.width / 2), (Screen.height / 2)), dialogBackgound);

			GUI.Label (new Rect(Screen.width / 2 - (Screen.width / 4) + 2, Screen.height / 2 - (Screen.height / 4) + 2, 200, 80), characterName);
			GUI.skin.label.fontSize = 14;
			if (quest == null || (quest.started  && !quest.complete)) {
				if (dialogStep == 0) {
					GUI.Label (new Rect(Screen.width / 2 - (Screen.width / 4), Screen.height / 2 - 60, (Screen.width / 2), 80), dialogOptions[dialogStep]);
					if (GUI.Button (new Rect(Screen.width / 2 - 110, Screen.height / 2 + 90, 100, 50), "I'll help")) {
						GameObject.FindGameObjectWithTag("HouseIndicator").GetComponent<Renderer>().enabled = true;
						GameObject.FindGameObjectWithTag("HouseIndicatorLight").GetComponent<Light>().enabled = true;
						pc.startQuest(questConfig);
						dialogStep = 1;
						endDialog();
					}

					if (GUI.Button (new Rect(Screen.width / 2 + 10, Screen.height / 2 + 90, 100, 50), "No")) {
						endDialog();
					}
				}

				if (dialogStep == 1) {
					GUI.Label (new Rect(Screen.width / 2 - (Screen.width / 4), Screen.height / 2 - 60, (Screen.width / 2), 80), dialogOptions[dialogStep]);
					if (GUI.Button (new Rect(Screen.width / 2 - 50, Screen.height / 2 + 90, 100, 50), "No")) {
						endDialog();
					}
				}

				if (quest != null) {
					if (quest.started && quest.currentStep == 1) {
						dialogStep = 2;
					}
				}

				if (dialogStep == 2) {

					GUI.Label (new Rect(Screen.width / 2 - (Screen.width / 4), Screen.height / 2 - 60, (Screen.width / 2), 80), dialogOptions[dialogStep]);
					if (GUI.Button (new Rect(Screen.width / 2 - 50, Screen.height / 2 + 90, 100, 50), "You're welcome!")) {
						pc.findQuest(questConfig.quest.getName()).updateQuest();
						dialogStep = 3;
						endDialog();
					}
				}
			} else {
				GUI.Label (new Rect(Screen.width / 2 - (Screen.width / 4), Screen.height / 2 - 60, (Screen.width / 2), 80), dialogOptions[dialogStep]);
				if (GUI.Button (new Rect(Screen.width / 2 - 60, Screen.height / 2 + 90, 120, 50), "I'm leaving now")) {
					endDialog();
				}
			}


		}
	}

	public void startDialog() {
		inDialog = true;
		player.GetComponent<FPSController> ().enabled = false;
		player.GetComponent<MouseLook> ().enabled = false;
		Cursor.visible = true;
	}

	public void endDialog() {
		AudioSource.PlayClipAtPoint (mouseClickClip, transform.position);
		inDialog = false;
		player.GetComponent<FPSController> ().enabled = true;
		player.GetComponent<MouseLook> ().enabled = true;
		Cursor.visible = false;
	}
}
