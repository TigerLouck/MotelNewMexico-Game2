using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtagSpin : MonoBehaviour
{
    Vector3 rotaryInertia;
    // Update is called once per frame
    void Update()
    {
        rotaryInertia = Quaternion.AngleAxis(0, Random.onUnitSphere) * rotaryInertia;
        //transform.rotation = rotaryInertia *
    }
}
