using UnityEngine;
using UnityEngine.InputSystem;
using GameEnum;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private static Vector3 CAMERA_OFFSET = new Vector3(0, 1, 0);
    [SerializeField]
    private float speed;
    private Camera cam;
    private Rigidbody rb;
    private Vector2 moveInput;
    private bool IsOutOfBounds = false;

    private void FirePlayerPositionChanged()
    {
        if (IsOutOfBounds)
            return;
        
        Vector3 pos = this.transform.position;

        // If player fell off the map
        if (pos.y <= -5)
        {
            IsOutOfBounds = true;
            Debug.Log("Player fell off the map");
            EventBroadcaster.Instance.PostEvent(Notifications.PlayerDied.ToString());
        } else {
            Parameters param = new Parameters();
            param.PutExtra(ParameterKey.X.ToString(), pos.x);
            param.PutExtra(ParameterKey.Y.ToString(), pos.y);
            param.PutExtra(ParameterKey.Z.ToString(), pos.z);

            EventBroadcaster.Instance.PostEvent(Notifications.PlayerPositionChanged.ToString(), param);
        }
        
    }

    private void Start()
    {
        this.rb = GetComponent<Rigidbody>();
        this.cam = Camera.main;
        this.cam.transform.localPosition = CAMERA_OFFSET;
    }

    private void FixedUpdate()
    {
        Vector3 move = cam.transform.forward * moveInput.y + cam.transform.right * moveInput.x;
        move.y = 0;
        rb.AddForce(move.normalized * speed, ForceMode.VelocityChange);
        this.FirePlayerPositionChanged();
    }

    void OnMove(InputValue value)
    {
        this.moveInput = value.Get<Vector2>();
    }
}