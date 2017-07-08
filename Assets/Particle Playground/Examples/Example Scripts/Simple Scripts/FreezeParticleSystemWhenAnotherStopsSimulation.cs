using UnityEngine;
using System.Collections;

namespace ParticlePlayground {
	public class FreezeParticleSystemWhenAnotherStopsSimulation : MonoBehaviour {

		public PlaygroundParticlesC conditionalParticleSystem;
		public PlaygroundParticlesC freezeParticleSystem;
		public float extraWaitingTime = 0;

		private float _originalTimeScale = 0;

		void OnEnable ()
		{
			if (Mathf.Approximately(_originalTimeScale, 0))
				_originalTimeScale = freezeParticleSystem.particleTimescale;
			else
				freezeParticleSystem.particleTimescale = _originalTimeScale;
			StartCoroutine(FreezeRoutine());
		}

		void OnDisable ()
		{
			StopCoroutine("FreezeRoutine");
		}

		IEnumerator FreezeRoutine () 
		{
			if (conditionalParticleSystem == null || freezeParticleSystem == null)
				yield break;

			// Wait while the conditional particle system isn't ready to begin or while it's still alive
			while (!conditionalParticleSystem.IsReady() || conditionalParticleSystem.IsAlive())
				yield return null;

			// Wait extra time if specified
			if (extraWaitingTime > 0)
				yield return new WaitForSeconds(extraWaitingTime);

			// Freeze the particle system by setting time scale to 0 now when the conditional particle system isn't alive any longer
			freezeParticleSystem.particleTimescale = 0;
		}
	}
}