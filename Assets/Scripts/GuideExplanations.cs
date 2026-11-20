using System;
using UnityEngine;

public class GuideExplanations : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetThrottleBindsExplanation()
    {
        ControlsCarrier.Instance().GetCurrentControlBinds(AxisType.Forward,out string forwardBind, out string reverseBind);
        return $"The throttle is how you accelerate forwards and backwards (Where you're pointing).\nCurrent bindings: Forward: {forwardBind}, Reverse: {reverseBind}";
    }


    public string GetLateralBindsExplanation()
    {
        ControlsCarrier.Instance().GetCurrentControlBinds(AxisType.Lateral,out string rightBind, out string leftBind);
        return $"Lateral movement is strafing left or right.\nCurrent bindings: Left: {leftBind}, Right: {rightBind}";
    }

    public string GetVerticalBindsExplanation()
    {
        ControlsCarrier.Instance().GetCurrentControlBinds(AxisType.Vertical,out string upBind, out string downBind);
        return $"Vertical movement is going up and down.\nCurrent bindings: Up: {upBind}, Down: {downBind}";
    }

    public string GetRollBindsExplanation()
    {
        if (ControlsCarrier.Instance().ControlsType==ControlsType.PitchRollOnMouse)
        {
            return $"Roll is rotating your ship arounds its forward axis.\nLeft roll pushes your left side down, right roll the right side.\nIt is bound to your mouse movement sideways.";

        }
        ControlsCarrier.Instance().GetCurrentControlBinds(AxisType.Roll,out string rightBind, out string leftBind);
        return $"Roll is rotating your ship arounds its forward axis.\nLeft roll pushes your left side down, right roll the right side.\nCurrent bindings: Left: {leftBind}, Right: {rightBind}";
    }

    public string GetYawBindsExplanation()
    {
        if (ControlsCarrier.Instance().ControlsType == ControlsType.PitchYawOnMouse)
        {
            return $"Yaw is rotating your ship either left or right, like steering a car.\nIt is noticeably slower than either pitch or roll, meaning for larger manoeuvres, rolling and pitching may be better.\nIt is bound to your sideways mouse movement.";
        }
        ControlsCarrier.Instance().GetCurrentControlBinds(AxisType.Yaw,out string leftBind, out string rightBind);
        return $"Yaw is rotating your ship either left or right, like steering a car.\nIt is noticeably slower than either pitch or roll, meaning for larger manoeuvres, rolling and pitching may be better.\nCurrent bindings: Left: {leftBind}, Right: {rightBind}";
    }

    public string GetPitchBindsExplanation()
    {
        if (ControlsCarrier.Instance().ControlsType == ControlsType.PitchYawOnMouse || ControlsCarrier.Instance().ControlsType == ControlsType.PitchRollOnMouse) 
        { 
            return $"Pitch is the motion of your ship's nose going up or down. It's generally faster than yawing.\nIt is bound to your mouse moving up and down.";
        }
        ControlsCarrier.Instance().GetCurrentControlBinds(AxisType.Pitch,out string upBind,out string downBind);
        return $"Pitch is the motion of your ship's nose going up or down. It's generally faster than yawing.\nCurrent bindings: Up: {upBind} Down: {downBind}.";
    }

}
