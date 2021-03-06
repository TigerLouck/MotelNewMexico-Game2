﻿using System.Collections;
using System.Collections.Generic;
using SplineMesh;
using UnityEngine;

public class SpineSpawnManager : MonoBehaviour
{
    // New Gen Fields
    private float firstRNG = 2.0f;
    private int numNodes;
    private int copiesPerShape;
    private float count = 0f;
    
    [HideInInspector]
    public Spline spline;
    //public GameObject nodesScript;
    public GameObject item;
    public GameObject obstacle;
    private Vector3 splineLocation, splineTan, splineUp;
    private Quaternion splineRotation;
    //public GameObject[] extruders;


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


    #region MoveScript Items
    private Move moveScript;
    // the index of the starting spline in the list. Keeps track of which object should be deleted
    private int currentTailIndex;
    // compare current spline index to the one from MOVE script
    private int currentSplineCompare;
    #endregion

    void Start()
    {
        currentTime = 0f;
        spawnedSplines = new List<GameObject>();
        SpawnRandomSpline();

        copiesPerShape = 4;
        numNodes = spawnedSplines[spawnedSplines.Count - 1].transform.GetChild(0).gameObject.GetComponent<Spline>().nodes.Count;
        spline = spawnedSplines[spawnedSplines.Count - 1].transform.GetChild(0).gameObject.GetComponent<Spline>();
        currentTailIndex = 0;
        currentSplineCompare = 0;
        moveScript = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Move>();
        //GenerateObjects();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        UpdateTimer();
        DeleteSplineTail();

    }

    private void DeleteSplineTail()
    {
        //Debug.Log("currentSplineCompare: " + currentSplineCompare);
        if (currentSplineCompare != moveScript.current)
        {
            currentSplineCompare++;
            StartCoroutine("DeleteSpline");
        }
    }

