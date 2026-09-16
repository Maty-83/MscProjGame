using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


public enum GaugeType
{
    FromMidpoint,
    OppositeSideAsZero
}
public class SpeedGauge : MonoBehaviour
{
    [Header("Speed Indicator sub-objects")]
    public List<GameObject> SpeedIndicatorBlips = new List<GameObject>(); //This is what we'll be coloring based on current velocity
    public List<GameObject> AccelerationIndicatorBlips=new List<GameObject>();//If used (We need to solve getting last frame acceleration, but that's just an extra function), it displays how much % thrust is used.
    public ShipMovementProto ShipMovementScript;
    public GaugeType gaugeType;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
