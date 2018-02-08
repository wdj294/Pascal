using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NewBehaviourScript : MonoBehaviour {

	public float minTemperature = 1;
	public float temperature = 32;
	public float maxTemperature = 5000;
	public float temperatureFactor = 500;

	public bool overheating;


	// Use this for initialization
	void Update () {
		{
			float tempChange = Time.deltaTime * temperatureFactor;

			if (Input.GetButton ("Fire1") && !overheating) {

				temperature = Math.Min (maxTemperature, temperature + tempChange);

				//if (temperature == roomTemperature) {

				//overheating = false;
				// overheating, no sprinting allowed until you cool down
				// you don't have to have this in your app, just an example
				//}

				if (Input.GetButton ("Fire2")) {

					temperature = Math.Max (minTemperature, temperature - tempChange);

					//if (temperature == minTemperature) {
					//overheating = false;
				}
					
			}
		}
	}
}