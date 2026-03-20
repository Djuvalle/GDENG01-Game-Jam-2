using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float sensitivity;
    private Vector2 mouseInput;
    private float pitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        transform.Rotate(Vector3.up, mouseInput.x * sensitivity * Time.deltaTime);
        pitch -= mouseInput.y * sensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        transform.localEulerAngles = new Vector3(pitch, transform.localEulerAngles.y, 0f);
    }

    private void OnMouseMove(InputValue value)
    {
        mouseInput = value.Get<Vector2>();
    }
}