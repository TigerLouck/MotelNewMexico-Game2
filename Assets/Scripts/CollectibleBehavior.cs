using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleBehavior : MonoBehaviour
{
    public GameManager gameManager;
    private AudioManager audioManager;
    private float timeLastCollected;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        timeLastCollected = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(0, 0, 2) * transform.localRotation;
        timeLastCollected += Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        //gameManager.distance += 3;
        gameManager.gems++;
        // if it's been less than half a second since you last collected a gem
        // make the sound jingle
        audioManager.PlayCollectible(timeLastCollected < .5f);
        timeLastCollected = 0.0f;
        Destroy(this.gameObject);
    }
}
