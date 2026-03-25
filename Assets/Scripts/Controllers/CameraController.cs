using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private const float HORIZONTAL_OFFSET = 5f;
    private const float VERTICAL_OFFSET = 1.25f;
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
        transform.localPosition = transform.forward * -HORIZONTAL_OFFSET + new Vector3(0, VERTICAL_OFFSET);
    }

    private void OnMouseMove(InputValue value)
    {
        mouseInput = value.Get<Vector2>();
    }
}