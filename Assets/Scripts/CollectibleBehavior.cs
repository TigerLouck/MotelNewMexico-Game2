using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleBehavior : MonoBehaviour
{
    public GameManager gameManager;
    private AudioManager audioManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(0, 0, 2) * transform.localRotation;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }
        Debug.Log(gameManager.timeLastCollected);
        gameManager.gems++;
        // if it's been less than .3 seconds since you last collected a gem
        // make the sound jingle
        audioManager.PlayCollectible(gameManager.timeLastCollected < .3f);
        gameManager.timeLastCollected = 0.0f;
        Destroy(this.gameObject);
    }
}
