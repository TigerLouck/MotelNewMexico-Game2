using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameObject singletonGameMaster;

    public static GameManager staticManager;
    public Text scoreText;
    public Text gemsText;
    public GameObject SplashText;
    public GameObject gameOverSplashText;
    public Canvas gameCanvas;

    public float score;
    public int gems;

    private Move moveScript;


    private void Awake()
    {
        //Enforce singleton pattern on load
        if (singletonGameMaster == null)
        {
            singletonGameMaster = gameObject;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(gameCanvas);
            staticManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        moveScript = Move.staticAccess;
        Time.timeScale = 0;
    }

	public void StartPlay()
	{
		Time.timeScale = 1;
		Move.staticAccess.transform.parent.parent.GetComponentInChildren<ParticleSystem>()?.gameObject.SetActive(false);
        SplashText.SetActive(false);
	}

	public void Reload()
	{
        SplashText.SetActive(true);
        gameOverSplashText.SetActive(false);
        gems = 0;
        score = 0;
        scoreText.enabled = true;
        gemsText.enabled = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}

    // Update is called once per frame
    void Update()
    {
        score += moveScript.speed * Time.deltaTime;
        scoreText.text = "Score: " + score.ToString("#.##");
        gemsText.text = "Gems: " + gems;
    }
}
