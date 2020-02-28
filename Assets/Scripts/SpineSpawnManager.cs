using System.Collections;
using System.Collections.Generic;
using SplineMesh;
using UnityEngine;

public class SpineSpawnManager : MonoBehaviour
{
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

    void Start ()
    {
        currentTime = 0f;
        spawnedSplines = new List<GameObject> ();
        SpawnRandomSpline ();
    }

    // Update is called once per frame
    void Update ()
    {

    }

    private void FixedUpdate() {
        UpdateTimer();
    }

    private void SpawnRandomSpline ()
    {
        if (spawnedSplines.Count <= 0)
        {
            // if not other splines to base position off of, start at origin
            GameObject temp = Instantiate (splinePrefabs[GetRandomSpine ()]);
            //temp.transform.parent = splineParent.transform;
            spawnedSplines.Add (temp);
            Debug.Log("Here");
        }
        else
        {
            Vector3 lastPosition = GetEndPosition (spawnedSplines[spawnedSplines.Count - 1]);
            Vector3 lastDirection = GetEndDirection (spawnedSplines[spawnedSplines.Count - 1]);
            GameObject temp = Instantiate (splinePrefabs[GetRandomSpine ()]);

            

            Transform currentTransform = spawnedSplines[spawnedSplines.Count - 1].transform.GetChild(0);
            Transform tempG = currentTransform.GetChild(0);

            // parenting of splines makes this not work properly
            Vector3 newPos = spawnedSplines[spawnedSplines.Count - 1].transform.GetChild(0).transform.TransformPoint(lastPosition);

            //newPos*=5f;
            
            Vector3 diff = currentTransform.TransformPoint (lastDirection);
            //diff*=5.019f;
            Vector3 newDir = (diff - newPos).normalized;

            //Debug.Log(newPos);

            temp.transform.position = newPos;
            temp.transform.Rotate(newDir);
            spawnedSplines.Add (temp);

            
        }

    }

    private void UpdateTimer ()
    {
        currentTime += Time.fixedDeltaTime;
        if (currentTime >= spawnTimer)
        {
            SpawnRandomSpline ();
            currentTime=0f;
        }
    }

    private int GetRandomSpine ()
    {
        int rand = Random.Range (0, splinePrefabs.Count);
        return rand;
    }

    public Vector3 GetEndPosition (GameObject spline)
    {
        List<SplineNode> nodes = spline.GetComponentInChildren<Spline> ().nodes;
        SplineNode lastNode = nodes[nodes.Count - 1];
        return lastNode.Position;
    }

    private Vector3 GetEndDirection (GameObject spline)
    {
        List<SplineNode> nodes = spline.GetComponentInChildren<Spline> ().nodes;
        SplineNode lastNode = nodes[nodes.Count - 1];
        return lastNode.Direction;
    }
}