using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameObject singletonGameMaster;

    public static GameManager staticManager;
    public Text distanceText;
    public Text gemsText;
    public GameObject SplashText;
    public GameObject gameOverSplashText;
    public Canvas gameCanvas;

    public int gems;

    public Move moveScript;

    private bool isPlaying;

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
        gems = 0;
    }

	public void StartPlay()
	{
		Time.timeScale = 1;
		Move.staticAccess.transform.parent.parent.GetComponentInChildren<ParticleSystem>()?.gameObject.SetActive(false);
        SplashText.SetActive(false);
        distanceText.gameObject.SetActive(true);
        gemsText.gameObject.SetActive(true);   
        isPlaying = true;
	}

	public void Reload()
	{
        SplashText.SetActive(true);
        gameOverSplashText.SetActive(false);
        gems = 0;
        isPlaying = false;
        //moveScript.distance = 0f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}

    // Update is called once per frame
    void Update()
    {
        if(isPlaying)
        {
            distanceText.text = "Distance: " + moveScript.distance.ToString("#.##") + "m";
            gemsText.text = "Gems: " + gems;
        }
        
    }
}
