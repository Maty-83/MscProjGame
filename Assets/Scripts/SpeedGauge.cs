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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
