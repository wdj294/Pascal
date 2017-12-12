using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SBLS;

namespace SBLS {

	public class SBLSUtils : MonoBehaviour
	{

		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
		
		}

		// Find all other configs
		public static SBLSConfiguration[] FindAll() {
			List<SBLSConfiguration> results = new List<SBLSConfiguration>();
			var allConfigs = Resources.LoadAll("SBLS/Config", typeof(SBLSConfiguration));
			
			foreach (var confs in allConfigs) {
				results.Add(confs as SBLSConfiguration);
			}
			
			return results.ToArray();
		}

		public static SBLSConfiguration FindDefault() {
			var allConfigs = Resources.LoadAll("SBLS/Config", typeof(SBLSConfiguration));

			foreach (SBLSConfiguration confs in allConfigs) {
				SBLSConfiguration config = confs as SBLSConfiguration;
				if (config.isDefault) {
					return confs as SBLSConfiguration;
				}
			}

			return null;
		}
	}
}

