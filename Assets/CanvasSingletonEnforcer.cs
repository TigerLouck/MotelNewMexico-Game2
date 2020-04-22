using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSingletonEnforcer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(GameManager.singletonGameMaster.GetComponent<GameManager>().gameCanvas != GetComponent<Canvas>())
        {
            Destroy(gameObject);
        }
    }
}
