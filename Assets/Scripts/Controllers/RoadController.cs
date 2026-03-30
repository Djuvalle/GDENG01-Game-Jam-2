using UnityEngine;
using GameEnum;

public class RoadController : MonoBehaviour
{
    private bool IsEntered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (IsEntered && !other.CompareTag("Player"))
            return;

        IsEntered = true;
        EventBroadcaster.Instance.PostEvent(Notifications.NewRoadEntered.ToString());
    }
}