    private void SpawnRandomSpline()
    {
        if (spawnedSplines.Count <= 0)
        {
            // if not other splines to base position off of, start at origin
            GameObject temp = Instantiate(splinePrefabs[GetRandomSpine()]);
            temp.transform.parent = GameObject.FindGameObjectWithTag("SplineParent").transform;
            //temp.transform.parent = splineParent.transform;
            spawnedSplines.Add(temp);
        }
        else
        {
            Vector3 lastPosition = GetEndPosition(spawnedSplines[spawnedSplines.Count - 1]);
            Vector3 lastDirection = GetEndDirection(spawnedSplines[spawnedSplines.Count - 1]);
            GameObject temp = Instantiate(splinePrefabs[GetRandomSpine()]);
            temp.transform.parent = GameObject.FindGameObjectWithTag("SplineParent").transform;



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
        Debug.Log("there are now " + spawnedSplines.Count + " splines spawned");
        spline = spawnedSplines[spawnedSplines.Count - 1].transform.GetChild(0).gameObject.GetComponent<Spline>();
        firstRNG = GenerateObjects(2.0f); // Made it 2 so that it would be true for the conditions so that they can run and generate obstacles
        GenerateObjects(firstRNG);
    }

    IEnumerator DeleteSpline()
    {
        yield return new WaitForSeconds(10f);
        //GameObject.Destroy(spawnedSplines[currentTailIndex]);
        //spawnedSplines[currentTailIndex] = null;
        GameObject.Destroy(spawnedSplines[0]);
        spawnedSplines.RemoveAt(0);
        currentTailIndex++;
        Debug.Log("Delete Spline");
    }

    private void UpdateTimer()
    {
        //Debug.Log("currentTime: " + currentTime + " and spawnTimer is " + spawnTimer);
        currentTime += Time.fixedDeltaTime;
        if (currentTime >= spawnTimer)
        {
            SpawnRandomSpline();
            currentTime = 0f;
            DeleteSplineTail();
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
    private float GenerateObjects(float firstN)
    {
        // Random number
        float rngPlacement = Random.Range(0.0f, 1.0f);
        float rngLines = Random.Range(0.0f, 1.0f);
        bool left = true;
        count = 0f;

        // Keep generating objects until the end is reached
        while (count <= spline.nodes.Count - 1)
        {
            if (rngPlacement < 0.33f && !(firstN < 0.33f)) // Generate Rings
            {
                //move along the spline
                CurveSample sample = spline.GetSample(count);
                Vector3 splineLocalLocation = sample.location; //location tracked 2 parents up
                splineLocation = spawnedSplines[spawnedSplines.Count - 1].transform.TransformPoint(splineLocalLocation);
                splineRotation = sample.Rotation; //rotation tracked 1 parent up, camera being in same level but above
                splineTan = sample.tangent;
                splineUp = sample.up;
                if (count >= 1.0f) // So the player doesn't die at the start
                {
                    Ring(splineLocation);
                    ItemRing(splineLocation);
                }
                //splineDir = spline.GetComponent<Spline>().GetSample(count).Direction;
                //Instantiate(item, splineLocation, splineRotation);
                count += .5f;
                //obstacle.transform.setparent(splineExtruder.transform, true)
                copiesPerShape = 4;
            }
            else if (rngPlacement < 0.66f && !(firstN < 0.66f)) // Generate Lines
            {
                //move along the spline
                //count = 0.5f;
                CurveSample sample = spline.GetSample(count);
                Vector3 splineLocalLocation = sample.location; //location tracked 2 parents up
                splineLocation = spawnedSplines[spawnedSplines.Count - 1].transform.TransformPoint(splineLocalLocation);
                splineRotation = sample.Rotation;
                splineTan = sample.tangent;
                splineUp = sample.up;
                if ((count >= 2.0f && count <= 3.5f) || (count >= 4.5f && count <= 6.0f)) // So the player doesn't die at the start
                {
                    Lines(splineLocation);
                    ItemLines(splineLocation, rngLines);
                }
                count += .08f;
                copiesPerShape = 3;
            }
            else if (rngPlacement > .66f && !(firstN > .66f)) // Generate Rows
            {
                //move along the spline
                CurveSample sample = spline.GetSample(count);
                Vector3 splineLocalLocation = sample.location; //location tracked 2 parents up
                splineLocation = spawnedSplines[spawnedSplines.Count - 1].transform.TransformPoint(splineLocalLocation);
                splineRotation = sample.Rotation;
                splineTan = sample.tangent;
                splineUp = sample.up;
                if (count >= 1.0f) // So the player doesn't die at the start
                {
                    ItemsLeftAndRight(splineLocalLocation, left);
                    LeftAndRight(splineLocation, left);
                    left = !left; // alternate left and right
                }
                count += .5f;
                copiesPerShape = 5;
            }
            else
            {
                rngPlacement = Random.Range(0.0f, 1.0f);
            }
        }

        return rngPlacement;
    }

    /// <summary>
    /// Place objects in a ring
    /// </summary>
    void Ring(Vector3 centerPosNode)
    {
        float angle = 200f / copiesPerShape;
        float radius = 6.0f;
        for (int i = 0; i < copiesPerShape; i++)
        {
            Vector3 direction = Quaternion.AngleAxis((i * angle) + Random.Range(80, 120), splineTan) * splineUp;
            //Vector3 direction = Vector3.ProjectOnPlane(Random.insideUnitCircle.normalized, splineTan);
            //Vector3 direction = Vector3.ProjectOnPlane(new Vector2(Mathf.Cos(angle * i), Mathf.Sin(angle * i)).normalized, splineTan);

            Vector3 position = centerPosNode + (direction * radius);
            position += splineUp * 9;
            GameObject temp = Instantiate(obstacle, position, splineRotation);//Quaternion.Euler(splineTan));
            temp.transform.SetParent(spawnedSplines[spawnedSplines.Count - 1].transform, true);
        }
    }

    /// <summary>
    /// Place items when its in ring mode
    /// </summary>
    /// <param name="centerPosNode"></param>
    void ItemRing(Vector3 centerPosNode)
    {
        int itemCopies = 3;
        float angle = 200f / itemCopies;
        float radius = 4.0f;
        for (int i = 0; i < itemCopies; i++)
        {
            Vector3 direction = Quaternion.AngleAxis((i * angle) + Random.Range(80, 120), splineTan) * splineUp;

            Vector3 position = centerPosNode + (direction * radius);
            position += splineUp * 9;
            GameObject temp = Instantiate(item, position, splineRotation);//Quaternion.Euler(splineTan));
            temp.transform.SetParent(spawnedSplines[spawnedSplines.Count - 1].transform, true);
        }
    }

    /// <summary>
    /// Place obstacles in rows
    /// </summary>
    void Lines(Vector3 centerPosNode)
    {
        float angle = 25f;
        float radius = 6.0f;
        for (int i = 0; i < copiesPerShape; i++)
        {
            Vector3 direction = Quaternion.AngleAxis((3.65f * i * angle) + 90, splineTan) * splineUp;
            Vector3 position = centerPosNode + (direction * radius);
            position += splineUp * 9;
            GameObject temp = Instantiate(obstacle, position, splineRotation);
            temp.transform.SetParent(spawnedSplines[spawnedSplines.Count - 1].transform, true);
        }
    }

    /// <summary>
    /// Place items when in lines mode
    /// </summary>
    /// <param name="centerPosNode"></param>
    void ItemLines(Vector3 centerPosNode, float rngLines)
    {
        float angle = 25f;
        float radius = 6.0f;
        // Left side
        if (rngLines < 0.5)
        {
            Vector3 direction = Quaternion.AngleAxis(angle + 100, splineTan) * splineUp;
            Vector3 position = centerPosNode + (direction * radius);
            position += splineUp * 9;
            GameObject temp = Instantiate(item, position, splineRotation);
            temp.transform.SetParent(spawnedSplines[spawnedSplines.Count - 1].transform, true);
        }
        // Right side
        else if (rngLines <= 1.0f)
        {
            Vector3 direction = Quaternion.AngleAxis(angle + 200, splineTan) * splineUp;
            Vector3 position = centerPosNode + (direction * radius);
            position += splineUp * 9;
            GameObject temp = Instantiate(item, position, splineRotation);
            temp.transform.SetParent(spawnedSplines[spawnedSplines.Count - 1].transform, true);
        }
    }

    /// <summary>
    /// Place obstacles to make the player move from left to right
    /// </summary>
    /// <param name="centerPosNode"></param>
    void LeftAndRight(Vector3 centerPosNode, bool left)
    {
        float angle = 120f / copiesPerShape;
        float radius = 6.0f;
        Vector3 direction;
        for (int i = 0; i < copiesPerShape; i++)
        {
            if (left)
            {
                direction = Quaternion.AngleAxis((i * angle) + 170, splineTan) * splineUp;
            }
            else
            {
                direction = Quaternion.AngleAxis((i * angle) + 80, splineTan) * splineUp;
            }

            Vector3 position = centerPosNode + (direction * radius);
            position += splineUp * 9;
            GameObject temp = Instantiate(obstacle, position, splineRotation);//Quaternion.Euler(splineTan));
            temp.transform.SetParent(spawnedSplines[spawnedSplines.Count - 1].transform, true);
        }
    }

    /// <summary>
    /// Place items when in left and right mode
    /// </summary>
    /// <param name="centerPosNode"></param>
    /// <param name=""></param>
    void ItemsLeftAndRight(Vector3 centerPosNode, bool left)
    {
        float radius = 6.0f;
        Vector3 direction;

        if (left)
        {
            direction = Quaternion.AngleAxis(120, splineTan) * splineUp;
        }
        else
        {
            direction = Quaternion.AngleAxis(240, splineTan) * splineUp;
        }

        Vector3 position = centerPosNode + (direction * radius);
        position += splineUp * 9;
        GameObject temp = Instantiate(item, position, splineRotation);
        temp.transform.SetParent(spawnedSplines[spawnedSplines.Count - 1].transform, true);
    }
}