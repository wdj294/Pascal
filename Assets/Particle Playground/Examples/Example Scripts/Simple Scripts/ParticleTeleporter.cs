using UnityEngine;
using System.Collections;
using ParticlePlayground;

/// <summary>
/// A particle teleporter which uses a Manipulator to track the entering particles.
/// Create a new Manipulator onto the 'particles' particle system which has Type: None, Track Particles: Enabled and Send Enter Events: Enabled.
/// </summary>
public class ParticleTeleporter : MonoBehaviour {

	public PlaygroundParticlesC particles;
	public int manipulatorIndex = 0;
	public Transform target;
	private ManipulatorObjectC _manipulator;
	private Vector3 _targetPosition;

	void OnEnable ()
	{
		if (_manipulator == null)
			_manipulator = PlaygroundC.GetManipulator(manipulatorIndex, particles);

		// Sanity check
		_targetPosition = target.position;
		_manipulator.particleEventEnter -= Teleport;

		// Enable the Manipulator
		_manipulator.enabled = true;

		// Assign to the event delegate of when a particle is entering the Manipulator
		_manipulator.particleEventEnter += Teleport;
	}

	void OnDisable ()
	{
		// Remove from the event delegate of when a particle is entering the Manipulator
		_manipulator.particleEventEnter -= Teleport;

		// Disable the Manipulator
		_manipulator.enabled = false;
	}

	void Update ()
	{
		// Get the thread-safe position of our target.
		// We do this because the teleport can happen on another thread (Get/Set Transform positions is not allowed elsewhere than on the Main-Thread)
		_targetPosition = target.position;
	}

	void Teleport (PlaygroundEventParticle eventParticle)
	{
		// Store the necessary values of this particle
		int particleId = eventParticle.particleId;
		Vector3 particlePosition = eventParticle.position;

		// Get the thread-safe position of the manipulator
		Vector3 manipulatorPosition = _manipulator.transform.position;

		// Set the position of the particle towards the target with an offset
		Vector3 offsetedTargetPosition = _targetPosition + (particlePosition - manipulatorPosition);
		particles.ParticlePosition (particleId, offsetedTargetPosition);
	}
}
