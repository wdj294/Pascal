using UnityEngine;
using System.Collections;
using SmartMath;

public class fahrenheitConvert : MonoBehaviour {



public float CelsiusToFahrenheit(float celsius)
	{
		return celsius * 9 / 5 + 32;
	}
}
