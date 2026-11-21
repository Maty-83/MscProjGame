using DG.Tweening.Core.Easing;
using System;
using System.ComponentModel;
using System.Linq;
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
   
    public StabilizationType stabType=StabilizationType.FAOnThrottleForwards;
    public ControlsType controlsType = ControlsType.PitchYawOnMouse;
    public AxisControlType mouseControlType = AxisControlType.Throttle;

    public InputActionAsset BindingsCollection;
    #region Singleton Pattern no.2
    private static ControlsCarrier instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if(BindingsCollection == null){
                BindingsCollection = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/IA_Base.inputactions");//Hard-coded so Singleton pattern works out the box.
            }
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
    
    #region Utility Functions
    public void CycleStab()
    {
        stabType =(StabilizationType)(((int)stabType + 1) % Enum.GetNames(typeof(StabilizationType)).Length);//A smart piece of code that automatically cycles through enum modulo count.      
    }
    /// <summary>
    /// Returns the description of the stabilization type set in the ControlsCarrier instance, excluding mouse controls.
    /// </summary>
    /// <returns>String describing the stabilization type</returns>
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
    /// <summary>
    /// Gets the string-based description of the controlsType of this ControlsCarrier instance
    /// </summary>
    /// <returns>String describing the controls type</returns>
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
    /// <summary>
    /// Returns the shared axis control type mouse controls share for this instance of ControlsCarrier 
    /// </summary>
    /// <returns>String description of the mouseControlType.</returns>
    public string GetMouseControlTypeDesc()
    {
        var fieldInfo = mouseControlType.GetType().GetField(mouseControlType.ToString());
        var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : mouseControlType.ToString();
    }
    /// <summary>
    /// Gets string-based description of the axis control type stored in this ControlsCarrier instance for a given axis
    /// </summary>
    /// <param name="axis">Axis whose control type description we want</param>
    /// <returns>String description of control type.</returns>
    public string GetControlTypeDesc(AxisType axis)
    {
        var type = GetAxisControlType(axis);
        var fieldInfo = type.GetType().GetField(type.ToString());
        var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : type.ToString();
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

    /// <summary>
    /// Get the axis control type for a specific axis for this instance of ControlsCarrier
    /// </summary>
    /// <param name="axis">Enum of axis whose control type we want</param>
    /// <returns>AxisControlType enum which is assigned to the axis currently.</returns>
    public AxisControlType GetAxisControlType(AxisType axis)
    {
        //Everything starts as Direct.
        AxisControlType controlType = AxisControlType.Direct;
        //If we're FA-On, we move to DirectDamped.
        if (stabType == StabilizationType.FAOn || stabType == StabilizationType.FAOnThrottleForwards)
        {
            controlType = AxisControlType.DirectDamped;
        }
        //If it's FA-On with forwards throttle, make throttle a throttle.
        if(stabType==StabilizationType.FAOnThrottleForwards && axis == AxisType.Forward)
        {
            controlType = AxisControlType.Throttle;
        }
        //If we want to stabilize all keyboard rotations...
        if (stabType == StabilizationType.FAOffNonMouseRotStabilized)
        {
            //Stabilize roll if we have pitch and yaw on mouse.
            if (controlsType==ControlsType.PitchYawOnMouse)
            {
                if (axis == AxisType.Roll)
                {
                    controlType = AxisControlType.DirectDamped;
                }
            }
            //Stabilize yaw if we have pitch and roll on mouse.
            if (controlsType==ControlsType.PitchRollOnMouse)
            {
                if(axis == AxisType.Yaw)
                {
                    controlType=AxisControlType.DirectDamped;
                }
            }
            //If we're full-keyboard, stabilize all.
            if (controlsType == ControlsType.KeyboardOnly || controlsType == ControlsType.KerbalKB)
            {
                if (axis == AxisType.Pitch || axis == AxisType.Yaw || axis == AxisType.Roll)
                {
                    controlType = AxisControlType.DirectDamped;
                }

            }
        }
        //If we're pitch+roll on mouse, use the mouse control type for pitch and roll.
        if (controlsType == ControlsType.PitchRollOnMouse)
        {
            if(axis == AxisType.Pitch || axis == AxisType.Roll)
            {
                controlType = mouseControlType;
            }
        }
        //if we're pitch+yaw on mouse, use the mouse control type for pitch and yaw.
        if(controlsType == ControlsType.PitchYawOnMouse)
        {
            if(axis== AxisType.Pitch || axis == AxisType.Yaw)
            {
                controlType = mouseControlType;
            }
        }
        return controlType;
    }
    /// <summary>
    /// Sets controls to a ShipMovementProto script and PlayerInput based on current instance saved parameters
    /// </summary>
    /// <param name="movementScript">ShipMovementProto instance</param>
    /// <param name="input">PlayerInput linked to player controls of the movement script</param>
    public void SetControls(ShipMovementProto movementScript, PlayerInput input)
    {
        input.actions = this.BindingsCollection;
        
        movementScript.thrustControlType = GetAxisControlType(AxisType.Forward);
        movementScript.lateralControlType = GetAxisControlType(AxisType.Lateral);
        movementScript.verticalControlType = GetAxisControlType (AxisType.Vertical);
        movementScript.pitchControlType = GetAxisControlType(AxisType.Pitch);
        movementScript.yawControlType = GetAxisControlType(AxisType.Yaw);
        movementScript.rollControlType = GetAxisControlType(AxisType.Roll);

        switch (controlsType)
        {
            case ControlsType.PitchYawOnMouse:
                input.SwitchCurrentActionMap("Primary (P+Y)M + KB");
                break;
            case ControlsType.PitchRollOnMouse:
                input.SwitchCurrentActionMap("Primary (P+R)M + KB");
                break;
            case ControlsType.KerbalKB:
                input.SwitchCurrentActionMap("Kerbal-like KB");
                break;
            case ControlsType.KeyboardOnly:
                input.SwitchCurrentActionMap("Primary KB-only");
                break;
        }
    }

    public void GetCurrentControlBinds(AxisType axis, out string positive, out string negative)
    {
        var currentMap = GetSelectedActionMap();
        InputAction controlAxis = null;
        switch (axis)
        {
            case AxisType.Forward:
                controlAxis= currentMap.FindAction("Thrust Forward", true);
                break;
            case AxisType.Vertical:
                controlAxis = currentMap.FindAction("Thrust Vertical", true);
                break;
            case AxisType.Lateral:
                controlAxis = currentMap.FindAction("Thrust Lateral", true);
                break;
            case AxisType.Pitch:
                controlAxis = currentMap.FindAction("Rotation Pitch", true);
                break;
            case AxisType.Yaw:
                controlAxis = currentMap.FindAction("Rotation Yaw", true);
                break;
            case AxisType.Roll:
                controlAxis = currentMap.FindAction("Rotation Roll", true);
                break;
        }
        //This I'm unsure about. But I should be able to find it with Linq
        var positiveBind = controlAxis.bindings.FirstOrDefault(b => b.name == "positive");
        var negativeBind = controlAxis.bindings.FirstOrDefault(b => b.name == "negative");
        positive=positiveBind.effectivePath.Split('/')[1];//The string is <Keyboard>/[bind], so split is optimal
        negative=negativeBind.effectivePath.Split("/")[1];
    }
    public InputActionMap GetSelectedActionMap()
    {
        if (BindingsCollection == null)
        {
            BindingsCollection = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/IA_Base.inputactions");//We default to our inputactionasset if BindingsCollection doesn't exist.
        }
        switch (controlsType)
        {
            case ControlsType.PitchYawOnMouse:
                return BindingsCollection.FindActionMap("Primary (P+Y)M + KB", true);
            case ControlsType.PitchRollOnMouse:
                return BindingsCollection.FindActionMap("Primary (P+R)M + KB", true);
            case ControlsType.KeyboardOnly:
                return BindingsCollection.FindActionMap("Primary KB-only", true);
            case ControlsType.KerbalKB:
                return BindingsCollection.FindActionMap("Kerbal-like KB", true);
        }
        return null;
    }
}
