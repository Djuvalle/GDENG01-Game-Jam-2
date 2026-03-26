using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Car : MonoBehaviour
{
    private const float STEERING_DAMPING = 120f; // Degrees per second
    [SerializeField] private float MaxSpeed = 30;
    [SerializeField] private float Acceleration = 2000f;
    [SerializeField] private float BrakeForce = 3000f;
    [SerializeField] private float SteerSpeed = 35;
    [SerializeField] private WheelCollider[] Wheels = new WheelCollider[4]; // Assumption: First two wheels are front wheels
    
    private Rigidbody rb;
    private Vector2 moveInput;
    private float currentSteerAngle = 0f;

    private void Start()
    {
        this.rb = GetComponent<Rigidbody>();
        foreach(WheelCollider wheel in Wheels)
        {
            wheel.gameObject.AddComponent<Wheel>();
        }
    }

    private void FixedUpdate()
    {
        // Vector3 move = driveSpeed * moveInput.y + cam.transform.right * moveInput.x;
        // this.FirePlayerPositionChanged();
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        float motorTorque = 0;
        
        bool isBelowMaxSpeed = moveInput.y != 0 && (
            (moveInput.y > 0 && forwardSpeed < MaxSpeed) ||
            (moveInput.y < 0 && forwardSpeed > -MaxSpeed)
        );

        if (isBelowMaxSpeed)
        {
            motorTorque = Acceleration * moveInput.y;
        }

        foreach(WheelCollider wheel in Wheels)
            wheel.motorTorque = motorTorque;

        float targetSteerAngle = SteerSpeed * moveInput.x;
        this.currentSteerAngle = Mathf.MoveTowards(this.currentSteerAngle, targetSteerAngle, STEERING_DAMPING * Time.fixedDeltaTime);
        Wheels[0].steerAngle = this.currentSteerAngle;
        Wheels[1].steerAngle = this.currentSteerAngle;
    }

    void OnMove(InputValue value)
    {
        this.moveInput = value.Get<Vector2>();
    }
}