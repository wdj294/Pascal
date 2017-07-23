using UnityEngine;
using System.Collections;
using SmartMath;

public class celsiusConvert : MonoBehaviour {



	public float KelvinToCelsius(float kelvin)
	{
		if (kelvin == 0)
		{
			return 0;
		}
		else
		{
			return kelvin - 273.15f;
		}
	}
}
