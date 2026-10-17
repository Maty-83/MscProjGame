using DG.Tweening.Core.Easing;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;


public enum StabilizationType
{
    [Description("FA-Off")]
    FAOff,
    [Description("FA-On, including forward")]
    FAOn,
    [Description("FA-On, forward axis as throttle")]
    FAOnThrottleForwards,
    [Description("FA-Off, rotation axis on KB Stabilized")]
    FAOffNonMouseRotStabilized,
}

public enum ControlsType
{
    [Description("KB+M (P+Y)")]
    PitchYawOnMouse,
    [Description("KB+M (P+R)")]
    PitchRollOnMouse,
    [Description("KB-Only")]
    KeyboardOnly
}
public class ControlsCarrier : MonoBehaviour
{
   
    public StabilizationType stabType=StabilizationType.FAOff;
    public ControlsType controlsType = ControlsType.PitchYawOnMouse;
    public AxisControlType mouseControlType = AxisControlType.Direct;
    #region Singleton Pattern no.2
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
    
    #region Button Utilities
    public void CycleStab()
    {
        stabType =(StabilizationType)(((int)stabType + 1) % Enum.GetNames(typeof(StabilizationType)).Length);//A smart piece of code that automatically cycles through enum modulo count.      
    }
    public string GetStabTypeDesc()
    {
        var fieldInfo=stabType.GetType().GetField(stabType.ToString());
        var attributes= (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : stabType.ToString();
    }
    
    public void CycleControls()
    {
        controlsType =(ControlsType)(((int)controlsType + 1)% Enum.GetNames (typeof(ControlsType)).Length);
    }
    public string GetControlsTypeDesc()
    {
        var fieldInfo = controlsType.GetType().GetField(controlsType.ToString());
        var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : controlsType.ToString();
    }
    public void CycleMouseControl()
    {
        mouseControlType= (AxisControlType)(((int)mouseControlType + 1) % Enum.GetNames(typeof(AxisControlType)).Length);
    }
    public string GetMouseControlTypeStringDesc()
    {
        var fieldInfo = mouseControlType.GetType().GetField(mouseControlType.ToString());
        var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : mouseControlType.ToString();
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

    public void SetControls(ShipMovementProto movementScript, PlayerInput input)
    {


        //TODO
        switch (stabType)
        {
            case StabilizationType.FAOff:
            case StabilizationType.FAOffNonMouseRotStabilized:
                //Both of these use the same setup, the non-mouse rotations are handled in controlsType switch.

                //FAOff for all keyboard axes is direct input, undamped.

                break;
            case StabilizationType.FAOn:
                //FA-On for all axes is direct damped.
                break;
            case StabilizationType.FAOnThrottleForwards:
                //Same as FA-On, but forwards is a throttle.
                break;            
        }

        //Set the mouse controls for everything, we'll adjust later.
        movementScript.pitchControlType = mouseControlType;
        movementScript.yawControlType = mouseControlType;
        movementScript.rollControlType = mouseControlType;

        switch (controlsType)
        {
            case ControlsType.PitchYawOnMouse:

                //TODO:Assign the correct IA_Base input

                if (stabType == StabilizationType.FAOffNonMouseRotStabilized)
                {
                    movementScript.rollControlType = AxisControlType.DirectDamped;
                }
                break;
            case ControlsType.PitchRollOnMouse:

                //TODO:Assign the correct IA_Base input

                if (stabType == StabilizationType.FAOffNonMouseRotStabilized)
                {
                    movementScript.yawControlType = AxisControlType.DirectDamped;

                }
                break;
            case ControlsType.KeyboardOnly:

                //TODO: Assign the correct IA_Base input

                if (stabType == StabilizationType.FAOffNonMouseRotStabilized)
                {
                    movementScript.pitchControlType = AxisControlType.DirectDamped;
                    movementScript.yawControlType = AxisControlType.DirectDamped;
                    movementScript.rollControlType = AxisControlType.DirectDamped;
                }
                break;
        }
    }
}
