using UnityEngine;
using UnityEngine.InputSystem;
using GameEnum;

public class CarController : MonoBehaviour
{
    private const float STEERING_DAMPING = 120f; // Degrees per second
    [SerializeField] private float MaxSpeed = 30;
    [SerializeField] private float Acceleration = 2000f;
    [SerializeField] private float BrakeForce = 3000f;
    [SerializeField] private float SteerSpeed = 35;
    [SerializeField] private WheelCollider[] Wheels = new WheelCollider[4]; // Assumption: First two wheels are front wheels
    [SerializeField] private ParticleSystem CollisionEffect;

    private Rigidbody rb;
    private Vector2 moveInput;
    private float currentSteerAngle = 0f;
    private bool IsOutOfBounds = false;

    private void FirePlayerPositionChanged(float forwardSpeed = 0f)
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
            param.PutExtra(ParameterKey.Speed.ToString(), forwardSpeed);

            EventBroadcaster.Instance.PostEvent(Notifications.PlayerPositionChanged.ToString(), param);
        }
        
    }


    private void Start()
    {
        this.rb = GetComponent<Rigidbody>();
        foreach(WheelCollider wheel in Wheels)
        {
            wheel.gameObject.AddComponent<WheelController>();
        }
    }
    
    private void FixedUpdate()
    {
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        float motorTorque = 0;
        float brakeTorque = 0;

        bool isReversingWhileInDrive = forwardSpeed > 0.5f && moveInput.y < 0;
        bool isInDriveWhileReversing = forwardSpeed < -0.5f && moveInput.y > 0;
        bool isBraking = isReversingWhileInDrive || isInDriveWhileReversing;

        if (isBraking)
            brakeTorque = BrakeForce;
        else if (moveInput.y == 0)
            brakeTorque = BrakeForce * 0.1f;
        else if (Mathf.Abs(forwardSpeed) < MaxSpeed)
            motorTorque = Acceleration * moveInput.y;

        foreach(WheelCollider wheel in Wheels)
            wheel.brakeTorque = brakeTorque;

        Wheels[2].motorTorque = motorTorque;
        Wheels[3].motorTorque = motorTorque;

        float targetSteerAngle = SteerSpeed * moveInput.x;
        this.currentSteerAngle = Mathf.MoveTowards(this.currentSteerAngle, targetSteerAngle, STEERING_DAMPING * Time.fixedDeltaTime);
        Wheels[0].steerAngle = this.currentSteerAngle;
        Wheels[1].steerAngle = this.currentSteerAngle;

        this.FirePlayerPositionChanged(forwardSpeed);
    }

    void OnMove(InputValue value)
    {
        this.moveInput = value.Get<Vector2>();
        //Debug.Log($"Moved: {moveInput.ToString()}");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Road"))
            return;
    
        // Get the first point of contact
        ContactPoint contact = collision.contacts[0];
        Vector3 pos = contact.point;
        CollisionEffect.transform.position = pos;
        CollisionEffect.Emit(30);

        Debug.Log("Hit at: " + pos);
    }
}