using DG.Tweening.Core.Easing;
using System;
using UnityEngine;


public enum StabilizationType
{
    Default, //In this case, we leave the controls alone entirely.
    FAOffDirect,
    FAOffAccumulated,
    FAOnDirect,
    FAOnRotThrottle,
    FAOffRollStab
}

public enum ControlsType
{
    PitchYawOnMouse,
    PitchRollOnMouse,
    KeyboardOnly
}
public class ControlsCarrier : MonoBehaviour
{
    #region Singleton Pattern no.2
    public StabilizationType stabType=StabilizationType.Default;
    public ControlsType controlsType = ControlsType.PitchYawOnMouse;
    private static ControlsCarrier instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static ControlsCarrier Instance()
    {
        if(instance == null)
        {
            var go = new GameObject("Controls Carrier");
            instance = go.AddComponent<ControlsCarrier>();
        }
        return instance;
    }
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetControls(ShipMovementProto movementScript)
    {
        //TODO
    }
}
