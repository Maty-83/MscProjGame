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
    [Description("FA-On, Including Forward")]
    FAOn,
    [Description("FA-On, Forward Axis as Throttle")]
    FAOnThrottleForwards,
    [Description("FA-Off, Rotation on KB Stabilized")]
    FAOffNonMouseRotStabilized,
}

public enum ControlsType
{
    [Description("Keyboard+Mouse Pitch & Yaw")]
    PitchYawOnMouse,
    [Description("Keyboard+Mouse Pitch & Roll")]
    PitchRollOnMouse,
    [Description("Keyboard-Only")]
    KeyboardOnly,
    [Description("Keyboard-Only, KSP-Like")]
    KerbalKB
}
public class ControlsCarrier : MonoBehaviour
{
   
    public StabilizationType StabType=StabilizationType.FAOff;
    public ControlsType ControlsType = ControlsType.PitchYawOnMouse;
    public AxisControlType MouseControlType = AxisControlType.Direct;
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
        StabType =(StabilizationType)(((int)StabType + 1) % Enum.GetNames(typeof(StabilizationType)).Length);//A smart piece of code that automatically cycles through enum modulo count.      
    }
    public string GetStabTypeDesc()
    {
        var fieldInfo=StabType.GetType().GetField(StabType.ToString());
        var attributes= (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : StabType.ToString();
    }
    
    public void CycleControls()
    {
        ControlsType =(ControlsType)(((int)ControlsType + 1)% Enum.GetNames (typeof(ControlsType)).Length);
    }
    public string GetControlsTypeDesc()
    {
        var fieldInfo = ControlsType.GetType().GetField(ControlsType.ToString());
        var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : ControlsType.ToString();
    }
    public void CycleMouseControl()
    {
        MouseControlType= (AxisControlType)(((int)MouseControlType + 1) % Enum.GetNames(typeof(AxisControlType)).Length);
    }
    public string GetMouseControlTypeDesc()
    {
        var fieldInfo = MouseControlType.GetType().GetField(MouseControlType.ToString());
        var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : MouseControlType.ToString();
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
        switch (StabType)
        {
            case StabilizationType.FAOff:
            case StabilizationType.FAOffNonMouseRotStabilized:
                //Both of these use the same setup, the non-mouse rotations are handled in controlsType switch.

                //FAOff for all keyboard axes is direct input, undamped.
                movementScript.thrustControlType = AxisControlType.Direct;
                movementScript.lateralControlType=AxisControlType.Direct;
                movementScript.verticalControlType=AxisControlType.Direct;
                movementScript.pitchControlType = AxisControlType.Direct;
                movementScript.yawControlType = AxisControlType.Direct;
                movementScript.rollControlType = AxisControlType.Direct;
                break;
            case StabilizationType.FAOn:
                movementScript.thrustControlType = AxisControlType.DirectDamped;
                movementScript.lateralControlType = AxisControlType.DirectDamped;
                movementScript.verticalControlType = AxisControlType.DirectDamped;
                movementScript.pitchControlType = AxisControlType.DirectDamped;
                movementScript.yawControlType = AxisControlType.DirectDamped;
                movementScript.rollControlType = AxisControlType.DirectDamped;
                break;
            case StabilizationType.FAOnThrottleForwards:
                //Same as FA-On, but forwards is a throttle.
                movementScript.thrustControlType = AxisControlType.Throttle;
                movementScript.lateralControlType = AxisControlType.DirectDamped;
                movementScript.verticalControlType = AxisControlType.DirectDamped;
                movementScript.pitchControlType = AxisControlType.DirectDamped;
                movementScript.yawControlType = AxisControlType.DirectDamped;
                movementScript.rollControlType = AxisControlType.DirectDamped;
                break;            
        }

        switch (ControlsType)
        {
            case ControlsType.PitchYawOnMouse:
                input.SwitchCurrentActionMap("Primary (P+Y)M + KB");

                movementScript.pitchControlType = MouseControlType;
                movementScript.yawControlType = MouseControlType;

                if (StabType == StabilizationType.FAOffNonMouseRotStabilized)
                {
                    movementScript.rollControlType = AxisControlType.DirectDamped;
                }
                break;
            case ControlsType.PitchRollOnMouse:
                input.SwitchCurrentActionMap("Primary (P+R)M + KB");

                movementScript.pitchControlType = MouseControlType;
                movementScript.rollControlType = MouseControlType;

                if (StabType == StabilizationType.FAOffNonMouseRotStabilized)
                {
                    movementScript.yawControlType = AxisControlType.DirectDamped;
                }
                break;
            case ControlsType.KerbalKB:
                input.SwitchCurrentActionMap("Kerbal-like KB");
                if (StabType == StabilizationType.FAOffNonMouseRotStabilized)
                {
                    movementScript.pitchControlType = AxisControlType.DirectDamped;
                    movementScript.yawControlType = AxisControlType.DirectDamped;
                    movementScript.rollControlType = AxisControlType.DirectDamped;
                }
                break;
            case ControlsType.KeyboardOnly:

                input.SwitchCurrentActionMap("Primary KB-only");

                if (StabType == StabilizationType.FAOffNonMouseRotStabilized)
                {
                    movementScript.pitchControlType = AxisControlType.DirectDamped;
                    movementScript.yawControlType = AxisControlType.DirectDamped;
                    movementScript.rollControlType = AxisControlType.DirectDamped;
                }
                break;
        }
    }
}
