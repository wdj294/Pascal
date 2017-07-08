using UnityEngine;
using System.Collections;

namespace ParticlePlayground {
	public class NestedParticleSystemSequence : MonoBehaviour {

		public SystemSequence[] sequences;

		void Start () 
		{
			for (int i = 0; i<sequences.Length; i++)
				StartCoroutine(sequences[i].StartDelayedEmission());
		}
	}

	[System.Serializable]
	public class SystemSequence {
		public PlaygroundParticlesC particles;
		public float startTime;

		public IEnumerator StartDelayedEmission ()
		{
			particles.particleSystemGameObject.SetActive(false);
			yield return new WaitForSeconds(startTime);
			particles.Emit (true);
		}
	}
}