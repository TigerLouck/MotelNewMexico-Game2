using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpinner : MonoBehaviour
{
	ParticleSystem pSystem;
	public GameManager gameManager;
	private AudioManager audioManager;
	private Camera PlayerCamera = null;
	private void Start()
	{
		pSystem = transform.parent.GetChild(1).GetComponent<ParticleSystem>();
		Debug.Assert(pSystem != null);
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
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
		gameManager.lives--;
		audioManager.PlayObstacle();
		Debug.Log("splash");
		pSystem.SetParticles(splashParticles);
		GameManager.staticManager.lives--;
		GetComponent<SphereCollider>().enabled = false;//turn off the collider to avoid double hits
		Time.timeScale = 0;
		if (GameManager.staticManager.lives < 0)
		{
			//disconnect the player controller and camera
			PlayerCamera = Move.staticAccess.posObj.GetComponentInChildren<Camera>();
			PlayerCamera.transform.SetParent(null, true);
			Move.staticAccess.enabled = false;
			//standby for reload
			StartCoroutine(DieAndRespawn());
		}
	}

	IEnumerator DieAndRespawn()
	{
		float timeToRespawn = 5;
		while (timeToRespawn > 0)
		{
			//Look to the thing that just killed you
			PlayerCamera.transform.rotation = Quaternion.Lerp(
				PlayerCamera.transform.rotation,
				Quaternion.LookRotation(transform.position - PlayerCamera.transform.position, Vector3.up),
				.05f
			);
			timeToRespawn -= Time.deltaTime;
			yield return null;
		}
		//Reload
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);

	}
}
