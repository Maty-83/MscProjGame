using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

/*public enum GaugeType
{
    FromMidpoint,
    OppositeSideAsZero
}*/
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
    public List<Image> SpeedIndicatorBlips = new List<Image>(); //This is what we'll be coloring based on current velocity
    public List<Image> AccelerationIndicatorBlips = new List<Image>();//If used (We need to solve getting last frame acceleration, but that's just an extra function), it displays how much % thrust is used.
    public GameObject ThrottlePin;//If we set a throttle to exist, the throttle pin becomes visible and indicates to which level throttle is supposed to go.
    public ShipMovementProto ShipMovementScript;
    /*public GaugeType gaugeType;*/
    public GaugeAxis gaugeAxis;
    public TMP_Text SpeedTextUI;
    public TMP_Text AccelTextUI;
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
        max = 0;
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
                max = ShipMovementScript.maxYawVel;
                break;
            case GaugeAxis.Roll:
                max = ShipMovementScript.maxRollVel;
                break;
            case GaugeAxis.TotalVelocity:
                max = ShipMovementScript.maxVel;
                break;
            default:
                Debug.Log("[ERROR]: unknown gauge axis");
                return;
        }
        if (min == 0) min = max;
    }
    /// <summary>
    /// Sets UI blips based on given input fraction from interval [-1,1]. If the list is odd, the central blip always stays lit unless fraction is precisely zero.
    /// </summary>
    /// <param name="blips">Total list of UI blips sorted in order from input=-1 to input=1. Minimum length of list should be 3</param>
    /// <param name="inputFraction">input fraction from interval [-1,1]</param>
    private void SetGaugeFromPercent(List<Image> blips, float inputFraction)
    {
        const float AlphaLowerBound = 0.2f;
        const float AlphaUpperBound = 0.8f;
        if (blips.Count < 3)
        {
            Debug.Log("UI error-Incorrect linkage of UI blips");
            return;
        }
        //First, set alpha of all blips to lower bound.
        for(int i = 0; i < blips.Count; i++)
        {
            var color = blips[i].color;
            color.a = AlphaLowerBound;
            blips[i].color = color;
        }
        //If input's zero, we can just do nothing.
        if (inputFraction == 0)
        {
            return;
        }
        //If input is zero and there's a central blip, we light it up.
        if (blips.Count % 2 == 1)
        {
            var col = blips[(blips.Count - 1) / 2].color;
            col.a = AlphaUpperBound;
            blips[(blips.Count - 1) / 2].color = col;
        }

        var blipsPerDir = (blips.Count - 2) / 2;//Amount of blips on each side we split to show rough percentage visually.
        float blipFraction = 1f / blipsPerDir;
        int curHead = 0;//which one do we start at?

        if (inputFraction > 0)
        {
            curHead = (blips.Count + 1) / 2;//We go to 1/2 of blips +1. At 12, this means 13/2=6 in int, which is correct. At 14, 14/2=7, which is also correct, since index 6 is central
        }
        if (inputFraction < 0)
        {
            curHead=(blips.Count/2)-1;//We go to 1/2 and then 1 lower due to count and indexing beign offset. So at 12 or 13 we go to 6, -1=5 which is the correct index.
        }

        float modifiedInput=Mathf.Abs(inputFraction);
        //TODO: Calculate the fractions per each blip and light them up properly.
        for (int i = 0; i < blipsPerDir; i++)
        {
            if (modifiedInput <= 0) break;
            var col = blips[curHead].color;
            col.a = AlphaUpperBound;
            blips[curHead].color = col;
            if (inputFraction > 0)
            {
                curHead++;
            }
            if (inputFraction < 0)
            {
                curHead--;
            }
            modifiedInput -= blipFraction;
        }
        //Finally, only if input>0.999 or input<-0.999 do we light up the final one on that side
        if (inputFraction > 0.999f)
        {
            var col = blips[blips.Count - 1].color;
            col.a = AlphaUpperBound;
            blips[blips.Count - 1].color = col;
        }
        if (inputFraction < -0.999f)
        {
            var col = blips[0].color;
            col.a = AlphaUpperBound;
            blips[0].color = col;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetMaxVelocities(out float maxVel, out float minVel);
        float vel = GetVelocity();
        float accPercent = GetLastFrameAccPercent();

        float speedFraction = vel / maxVel;
        if (vel < 0)
        {
            speedFraction = vel / minVel;
        }
        SetGaugeFromPercent(SpeedIndicatorBlips, speedFraction);
        SetGaugeFromPercent(AccelerationIndicatorBlips, accPercent);
        SpeedTextUI.text = vel.ToString("F0");
        AccelTextUI.text = $"{accPercent.ToString("F0")}%";
        //TODO: Throttle pin movement
    }
}