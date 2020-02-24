using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class NewGen : MonoBehaviour
{
    private int numNodes;
    private float count = 0f;
    private GameObject spline;
    public GameObject nodesScript;
    public GameObject item;
    public GameObject obstacle;
    private Vector3 splineLocation;
    private Quaternion splineRotation;

    // Start is called before the first frame update
    void Start()
    {
        numNodes = nodesScript.GetComponent<Spline>().nodes.Count;
        spline = GameObject.Find("Smoothed"); //the spline
        GenerateObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Place Objects
    void GenerateObjects()
    {
        //move along the spline
        splineLocation = (spline.GetComponent<SplineMesh.Spline>().GetSample(count)).location; //location tracked 2 parents up
        splineRotation = (spline.GetComponent<SplineMesh.Spline>().GetSample(count)).Rotation; //rotation tracked 1 parent up, camera being in same level but above
        Instantiate(item, splineLocation, splineRotation);
        //posObj.transform.position = splineLocaton;
        //rotObj.transform.rotation = splineRotation;
    }
}
