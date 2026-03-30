using UnityEngine;
using GameEnum;

public class CoinController : MonoBehaviour
{
    private const float ROTATION_SPEED = 100f;
    private bool IsCollected = false;
    private void OnTriggerEnter(Collider other)
    {
        if (IsCollected && !other.CompareTag("Player"))
            return;

        IsCollected = true;
        // TODO: Add particles here
        EventBroadcaster.Instance.PostEvent(Notifications.CoinCollected.ToString());
        Destroy(this.gameObject);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, ROTATION_SPEED * Time.deltaTime);
    }
}