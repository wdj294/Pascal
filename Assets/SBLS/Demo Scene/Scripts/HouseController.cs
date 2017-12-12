using UnityEngine;
using System.Collections;
using SBLS;

public class HouseController : MonoBehaviour {
	public SBLSQuestConfiguration questConfig;
	private SBLSCharacter pc;
	private GameObject player;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		pc = player.GetComponent<SBLSCharacter> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col) {
		if (col.tag == "Player") {
			pc = col.gameObject.GetComponent<SBLSCharacter>();

			// We only want to update the quest if it has started, and we're on the first step
			if (pc.findQuest(questConfig.quest.getName()).started && pc.findQuest(questConfig.quest.getName()).currentStep == 0) {
				GameObject.FindGameObjectWithTag("HouseIndicator").SetActive(false);
				pc.questStepCompleted(questConfig.quest.getName ());
			}
		}
	}
}
