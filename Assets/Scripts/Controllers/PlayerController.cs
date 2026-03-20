using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private static Vector3 CAMERA_OFFSET = new Vector3(0, 1, 0);
    [SerializeField]
    private float speed;
    private Camera cam;
    private Rigidbody rb;
    private Vector2 moveInput;


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

    }

    void OnMove(InputValue value)
    {
        this.moveInput = value.Get<Vector2>();
    }
}