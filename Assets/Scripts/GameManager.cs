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
    private bool isPlaying;

    // not the cleanest way to keep track of the last time you picked up a gem
    // but it works; this variable is used in CollectibleBehavior.cs
    public float timeLastCollected;

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
        Time.timeScale = 0;
        gems = 0;
        timeLastCollected = 0.0f;
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
        timeLastCollected = 0.0f;
        isPlaying = false;
        //moveScript.distance = 0f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}

    // Update is called once per frame
    void Update()
    {
        if(isPlaying)
        {
            distanceText.text = "Distance: " + Move.staticAccess.distance.ToString("#.##") + "m";
            gemsText.text = "Gems: " + gems;
            timeLastCollected += Time.deltaTime;
        }
        
    }
}
