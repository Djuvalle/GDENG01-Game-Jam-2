using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private const float HORIZONTAL_OFFSET = 8f;
    private const float VERTICAL_OFFSET = 1.5f;
    [SerializeField]
    private float sensitivity;
    private Vector2 mouseInput;
    private float pitch;
    private float yaw;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        yaw = transform.eulerAngles.y;
    }

    private void LateUpdate()
    {
        //yaw += mouseInput.x * sensitivity * Time.deltaTime;
        //pitch -= mouseInput.y * sensitivity * Time.deltaTime;
        yaw = transform.eulerAngles.y;
        pitch = 30f;
        pitch = Mathf.Clamp(pitch, -30f, 60f); // clamp for better vehicle viewing
        
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.rotation = targetRotation;

        Vector3 offsetVector = targetRotation * Vector3.forward * -HORIZONTAL_OFFSET;
        transform.position = transform.parent.position + offsetVector + new Vector3(0, VERTICAL_OFFSET, 0);
    }

    private void OnMouseMove(InputValue value)
    {
        mouseInput = value.Get<Vector2>();
    }
}