using UnityEngine;
using GameEnum;

public class TerrainGenerator : MonoBehaviour
{
    private const float MIN_SPAWN_RANGE = 10;
    private const string TERRAIN_PATH = "Prefabs/Terrain/";
    private static GameObject TerrainContainer;
    private static GameObject[] TerrainPrefabs;
    private static Vector3 PlayerPosition = Vector3.zero;
    [SerializeField] private GameObject PrevEndObj;

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
    private void Start()
    {   
        TerrainContainer = GameObject.Find("TerrainContainer");
        TerrainPrefabs = Resources.LoadAll<GameObject>(TERRAIN_PATH);
        this.PostEventWaypoint(PrevEndObj);
        EventBroadcaster.Instance.AddObserver(Notifications.PlayerPositionChanged.ToString(), this.HandlePlayerPositionChanged);
        Debug.Log($"Number of terrain prefabs found: {TerrainPrefabs.Length}");
    }
}