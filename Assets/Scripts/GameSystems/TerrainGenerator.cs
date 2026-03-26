using UnityEngine;
using GameEnum;
using System.Collections.Generic;
using System.Collections;

public class TerrainGenerator : MonoBehaviour
{
    private const float MIN_SPAWN_RANGE = 50;
    private const float DESTROY_START_DELAY = 5f;
    private const float SLOW_DESTROY_TIME = 5;
    private const float MAX__SLOW_DESTROY_SIZE = 10;
    
    
    private static GameObject TerrainContainer;
    private static GameObject[] TerrainPrefabs;
    private static Queue<GameObject> SpawnedTerrainQueue = new Queue<GameObject>();
    private static Vector3 PlayerPosition = Vector3.zero;
    [SerializeField] private GameObject StartingTerrain;
    private GameObject PrevEndObj;

    private void PostEventWaypoint(GameObject prevEndObj)
    {
        Vector3 pos = prevEndObj.GetComponent<Transform>().position;

        Parameters param = new Parameters();
        param.PutExtra(ParameterKey.X.ToString(), pos.x);
        param.PutExtra(ParameterKey.Y.ToString(), pos.y);
        param.PutExtra(ParameterKey.Z.ToString(), pos.z);

        EventBroadcaster.Instance.PostEvent(Notifications.WaypointAdded.ToString(), param);
    }
    private void TryGeneration()
    {
        Vector3 diff = PrevEndObj.transform.position - PlayerPosition;
        if (diff.magnitude <= MIN_SPAWN_RANGE)
        {
            GameObject newTerrain = Instantiate(
                TerrainPrefabs[Random.Range(0, TerrainPrefabs.Length - 1)],
                this.PrevEndObj.transform.position,
                this.PrevEndObj.transform.rotation,
                TerrainContainer.transform
            );
            this.PrevEndObj = newTerrain.transform.Find("EndPoint").gameObject;
            SpawnedTerrainQueue.Enqueue(newTerrain);

            //TODO: Broadcast new terrain added and add old terrain removal
        }
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

    IEnumerator HandleTerrainDestruction()
    {
        yield return new WaitForSeconds(DESTROY_START_DELAY);
        Debug.Log("Destroy loop started");

        while (true)
        {
            if (SpawnedTerrainQueue.Count == 0)
                yield return new WaitUntil(() => SpawnedTerrainQueue.Count > 0);
                
            GameObject terrain = SpawnedTerrainQueue.Dequeue();

            // Additional code here

            if (SpawnedTerrainQueue.Count >= MAX__SLOW_DESTROY_SIZE)
            {
                Debug.Log("Quick destroy terrain");
                yield return null;
            } else {
                Debug.Log("Slow destroy terrain");
                yield return new WaitForSeconds(SLOW_DESTROY_TIME);
            }
                
            
            if (terrain != null)
                Destroy(terrain);
        }
    }

    private void Start()
    {   
        const string TERRAIN_PATH = "Prefabs/Terrain/";
        TerrainContainer = GameObject.Find("TerrainContainer");
        TerrainPrefabs = Resources.LoadAll<GameObject>(TERRAIN_PATH);
        Debug.Log($"Number of terrain prefabs found: {TerrainPrefabs.Length}");

        this.PrevEndObj = StartingTerrain.transform.Find("EndPoint").gameObject;
        SpawnedTerrainQueue.Enqueue(StartingTerrain);
        //this.PostEventWaypoint(PrevEndObj);

        EventBroadcaster.Instance.AddObserver(Notifications.PlayerPositionChanged.ToString(), this.HandlePlayerPositionChanged);
        StartCoroutine(HandleTerrainDestruction());
    }
}