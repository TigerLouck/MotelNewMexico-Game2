using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpinner : MonoBehaviour
{
	ParticleSystem pSystem;

	private void Start()
	{
		pSystem = transform.parent.GetChild(1).GetComponent<ParticleSystem>();
		Debug.Assert(pSystem != null);
	}

	void Update()
	{
		transform.rotation = Quaternion.Euler(0, 0, 2) * transform.rotation;
	}

	private void OnTriggerEnter(Collider other)
	{
		ParticleSystem.Particle[] splashParticles = new ParticleSystem.Particle[pSystem.particleCount];
		pSystem.GetParticles(splashParticles);
		for (int i = 0; i < splashParticles.Length; i++)
		{
			// add the vector from player to this, multiply by random, disperse
			splashParticles[i].position += ((transform.position - other.transform.position) * Random.Range(1f, 5) + Random.insideUnitSphere);
			splashParticles[i].velocity = ((transform.position - other.transform.position) * 10 + Random.insideUnitSphere);
		}
		Debug.Log("splash");
		pSystem.SetParticles(splashParticles);

	}
}
