using UnityEngine;
using GameEnum;
using System.Collections;

public class CoinController : MonoBehaviour
{
    [SerializeField] private ParticleSystem CollectEffect;
    private const float ROTATION_SPEED = 100f;
    private bool IsCollected = false;
    IEnumerator DestroyAfterEffect()
    {
        yield return new WaitForSeconds(CollectEffect.main.startLifetime.constant);
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsCollected && !other.CompareTag("Player"))
            return;

        IsCollected = true;
        transform.Find("Coin.tris").gameObject.SetActive(false);
        CollectEffect.Emit(1);
        EventBroadcaster.Instance.PostEvent(Notifications.CoinCollected.ToString());
        StartCoroutine(DestroyAfterEffect());
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, ROTATION_SPEED * Time.deltaTime);
    }
}