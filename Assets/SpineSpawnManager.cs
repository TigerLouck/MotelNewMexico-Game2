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
    private List<GameObject> spawnedSplines;

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
        // /UpdateTimer();
    }

    private void SpawnRandomSpline ()
    {
        if (transform.childCount <= 0)
        {
            // if not other splines to base position off of, start at origin
            GameObject temp = Instantiate (splinePrefabs[GetRandomSpine ()]);
            temp.transform.parent = splineParent.transform;
            spawnedSplines.Add (temp);
        }
        else
        {
            Vector3 lastPosition = GetEndPosition (spawnedSplines[spawnedSplines.Count - 1]);
            Vector3 lastDirection = GetEndDirection (spawnedSplines[spawnedSplines.Count - 1]);
            GameObject temp = Instantiate (splinePrefabs[GetRandomSpine ()]);

            Transform currentTransform = spawnedSplines[spawnedSplines.Count - 1].transform;
            Vector3 newPos = currentTransform.TransformPoint (lastPosition);
            temp.transform.position = newPos;

            Vector3 diff = currentTransform.TransformPoint (lastDirection);
            Vector3 newDir = (diff - newPos).normalized;
            temp.transform.Rotate(newDir);
        }

    }

    private void UpdateTimer ()
    {
        currentTime += Time.fixedDeltaTime;
        if (currentTime >= spawnTimer)
        {
            SpawnRandomSpline ();
        }
    }

    private int GetRandomSpine ()
    {
        int rand = Random.Range (0, splinePrefabs.Count);
        return rand;
    }

    private Vector3 GetEndPosition (GameObject spline)
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