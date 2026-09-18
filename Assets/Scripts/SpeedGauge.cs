using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


public enum GaugeType
{
    FromMidpoint,
    OppositeSideAsZero
}
public enum GaugeAxis
{
    Forward,
    Vertical,
    Lateral,
    Pitch,
    Yaw,
    Roll,
    TotalVelocity

}
public class SpeedGauge : MonoBehaviour
{
    [Header("Speed Indicator sub-objects")]
    public List<GameObject> SpeedIndicatorBlips = new List<GameObject>(); //This is what we'll be coloring based on current velocity
    public List<GameObject> AccelerationIndicatorBlips=new List<GameObject>();//If used (We need to solve getting last frame acceleration, but that's just an extra function), it displays how much % thrust is used.
    public GameObject ThrottlePin;//If we set a throttle to exist, the throttle pin becomes visible and indicates to which level throttle is supposed to go.
    public ShipMovementProto ShipMovementScript;
    public GaugeType gaugeType;
    public GaugeAxis gaugeAxis;
    private float GetLastFrameAccPercent()
    {
        switch (gaugeAxis)
        {
            case GaugeAxis.Forward:
                return ShipMovementScript.LastFrameInputPercentLinear.x;
            case GaugeAxis.Vertical:
                return ShipMovementScript.LastFrameInputPercentLinear.y;
            case GaugeAxis.Lateral:
                return ShipMovementScript.LastFrameInputPercentLinear.z;
            case GaugeAxis.Pitch:
                return ShipMovementScript.LastFrameInputPercentRotation.z;
            case GaugeAxis.Yaw:
                return ShipMovementScript.LastFrameInputPercentRotation.y;
            case GaugeAxis.Roll:
                return ShipMovementScript.LastFrameInputPercentRotation.x;
            case GaugeAxis.TotalVelocity:
                return ShipMovementScript.LastFrameInputPercentLinear.x;
            default:
                Debug.Log("[ERROR]: unknown gauge axis");
                return 0;
        }
    }
    private float GetVelocity()
    {
        Rigidbody rb = ShipMovementScript.GetComponent<Rigidbody>();
        Vector3 velocityInDirs = Quaternion.Inverse(rb.rotation) * rb.linearVelocity;
        Vector3 localAngularVelocity = transform.InverseTransformDirection(rb.angularVelocity);
        switch (gaugeAxis)
        {
            case GaugeAxis.Forward:
                return velocityInDirs.x;
            case GaugeAxis.Vertical:
                return velocityInDirs.y;
            case GaugeAxis.Lateral:
                return velocityInDirs.z;
            case GaugeAxis.Pitch:
                return localAngularVelocity.z;
            case GaugeAxis.Yaw:
                return localAngularVelocity.y;
            case GaugeAxis.Roll:
                return localAngularVelocity.x;
            case GaugeAxis.TotalVelocity:
                return velocityInDirs.magnitude;
            default:
                Debug.Log("[ERROR]: unknown gauge axis");
                return 0;
        }
    }
    private void GetMaxVelocities(out float max, out float min)
    {
        max= 0;
        min = 0;
        switch (gaugeAxis)
        {
            case GaugeAxis.Forward:
                max = ShipMovementScript.maxVel;
                min = ShipMovementScript.maxReverseVel;
                break;
            case GaugeAxis.Vertical:
                max = ShipMovementScript.maxVerticalVel;
                break;
            case GaugeAxis.Lateral:
                max = ShipMovementScript.maxLateralVel;
                break;
            case GaugeAxis.Pitch:
                max = ShipMovementScript.maxPitchVel;
                break;
            case GaugeAxis.Yaw:
                max= ShipMovementScript.maxYawVel;
                break;
            case GaugeAxis.Roll:
                max= ShipMovementScript.maxRollVel;
                break;
            case GaugeAxis.TotalVelocity:
                max = ShipMovementScript.maxVel;
                break;
            default:
                Debug.Log("[ERROR]: unknown gauge axis");
                return;
        }
        if (min == 0) min = -max;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}