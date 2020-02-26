using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class NewGen : MonoBehaviour
{
    private int numNodes;
    private int copiesPerShape;
    private float count = 0f;
    private GameObject spline;
    public GameObject nodesScript;
    public GameObject item;
    public GameObject obstacle;
    private Vector3 splineLocation, splineTan, splineDir;
    private Quaternion splineRotation;

    // Start is called before the first frame update
    void Start()
    {
        copiesPerShape = 5;
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
        // Keep generating objects until the end is reached
        while (count <= numNodes)
        {
            //move along the spline
            splineLocation = (spline.GetComponent<SplineMesh.Spline>().GetSample(count)).location; //location tracked 2 parents up
            splineRotation = (spline.GetComponent<SplineMesh.Spline>().GetSample(count)).Rotation; //rotation tracked 1 parent up, camera being in same level but above
            splineTan = spline.GetComponent<Spline>().GetSample(count).tangent;
            Ring(splineLocation);
            //splineDir = spline.GetComponent<Spline>().GetSample(count).Direction;
            //Instantiate(item, splineLocation, splineRotation);
            count += 1.0f;
        }
    }

    /// <summary>
    /// Place objects in a ring
    /// </summary>
    void Ring(Vector3 centerPosNode)
    {
        float angle = Mathf.Deg2Rad * 360f / copiesPerShape;
        float radius = 1.0f;
        for (int i = 0; i < copiesPerShape; i++)
        {
            //Quaternion rotation = Quaternion.AngleAxis(i * angle,Vector3.up);
            //Vector3 direction = Vector3.ProjectOnPlane(Random.insideUnitCircle.normalized, splineTan);
            Vector3 direction = Vector3.ProjectOnPlane(new Vector2(Mathf.Cos(angle * i), Mathf.Sin(angle * i)).normalized, splineTan);

            Vector3 position = centerPosNode + (direction * radius);
            Instantiate(item, position, splineRotation);//Quaternion.Euler(splineTan));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(splineLocation, splineTan * 2f);
    }
}

