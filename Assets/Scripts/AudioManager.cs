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

    public void PlayCollectible(bool shouldJingle)
    {
        if(shouldJingle)
        {
            float pitchChangeRNG = Random.Range(0.0f, 1.0f);
            if (pitchChangeRNG < .5f)
            {
                collectibleSound.pitch -= .01f;
            }
            else
            {
                collectibleSound.pitch += .01f;
            }
        }
        else
        {
            collectibleSound.pitch = 1.0f;
        }
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
