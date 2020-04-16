using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettingChanger : MonoBehaviour
{
	public AudioSource targetSource;
	public float maxVolume;

    // Update is called once per frame
    public void SetVolume(float newVolume)
    {
		targetSource.volume = newVolume * maxVolume;
    }
}
