using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource obstacleSound;
    public AudioSource buttonSound;
    public AudioSource musicSound;
    public AudioSource collectibleSound;
    // Start is called before the first frame update
    void Start()
    {
        PlayMusic();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayMusic()
    {
        musicSound.Play();
    }

    public void PlayObstacle()
    {
        obstacleSound.Play();
    }

    public void PlayCollectible()
    {
        collectibleSound.Play();
    }

    public void PlayButton()
    {
        buttonSound.Play();
    }

    public void StopMusic()
    {
        musicSound.Stop();
    }

}
