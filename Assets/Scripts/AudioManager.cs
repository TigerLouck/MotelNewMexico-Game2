using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioSource obstacleSound;
    public AudioSource buttonSound;
    public AudioSource musicSound;
    public AudioSource collectibleSound;
    private AudioMixerGroup pitchBendGroup;
    // Start is called before the first frame update
    void Start()
    {
        PlayMusic();
        pitchBendGroup = Resources.Load<AudioMixerGroup>("Pitch Bender");
        collectibleSound.outputAudioMixerGroup = pitchBendGroup;
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
        // Algorithm #1: using RNG to decide to change the pitch up or down
        
        if(shouldJingle)
        {   
            float changePitchRNG = UnityEngine.Random.Range(0.9f, 1.1f);
            collectibleSound.pitch = changePitchRNG;
            /*
            if (changePitchRNG < .5f)
            {
                collectibleSound.pitch -= .03f;
            }
            else
            {
                collectibleSound.pitch += .03f;
            }*/
            //Mathf.Clamp(collectibleSound.pitch, 0.5f, 2f);
        }
        else
        {
            collectibleSound.pitch = 1.0f;
        }

        // Algorithm #2: using the audio mixer
        /*
        if (shouldJingle)
        {
            collectibleSound.pitch = 1.5f;
            pitchBendGroup.audioMixer.SetFloat("pitchBend", 1f / collectibleSound.pitch);
        }
        else
        {
            collectibleSound.pitch = 1.0f;
        }
        */
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
