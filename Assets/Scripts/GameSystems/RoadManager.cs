using UnityEngine;
using GameEnum;
using System.Collections.Generic;
using System.Collections;

public class RoadManager : MonoBehaviour
{
    private const float MIN_SPAWN_RANGE = 200;
    private const float DESTROY_START_DELAY = 5f;
    private const float SLOW_DESTROY_TIME = 10f;
    private const float TIME_REDUCTION_STEP = 0.1f;
    private const float MIN_SLOW_DESTROY_TIME = 1.5f;
    private const float MAX__SLOW_DESTROY_DIST = 100;
    private const string END_POINT = "EndPoint";
    
    private static GameObject RoadContainer;
    private static Dictionary<RoadDirection, GameObject[]> RoadPrefabs = new Dictionary<RoadDirection, GameObject[]>();
    private static Dictionary<RoadDirection, GameObject[]> ObstaclePrefabs = new Dictionary<RoadDirection, GameObject[]>();
    private static Queue<GameObject> SpawnedRoadQueue = new Queue<GameObject>();
    private static Vector3 PlayerPosition = Vector3.zero;
    [SerializeField] private GameObject StartingRoad;
    private GameObject PrevEndObj;
    private int CurrentAxisDirection = 0;
    private int RoadsPassed = 0;

    private void PostEventWaypoint(GameObject prevEndObj)
    {
        Vector3 pos = prevEndObj.GetComponent<Transform>().position;

        Parameters param = new Parameters();
        param.PutExtra(ParameterKey.X.ToString(), pos.x);
        param.PutExtra(ParameterKey.Y.ToString(), pos.y);
        param.PutExtra(ParameterKey.Z.ToString(), pos.z);

        EventBroadcaster.Instance.PostEvent(Notifications.WaypointAdded.ToString(), param);
    }
    private RoadDirection RollObstacleType(RoadDirection roadDirection)
    {
        int roll = Random.Range((int) 0, (int) 101);
        if (roll >= 75)
            return RoadDirection.General;

        return roadDirection;
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
        //Debug.Log($"Target Dir: {targetDir} Current Axis: {this.CurrentAxisDirection}");

        GameObject newRoad = Instantiate(
            RoadPrefabs[targetDir][idx],
            this.PrevEndObj.transform.position,
            this.PrevEndObj.transform.rotation,
            RoadContainer.transform
        );
        newRoad.tag = "Road";
        GameObject endPoint = newRoad.transform.Find(END_POINT).gameObject;

        RoadDirection obstacleDir = RollObstacleType(targetDir);
        GameObject newObstacle = Instantiate(
            ObstaclePrefabs[obstacleDir][Random.Range(0, ObstaclePrefabs[obstacleDir].Length)],
            newRoad.transform.position,
            newRoad.transform.rotation,
            newRoad.transform
        );

        this.PrevEndObj = endPoint;
        SpawnedRoadQueue.Enqueue(newRoad);
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
                
            GameObject oldRoad = SpawnedRoadQueue.Dequeue();
            GameObject oldEndPoint = oldRoad.transform.Find(END_POINT).gameObject;

            // Additional code here
            if ((oldEndPoint.transform.position - PlayerPosition).magnitude > MAX__SLOW_DESTROY_DIST)
            {
                Debug.Log("Quick destroy old road");
                yield return null;
            } else {
                Debug.Log("Slow destroy old road");
                // TODO: Add destroy indicator here
                float destroyTime = Mathf.Max(
                    MIN_SLOW_DESTROY_TIME,
                    SLOW_DESTROY_TIME - RoadsPassed * TIME_REDUCTION_STEP
                );
                Debug.Log($"Waiting time: {destroyTime}");
                yield return new WaitForSeconds(destroyTime);
            }
            
            if (oldRoad != null)
                Destroy(oldRoad);

            // TODO: Destroy effect here
        }
    }

    private void HandleNewRoadEntered()
    {
        RoadsPassed++;
    }

    private void Start()
    {   
        RoadContainer = GameObject.Find("RoadContainer");

        const string ROAD_PATH = "Prefabs/Road/";
        RoadPrefabs.Add(RoadDirection.Straight, Resources.LoadAll<GameObject>($"{ROAD_PATH}{RoadDirection.Straight.ToString()}/"));
        RoadPrefabs.Add(RoadDirection.Right, Resources.LoadAll<GameObject>($"{ROAD_PATH}{RoadDirection.Right.ToString()}/"));
        RoadPrefabs.Add(RoadDirection.Left, Resources.LoadAll<GameObject>($"{ROAD_PATH}{RoadDirection.Left.ToString()}/"));
        
        Debug.Log("Number of Road prefabs found\n" +
            $"Straight: {RoadPrefabs[RoadDirection.Straight].Length}\n" +
            $"Right: {RoadPrefabs[RoadDirection.Right].Length}\n" + 
            $"Left: {RoadPrefabs[RoadDirection.Left].Length}"
        );

        const string OBSTACLE_PATH = "Prefabs/Obstacle/";
        ObstaclePrefabs.Add(RoadDirection.General, Resources.LoadAll<GameObject>($"{OBSTACLE_PATH}{RoadDirection.General.ToString()}/"));
        ObstaclePrefabs.Add(RoadDirection.Straight, Resources.LoadAll<GameObject>($"{OBSTACLE_PATH}{RoadDirection.Straight.ToString()}/"));
        ObstaclePrefabs.Add(RoadDirection.Right, Resources.LoadAll<GameObject>($"{OBSTACLE_PATH}{RoadDirection.Right.ToString()}/"));
        ObstaclePrefabs.Add(RoadDirection.Left, Resources.LoadAll<GameObject>($"{OBSTACLE_PATH}{RoadDirection.Left.ToString()}/"));
        
        Debug.Log("Number of Obstacle prefabs found\n" +
            $"General: {ObstaclePrefabs[RoadDirection.General].Length}\n" +
            $"Straight: {ObstaclePrefabs[RoadDirection.Straight].Length}\n" +
            $"Right: {ObstaclePrefabs[RoadDirection.Right].Length}\n" + 
            $"Left: {ObstaclePrefabs[RoadDirection.Left].Length}"
        );

        this.PrevEndObj = StartingRoad.transform.Find(END_POINT).gameObject;
        SpawnedRoadQueue.Enqueue(StartingRoad);
        //this.PostEventWaypoint(PrevEndObj);

        EventBroadcaster.Instance.AddObserver(Notifications.NewRoadEntered.ToString(), this.HandleNewRoadEntered);
        EventBroadcaster.Instance.AddObserver(Notifications.PlayerPositionChanged.ToString(), this.HandlePlayerPositionChanged);
        StartCoroutine(HandleRoadDestruction());
    }
}