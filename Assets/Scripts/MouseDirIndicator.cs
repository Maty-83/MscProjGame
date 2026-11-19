using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class MouseDirIndicator : MonoBehaviour
{
    public GameObject OffsetTarget;
    public GameObject RollTarget;
    public GameObject PitchTarget;
    public Renderer TransparencyTargetRenderer;
    private Material TransparencyTarget;
    public ShipMovementProto ShipMovementScript;
    public float xOffsetMult = 0.15f;
    public float yOffsetMult = 0.1f;
    public float OffsetFracToMaxIntensity=0.3f;
    public float MaxIntensity = 0;
    public float MinIntensity = -5;
    public float PitchDegAtMaxOffset = 100f;
    public Color EmissionColorBase = new Color(0, 191f / 255f, 0);
    private Vector3 basePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        basePos=transform.localPosition;
        TransparencyTarget=TransparencyTargetRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        SetDirIndicator();
    }

    public void SetDirIndicator()
    {
        //It's meant to be mouse-only, exclude KB-only first. For stability with addon schemes, use if's that say "We don't equal either" so the default is doing nothing.
        if (ControlsCarrier.Instance().ControlsType != ControlsType.PitchYawOnMouse && ControlsCarrier.Instance().ControlsType != ControlsType.PitchRollOnMouse) return;

        float xAxis = 0f;
        float yAxis = 0f;

        //This is really very good at showing throttles, but sucks for anything else.
        if (ControlsCarrier.Instance().MouseControlType == AxisControlType.Throttle)
        {
            yAxis = ShipMovementScript.LastFrameRotInputPercent.z;
            //Varies based on the controls type instance, can be roll or yaw.

            if (ControlsCarrier.Instance().ControlsType == ControlsType.PitchYawOnMouse)
            {
                xAxis = ShipMovementScript.LastFrameRotInputPercent.y;
            }
            if (ControlsCarrier.Instance().ControlsType == ControlsType.PitchRollOnMouse)
            {
                xAxis = ShipMovementScript.LastFrameRotInputPercent.x;
            }
        }
        else
        {
            //TODO: We'll need to show the % of accumulated input compared to max accumulation.

            yAxis = ShipMovementScript.pitchOverFrames / ShipMovementScript.maxAccumulation;
            //Varies based on the controls type instance, can be roll or yaw.

            if (ControlsCarrier.Instance().ControlsType == ControlsType.PitchYawOnMouse)
            {
                xAxis =ShipMovementScript.yawOverFrames / ShipMovementScript.maxAccumulation;// ShipMovementScript.LastFrameRotInputPercent.y;
            }
            if (ControlsCarrier.Instance().ControlsType == ControlsType.PitchRollOnMouse)
            {
                xAxis =ShipMovementScript.rollOverFrames / ShipMovementScript.maxAccumulation; //ShipMovementScript.LastFrameRotInputPercent.x;
            }
        }
        SetOffset(xAxis, yAxis);
        SetAngleFromOffsets(xAxis, yAxis);
        SetTransparencyFromOffsets(xAxis, yAxis);
    }
    public void SetOffset(float xOffset, float yOffset)
    {
        OffsetTarget.transform.localPosition = basePos + new Vector3(xOffset * xOffsetMult, yOffset * yOffsetMult, 0);
    }
    public void SetAngleFromOffsets(float xOffset, float yOffset)
    {
        //Pitch target sets linear interpolated pitch
        //Angle to the side max is about -110. It's based on offset magnitude  (If mag >1, we clamp)
        //I think it's X axis?
        float rot= -PitchDegAtMaxOffset*Mathf.Clamp(Mathf.Sqrt(xOffset * xOffset + yOffset * yOffset),0,1);
        PitchTarget.transform.localEulerAngles=new Vector3(0,rot,0);

        //Roll Target only rolls Z axis based on AngleFromOffset function.
        RollTarget.transform.localEulerAngles=new Vector3(0,0, AngleFromOffset(xOffset, yOffset));
    }
    public void SetTransparencyFromOffsets(float xOffset, float yOffset)
    {
        //If we move
        var offFrac = Mathf.Sqrt(xOffset * xOffset + yOffset * yOffset);
        
        var intensity=Mathf.Clamp((MinIntensity+ offFrac*((MaxIntensity-MinIntensity)/OffsetFracToMaxIntensity)), MinIntensity, MaxIntensity);
        TransparencyTarget.EnableKeyword("_EMISSION");
        TransparencyTarget.SetColor("_EmissionColor", EmissionColorBase * Mathf.Pow(2, intensity));//This I'm not sure about, it depends on the render pipelines.
        RendererExtensions.UpdateGIMaterials(TransparencyTargetRenderer);
    }
    public float AngleFromOffset(float xOffset, float yOffset)
    {
        float angleRad = Mathf.Atan2(yOffset, xOffset);

        // Convert to degrees: -180 to 180
        float angleDeg = angleRad * (180.0f / Mathf.PI);
        return angleDeg;
    }
}
