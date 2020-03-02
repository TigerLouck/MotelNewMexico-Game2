using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    void Update()
    {
        GetComponent<SplineMesh.Spline>().nodes[0].Roll += .1f;
    }
}
