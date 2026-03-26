

using UnityEngine;
using UnityEngine.InputSystem;

public class Wheel : MonoBehaviour
{
    private WheelCollider WheelCollider;
    private Transform WheelModel;

    private void Start()
    {
        this.WheelCollider = GetComponent<WheelCollider>();
        this.WheelModel = this.transform.Find("WheelModel");
    }

    private void Update()
    {
        this.WheelModel.localPosition = new Vector3(0, -this.WheelCollider.suspensionSpring.targetPosition + this.WheelCollider.radius);
        this.WheelModel.localEulerAngles = new Vector3(
            this.WheelModel.localEulerAngles.x,
            this.WheelCollider.steerAngle - this.WheelModel.localEulerAngles.z,
            this.WheelModel.localEulerAngles.z
        );
        this.WheelModel.Rotate(WheelCollider.rpm / 60 * 360 * Time.deltaTime, 0, 0);
    }
}