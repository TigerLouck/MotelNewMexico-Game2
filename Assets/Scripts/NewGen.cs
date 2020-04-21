using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class NewGen : MonoBehaviour
{
    private int numNodes;
    private int copiesPerShape;
    private float count = 0f;
    public Spline spline;
    public GameObject nodesScript;
    public GameObject item;
    public GameObject obstacle;
    private Vector3 splineLocation, splineTan, splineUp;
    private Quaternion splineRotation;

    public GameObject[] extruders;
    
    // Start is called before the first frame update
    void Start()
    {
        
        copiesPerShape = 1;
        numNodes = nodesScript.GetComponent<Spline>().nodes.Count;
        //spline = GameObject.Find("Extruder").GetComponent<Spline>(); //the spline
        extruders = new GameObject[100];
        extruders[0] = GameObject.Find("Extruder");
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
            CurveSample sample = spline.GetSample(count);
            Vector3 splineLocalLocation = sample.location; //location tracked 2 parents up
            splineLocation = extruders[0].transform.TransformPoint(splineLocalLocation);
            splineRotation = sample.Rotation; //rotation tracked 1 parent up, camera being in same level but above
            splineTan = sample.tangent;
            splineUp = sample.up;
            Ring(splineLocation);
            //splineDir = spline.GetComponent<Spline>().GetSample(count).Direction;
            //Instantiate(item, splineLocation, splineRotation);
            count += .04f;
        }
    }

    /// <summary>
    /// Place objects in a ring
    /// </summary>
    void Ring(Vector3 centerPosNode)
    {
        float angle = 90f / copiesPerShape;
        float radius = 5.0f;
        for (int i = 0; i < copiesPerShape; i++)
        {
            Vector3 direction = Quaternion.AngleAxis(i * angle, splineTan) * splineUp;
            //Vector3 direction = Vector3.ProjectOnPlane(Random.insideUnitCircle.normalized, splineTan);
            //Vector3 direction = Vector3.ProjectOnPlane(new Vector2(Mathf.Cos(angle * i), Mathf.Sin(angle * i)).normalized, splineTan);

            Vector3 position = centerPosNode + (direction * radius);
            position.y += 8;
            Instantiate(item, position, splineRotation);//Quaternion.Euler(splineTan));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(splineLocation, splineTan * 2f);
    }
}

