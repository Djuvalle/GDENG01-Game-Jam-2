using UnityEngine;
using UnityEngine.InputSystem;
using GameEnum;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Transform))]
public class PlayerController : MonoBehaviour
{
    private static Vector3 CAMERA_OFFSET = new Vector3(0, 1, 0);
    [SerializeField]
    private float speed;
    private Camera cam;
    private Rigidbody rb;
    private Transform transform;
    private Vector2 moveInput;

    private void FirePlayerPositionChanged()
    {
        Vector3 pos = this.transform.position;

        Parameters param = new Parameters();
        param.PutExtra(ParameterKey.X.ToString(), pos.x);
        param.PutExtra(ParameterKey.Y.ToString(), pos.y);
        param.PutExtra(ParameterKey.Z.ToString(), pos.z);

        EventBroadcaster.Instance.PostEvent(Notifications.PlayerPositionChanged.ToString(), param);
    }

    private void Start()
    {
        this.rb = GetComponent<Rigidbody>();
        this.transform = GetComponent<Transform>();
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