using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private const float HORIZONTAL_OFFSET = 8f;
    private const float VERTICAL_OFFSET = 6f; //1.5f;
    private GameObject Player;
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
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        //yaw += mouseInput.x * sensitivity * Time.deltaTime;
        //pitch -= mouseInput.y * sensitivity * Time.deltaTime;
        yaw = Player.transform.eulerAngles.y;
        pitch = 30f;
        pitch = Mathf.Clamp(pitch, -30f, 60f); // clamp for better vehicle viewing
        
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);

        Vector3 offsetVector = /*targetRotation **/ Player.transform.forward * -HORIZONTAL_OFFSET;
        transform.position = Player.transform.position + offsetVector + new Vector3(0, VERTICAL_OFFSET, 0);
    }

    private void OnMouseMove(InputValue value)
    {
        mouseInput = value.Get<Vector2>();
    }
}