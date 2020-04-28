using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleSpinner : MonoBehaviour
{
	ParticleSystem pSystem;
	public GameManager gameManager;
	private AudioManager audioManager;
	private Camera PlayerCamera = null;

	[SerializeField]
    private bool doesDamage = true;

    private void Awake() {
		pSystem = transform.parent.GetChild(1).GetComponent<ParticleSystem>();
		Debug.Assert(pSystem != null);
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
	}

	void Update()
	{
		transform.localRotation = Quaternion.Euler(0, 0, 2) * transform.localRotation;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Player"||!doesDamage)
		{
			return;
		}
		ParticleSystem.Particle[] splashParticles = new ParticleSystem.Particle[pSystem.particleCount];
		pSystem.GetParticles(splashParticles);
		for (int i = 0; i < splashParticles.Length; i++)
		{
			// add the vector from player to this, multiply by random, disperse
			splashParticles[i].position += ((transform.position - other.transform.position) * Random.Range(1f, 5) + Random.insideUnitSphere);
			splashParticles[i].velocity = ((transform.position - other.transform.position) * 10 + Random.insideUnitSphere);
		}

		audioManager.PlayObstacle();
		pSystem.SetParticles(splashParticles);
		GetComponent<SphereCollider>().enabled = false;//turn off the collider to avoid double hits
		Time.timeScale = 0;

		// game over code
		//disconnect the player controller and camera
		PlayerCamera = Move.staticAccess.posObj.GetComponentInChildren<Camera>();
		PlayerCamera.transform.SetParent(null, true);
		gameManager.scoreText.enabled = false;
		gameManager.gemsText.enabled = false;
		gameManager.gameOverSplashText.transform.GetChild(1).GetComponent<Text>()
			.text = "Final Score: " + gameManager.score.ToString("#.##");
		gameManager.gameOverSplashText.SetActive(true);
		//GameManager.staticManager.ParticleSystem.SetActive(true);
		Move.staticAccess.enabled = false;
		//standby for reload
		StartCoroutine(DieAndRespawn());
	}

	IEnumerator DieAndRespawn()
	{
		while (true)
		{
			//Look to the thing that just killed you
			PlayerCamera.transform.rotation = Quaternion.Lerp(
				PlayerCamera.transform.rotation,
				Quaternion.LookRotation(transform.position - PlayerCamera.transform.position, Vector3.up),
				.05f
			);
			//timeToRespawn -= Time.deltaTime;
			yield return null;
		}

	}
}
