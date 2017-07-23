using UnityEngine;
using System.Collections;
using SmartMath;

public class energyMass : MonoBehaviour {



	public float MassEnergy(float m, float c)
	{

		return m * float.Parse(System.Math.Pow(c, 2).ToString());

	}
}
