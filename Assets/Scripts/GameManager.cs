using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text scoreText;
    public Text livesText;

    private float score;
    public int lives;

    public GameObject player;
    public GameObject obstacle;

    private Move moveScript;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        lives = 3;

        moveScript = player.GetComponent<Move>();
    }

    // Update is called once per frame
    void Update()
    {
        score += moveScript.speed * Time.deltaTime;
        scoreText.text = "Score: " + score.ToString("#.##");
        livesText.text = "Lives: " + lives;
    }
}
