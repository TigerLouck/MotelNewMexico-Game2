﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager staticManager;
    public Text scoreText;
    public Text livesText;
    public GameObject SplashText;
    public GameObject gameOverSplashText;

    public float score;
    public int lives;

    public GameObject obstacle;

    private Move moveScript;

    // Start is called before the first frame update
    void Start()
    {
        staticManager = this;
        moveScript = Move.staticAccess;
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale < .99f && (Input.touchCount > 0 || Input.anyKeyDown))
        {
            Time.timeScale = 1;
            Move.staticAccess.transform.parent.parent.GetComponentInChildren<ParticleSystem>().gameObject.SetActive(false);
            SplashText.SetActive(false);
        }
        score += moveScript.speed * Time.deltaTime;
        scoreText.text = "Score: " + score.ToString("#.##");
        livesText.text = "Lives: " + lives;
    }
}
