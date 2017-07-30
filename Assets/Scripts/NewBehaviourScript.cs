using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NewBehaviourScript : MonoBehaviour {

	public float minTemperature = 32;
	public float temperature = 32;
	public float maxTemperature = 105;
	// this defines both heatup and cooldown speeds; this is 30 units per second
	// you could separate these speeds if you prefer
	public float temperatureFactor = 30;
	public bool overheating;

	// Use this for initialization
	void Update () {
		{
			float tempChange = Time.deltaTime * temperatureFactor;
			if (Input.GetKey (KeyCode.LeftShift) && !overheating) {
				temperature = Math.Min (maxTemperature, temperature + tempChange);
				if (temperature == maxTemperature) {
					overheating = true;
					// overheating, no sprinting allowed until you cool down
					// you don't have to have this in your app, just an example
				}
			} else if (!Input.GetKey (KeyCode.W)) {
				temperature = Math.Max (minTemperature, temperature - tempChange);
				if (temperature == minTemperature) {
					overheating = false;
				}
					
			}
		}
	}
}

