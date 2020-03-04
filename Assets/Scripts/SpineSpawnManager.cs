using System.Collections;
using System.Collections.Generic;
using SplineMesh;
using UnityEngine;

public class SpineSpawnManager : MonoBehaviour
{
    // New Gen Fields
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


    [SerializeField]
    private List<GameObject> splinePrefabs;

    [SerializeField]
    private GameObject splineParent;

    // Start is called before the first frame update

    [SerializeField]
    private float spawnTimer;

    // a list of all the splines that have currently spawned in the scene
    public List<GameObject> spawnedSplines;

    private float currentTime;

    void Start()
    {
        currentTime = 0f;
        spawnedSplines = new List<GameObject>();
        SpawnRandomSpline();

        copiesPerShape = 5;
        //nodesScript = spawnedSplines[0].transform.GetChild(0).gameObject.GetComponent<Spline>().nodes.Count;
        numNodes = spawnedSplines[0].transform.GetChild(0).gameObject.GetComponent<Spline>().nodes.Count;
        spline = spawnedSplines[0].transform.GetChild(0).gameObject.GetComponent<Spline>();
        //numNodes = nodesScript.GetComponent<Spline>().nodes.Count;
        //spline = GameObject.Find("Extruder").GetComponent<Spline>(); //the spline
        extruders = new GameObject[100];
        extruders[0] = spawnedSplines[0].transform.GetChild(0).gameObject;
        //spline = spawnedSplines[0].transform.GetChild(0).gameObject;
        //extruders[0] = GameObject.Find("Extruder");
        GenerateObjects();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        UpdateTimer();
    }

    private void SpawnRandomSpline()
    {
        if (spawnedSplines.Count <= 0)
        {
            // if not other splines to base position off of, start at origin
            GameObject temp = Instantiate(splinePrefabs[GetRandomSpine()]);
            //temp.transform.parent = splineParent.transform;
            spawnedSplines.Add(temp);
            //Debug.Log("Here");
        }
        else
        {
            Vector3 lastPosition = GetEndPosition(spawnedSplines[spawnedSplines.Count - 1]);
            Vector3 lastDirection = GetEndDirection(spawnedSplines[spawnedSplines.Count - 1]);
            GameObject temp = Instantiate(splinePrefabs[GetRandomSpine()]);



            Transform currentTransform = spawnedSplines[spawnedSplines.Count - 1].transform.GetChild(0);
            Transform tempG = currentTransform.GetChild(0);

            // parenting of splines makes this not work properly
            Vector3 newPos = spawnedSplines[spawnedSplines.Count - 1].transform.GetChild(0).transform.TransformPoint(lastPosition);

            //newPos*=5f;

            Vector3 diff = currentTransform.TransformPoint(lastDirection);
            //diff*=5.019f;
            Vector3 newDir = (diff - newPos).normalized;

            //Debug.Log(newPos);

            temp.transform.position = newPos;
            temp.transform.Rotate(newDir);
            spawnedSplines.Add(temp);


        }

    }

    private void UpdateTimer()
    {
        currentTime += Time.fixedDeltaTime;
        if (currentTime >= spawnTimer)
        {
            SpawnRandomSpline();
            currentTime = 0f;
        }
    }

    private int GetRandomSpine()
    {
        int rand = Random.Range(0, splinePrefabs.Count);
        return rand;
    }

    public Vector3 GetEndPosition(GameObject spline)
    {
        List<SplineNode> nodes = spline.GetComponentInChildren<Spline>().nodes;
        SplineNode lastNode = nodes[nodes.Count - 1];
        return lastNode.Position;
    }

    private Vector3 GetEndDirection(GameObject spline)
    {
        List<SplineNode> nodes = spline.GetComponentInChildren<Spline>().nodes;
        SplineNode lastNode = nodes[nodes.Count - 1];
        return lastNode.Direction;
    }

    // Place Objects along the spline
    private void GenerateObjects()
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
            count += .5f;
        }
    }

    /// <summary>
    /// Place objects in a ring
    /// </summary>
    void Ring(Vector3 centerPosNode)
    {
        float angle = 360f / copiesPerShape;
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
}