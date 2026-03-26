using UnityEngine;
using GameEnum;
using System.Collections.Generic;
using System.Collections;

public class RoadManager : MonoBehaviour
{
    private const float MIN_SPAWN_RANGE = 50;
    private const float DESTROY_START_DELAY = 5f;
    private const float SLOW_DESTROY_TIME = 5;
    private const float MAX__SLOW_DESTROY_DIST = 100;
    private const string END_POINT = "EndPoint";
    
    private static GameObject RoadContainer;
    private static Dictionary<RoadDirection, GameObject[]> RoadPrefabs = new Dictionary<RoadDirection, GameObject[]>();
    private static Queue<GameObject> SpawnedRoadQueue = new Queue<GameObject>();
    private static Vector3 PlayerPosition = Vector3.zero;
    [SerializeField] private GameObject StartingRoad;
    private GameObject PrevEndObj;
    private int CurrentAxisDirection = 0;

    private void PostEventWaypoint(GameObject prevEndObj)
    {
        Vector3 pos = prevEndObj.GetComponent<Transform>().position;

        Parameters param = new Parameters();
        param.PutExtra(ParameterKey.X.ToString(), pos.x);
        param.PutExtra(ParameterKey.Y.ToString(), pos.y);
        param.PutExtra(ParameterKey.Z.ToString(), pos.z);

        EventBroadcaster.Instance.PostEvent(Notifications.WaypointAdded.ToString(), param);
    }
    private RoadDirection RollTargetDirection()
    {
        switch(Random.Range((int) -1, (int) 2))
        {
            case -1:
                if (CurrentAxisDirection == -1)
                    return RoadDirection.Straight;
                else
                {
                    CurrentAxisDirection--;
                    return RoadDirection.Left;
                }
            case 1:
                if (CurrentAxisDirection == 1)
                    return RoadDirection.Straight;
                else
                {
                    CurrentAxisDirection++;
                    return RoadDirection.Right;
                }
                    
            case 0:
            default:
                return RoadDirection.Straight;
        }
    }
    private void TryGeneration()
    {
        Vector3 diff = PrevEndObj.transform.position - PlayerPosition;
        if (diff.magnitude > MIN_SPAWN_RANGE)
            return;

        RoadDirection targetDir = RollTargetDirection();
        int idx = Random.Range(0, RoadPrefabs[targetDir].Length);
        Debug.Log($"Target Dir: {targetDir} Current Axis: {this.CurrentAxisDirection}");

        GameObject newRoad = Instantiate(
            RoadPrefabs[targetDir][idx],
            this.PrevEndObj.transform.position,
            this.PrevEndObj.transform.rotation,
            RoadContainer.transform
        );
        GameObject endPoint = newRoad.transform.Find(END_POINT).gameObject;

        this.PrevEndObj = endPoint;
        SpawnedRoadQueue.Enqueue(newRoad);

        // TODO: Broadcast new Road added and add old Road removal
    }
    private void HandlePlayerPositionChanged(Parameters parameters)
    {
        //Debug.Log("Update Player Position");
        float x = parameters.GetFloatExtra(ParameterKey.X.ToString(), 0);
        float y = parameters.GetFloatExtra(ParameterKey.Y.ToString(), 0);
        float z = parameters.GetFloatExtra(ParameterKey.Z.ToString(), 0);

        PlayerPosition = new Vector3(x, y, z);
        this.TryGeneration();
    }

    IEnumerator HandleRoadDestruction()
    {
        yield return new WaitForSeconds(DESTROY_START_DELAY);
        Debug.Log("Destroy loop started");

        while (true)
        {
            if (SpawnedRoadQueue.Count == 0)
                yield return new WaitUntil(() => SpawnedRoadQueue.Count > 0);
                
            GameObject road = SpawnedRoadQueue.Dequeue();

            // Additional code here

            if (SpawnedRoadQueue.Count >= MAX__SLOW_DESTROY_SIZE)
            {
                Debug.Log("Quick destroy road");
                yield return null;
            } else {
                Debug.Log("Slow destroy road");
                yield return new WaitForSeconds(SLOW_DESTROY_TIME);
            }
                
            
            if (road != null)
                Destroy(road);
        }
    }

    private void Start()
    {   
        const string ROAD_PATH = "Prefabs/Road/";
        RoadContainer = GameObject.Find("RoadContainer");
        RoadPrefabs.Add(RoadDirection.Straight, Resources.LoadAll<GameObject>($"{ROAD_PATH}{RoadDirection.Straight.ToString()}/"));
        RoadPrefabs.Add(RoadDirection.Right, Resources.LoadAll<GameObject>($"{ROAD_PATH}{RoadDirection.Right.ToString()}/"));
        RoadPrefabs.Add(RoadDirection.Left, Resources.LoadAll<GameObject>($"{ROAD_PATH}{RoadDirection.Left.ToString()}/"));
        Debug.Log("Number of Road prefabs found\n" +
            $"Straight: {RoadPrefabs[RoadDirection.Straight].Length}\n" +
            $"Right: {RoadPrefabs[RoadDirection.Right].Length}\n" + 
            $"Left: {RoadPrefabs[RoadDirection.Left].Length}"
        );

        this.PrevEndObj = StartingRoad.transform.Find(END_POINT).gameObject;
        SpawnedRoadQueue.Enqueue(StartingRoad);
        //this.PostEventWaypoint(PrevEndObj);

        EventBroadcaster.Instance.AddObserver(Notifications.PlayerPositionChanged.ToString(), this.HandlePlayerPositionChanged);
        StartCoroutine(HandleRoadDestruction());
    }
}