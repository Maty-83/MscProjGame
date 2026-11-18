using UnityEngine;

public class MouseDirIndicator : MonoBehaviour
{
    public GameObject OffsetTarget;
    public GameObject RotationTarget;
    public Material TransparencyTarget;
    public ShipMovementProto ShipMovementScript;
    public float xOffsetMult = 0.15f;
    public float yOffsetMult = 0.1f;
    public float OffsetFracToMaxIntensity=0.3f;
    public float MaxIntensity = 0;
    public float MinIntensity = -5;
    public Color EmissionColorBase = new Color(0, 191f / 255f, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetDirIndicator();
    }

    public void SetDirIndicator()
    {
        //It's meant to be mouse-only, exclude KB-only first. For stability with addon schemes, use if's that say "We don't equal either" so the default is doing nothing.
        if (ControlsCarrier.Instance().controlsType != ControlsType.PitchYawOnMouse && ControlsCarrier.Instance().controlsType != ControlsType.PitchRollOnMouse) return;

        float yAxis = ShipMovementScript.LastFrameRotInputPercent.z;
        float xAxis = 0f;//Varies based on the controls type instance, can be roll or yaw.

        if (ControlsCarrier.Instance().controlsType == ControlsType.PitchYawOnMouse)
        {
            xAxis = ShipMovementScript.LastFrameRotInputPercent.y;
        }
        if (ControlsCarrier.Instance().controlsType == ControlsType.PitchRollOnMouse)
        {
            xAxis = ShipMovementScript.LastFrameRotInputPercent.x;
        }

        //TODO.
        SetOffset(xAxis, yAxis);
        SetAngleFromOffsets(xAxis, yAxis);

    }
    public void SetOffset(float xOffset, float yOffset)
    {
        OffsetTarget.transform.position += new Vector3(xOffset * xOffsetMult, yOffset * yOffsetMult, 0);
    }
    public void SetAngleFromOffsets(float xOffset, float yOffset)
    {
        //TODO
    }
    public void SetTransparencyFromOffsets(float xOffset, float yOffset)
    {
        //If we move
        var offFrac = Mathf.Sqrt(xOffset * xOffset + yOffset * yOffset);

        var intensity=Mathf.Clamp((MinIntensity+ offFrac*((MaxIntensity-MinIntensity)/OffsetFracToMaxIntensity)), MinIntensity, MaxIntensity);
        TransparencyTarget.SetColor("_EmissiveColor", EmissionColorBase * Mathf.Pow(2, intensity));//This I'm not sure about, it depends on the render pipelines.
    }
    public float AngleFromOffset(float offsetX, float offsetY)
    {
        float angleRad = Mathf.Atan2(offsetY, offsetX);

        // Convert to degrees: -180 to 180
        float angleDeg = angleRad * (180.0f / Mathf.PI);
        return angleDeg;
    }
}
