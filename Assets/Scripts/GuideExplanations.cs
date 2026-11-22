using System;
using UnityEngine;
using TMPro;

public class GuideExplanations : MonoBehaviour
{
    public TMP_Text throttleText;
    public TMP_Text lateralText;
    public TMP_Text verticalText;

    public TMP_Text pitchText;
    public TMP_Text yawText;
    public TMP_Text rollText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        throttleText.text = GetThrottleBindsExplanation();
        lateralText.text = GetLateralBindsExplanation();
        verticalText.text = GetVerticalBindsExplanation();
        pitchText.text = GetPitchBindsExplanation();
        yawText.text = GetYawBindsExplanation();
        rollText.text = GetRollBindsExplanation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetThrottleBindsExplanation()
    {
        ControlsCarrier.Instance().GetCurrentControlBinds(AxisType.Forward,out string forwardBind, out string reverseBind);
        string controlType=ControlsCarrier.Instance().GetControlTypeDesc(AxisType.Forward);
        return $"<color=orange>The throttle is how you accelerate forwards and backwards (Where you're pointing).\n\nCurrent bindings: <i>Forward:</i></color> {forwardBind.ToUpper()}<color=orange> Reverse:</color> {reverseBind.ToUpper()} <color=orange>Control Type:</color> {controlType}";
    }


    public string GetLateralBindsExplanation()
    {
        ControlsCarrier.Instance().GetCurrentControlBinds(AxisType.Lateral,out string rightBind, out string leftBind);
        string controlType = ControlsCarrier.Instance().GetControlTypeDesc(AxisType.Lateral);
        return $"<color=orange>Lateral movement is strafing left or right.\n\nCurrent bindings: Left:</color> {leftBind.ToUpper()}<color=orange> Right:</color> {rightBind.ToUpper()} <color=orange>Control Type:</color> {controlType}";
    }

    public string GetVerticalBindsExplanation()
    {
        ControlsCarrier.Instance().GetCurrentControlBinds(AxisType.Vertical,out string upBind, out string downBind);
        string controlType = ControlsCarrier.Instance().GetControlTypeDesc(AxisType.Vertical);
        return $"<color=orange>Vertical movement is going up and down.\n\nCurrent bindings: Up:</color> {upBind.ToUpper()}<color=orange> Down:</color> {downBind.ToUpper()} <color=orange>Control Type:</color> {controlType}";
    }

    public string GetRollBindsExplanation()
    {
        string controlType = ControlsCarrier.Instance().GetControlTypeDesc(AxisType.Roll);
        if (ControlsCarrier.Instance().controlsType==ControlsType.PitchRollOnMouse)
        {
            return $"<color=orange>Roll is rotating your ship arounds its forward axis.\nLeft roll pushes your left side down, right roll the right side.\nCurrent bindings: Left:</color> Mouse Left <color=orange>Right:</color> Mouse Right <color=orange>Control Type:</color> {controlType}";

        }
        ControlsCarrier.Instance().GetCurrentControlBinds(AxisType.Roll,out string rightBind, out string leftBind);
        return $"<color=orange>Roll is rotating your ship arounds its forward axis.\nLeft roll pushes your left side down, right roll the right side.\nCurrent bindings: Left:</color> {leftBind.ToUpper()}<color=orange> Right:</color> {rightBind.ToUpper()} <color=orange>Control Type:</color> {controlType}";
    }

    public string GetYawBindsExplanation()
    {
        string controlType = ControlsCarrier.Instance().GetControlTypeDesc(AxisType.Yaw);
        if (ControlsCarrier.Instance().controlsType == ControlsType.PitchYawOnMouse)
        {
            return $"<color=orange>Yaw is rotating your ship either left or right, like steering a car.\nIt is noticeably slower than either pitch or roll.\nCurrent bindings:</color> Left: Mouse Left<color=orange> Right:</color> Mouse Right <color=orange>Control Type:</color> {controlType}";
        }
        ControlsCarrier.Instance().GetCurrentControlBinds(AxisType.Yaw,out string leftBind, out string rightBind);
        return $"<color=orange>Yaw is rotating your ship either left or right, like steering a car.\nIt is noticeably slower than either pitch or roll.\nCurrent bindings: Left:</color> {leftBind.ToUpper()}<color=orange> Right:</color> {rightBind.ToUpper()} <color=orange>Control Type:</color> {controlType}";
    }

    public string GetPitchBindsExplanation()
    {
        string controlType = ControlsCarrier.Instance().GetControlTypeDesc(AxisType.Pitch);
        if (ControlsCarrier.Instance().controlsType == ControlsType.PitchYawOnMouse || ControlsCarrier.Instance().controlsType == ControlsType.PitchRollOnMouse) 
        { 
            return $"<color=orange>Pitch is the motion of your ship's nose going up or down. It's generally faster than yawing.\n\nCurrent bindings: Up:</color> Mouse Forward<color=orange> Down:</color> Mouse Down <color=orange>Control Type:</color> {controlType}";
        }
        ControlsCarrier.Instance().GetCurrentControlBinds(AxisType.Pitch,out string upBind,out string downBind);
        return $"<color=orange>Pitch is the motion of your ship's nose going up or down. It's generally faster than yawing.\n\nCurrent bindings: Up:</color> {upBind.ToUpper()}<color=orange> Down:</color> {downBind.ToUpper()} <color=orange>Control Type:</color> {controlType}";
    }

}
