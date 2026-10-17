using DG.Tweening.Core.Easing;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


public enum StabilizationType
{
    FAOff=0,
    FAOn=1,
    FAOffNonMouseRotStabilized=2
}

public enum ControlsType
{
    PitchYawOnMouse=0,
    PitchRollOnMouse=1,
    KeyboardOnly=2
}
public class ControlsCarrier : MonoBehaviour
{
    #region Singleton Pattern no.2
    public StabilizationType stabType=StabilizationType.FAOff;
    public ControlsType controlsType = ControlsType.PitchYawOnMouse;
    public AxisControlType mouseControlType = AxisControlType.Direct;
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

    public void CycleStab(TMP_Text txt = null)
    {
        int stabTemp = (int)stabType;
        stabType =(StabilizationType)((stabTemp + 1) % Enum.GetNames(typeof(StabilizationType)).Length);//A smart piece of code that automatically cycles through enum modulo count.
        if (txt != null)
        {
            txt.text = GetStabTypeDesc();
        }
        
    }
    public string GetStabTypeDesc()
    {
        switch (stabType)
        {

        }
    }
    
    public void CycleControls(TMP_Text txt = null)
    {
        int controlsTemp = (int)controlsType;
        controlsType =(ControlsType)((controlsTemp + 1)% Enum.GetNames (typeof(ControlsType)).Length);
        if (txt != null)
        {
            txt.text=GetControlsTypeDesc();
        }
    }
    public string GetControlsTypeDesc()
    {
        switch (controlsType)
        {

        }
    }
    public void CycleMouseControl(TMP_Text txt = null)
    {
        int mouseControlTemp = (int)mouseControlType;
        mouseControlType= (AxisControlType)((mouseControlTemp+1) % Enum.GetNames(typeof(AxisControlType)).Length);
        if (txt != null)
        {
            txt.text = GetMouseControlTypeStringDesc();
        }
    }
    public string GetMouseControlTypeStringDesc()
    {
        switch (mouseControlType)
        {

        }
    }

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
    }
}
