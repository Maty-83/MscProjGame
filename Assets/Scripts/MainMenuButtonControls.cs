using UnityEngine;
using TMPro;

public enum SettingType
{
    Controls,
    MouseControlType,
    Stabilization
}
public class MainMenuButtonControls : MonoBehaviour
{
    public SettingType SettingType = SettingType.Controls;
    public TMP_Text TextToChange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switch (SettingType)
        {
            case SettingType.Controls:
                TextToChange.text = "CONTROLS:\n" + ControlsCarrier.Instance().GetControlsTypeDesc();
                break;
            case SettingType.MouseControlType:
                TextToChange.text = "MOUSE CONTROLS:\n" + ControlsCarrier.Instance().GetMouseControlTypeDesc();
                break;
            case SettingType.Stabilization:
                TextToChange.text = "STABILIZATION:\n" + ControlsCarrier.Instance().GetStabTypeDesc();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnButtonPressed()
    {
        switch (SettingType)
        {
            case SettingType.Controls:
                CycleControls();
                break;
            case SettingType.MouseControlType:
                CycleMouseControls();
                break;
            case SettingType.Stabilization:
                CycleStab();
                break;
        }
    }
    public void CycleStab()
    {
        ControlsCarrier.Instance().CycleStab();
        TextToChange.text="STABILIZATION:\n"+ControlsCarrier.Instance().GetStabTypeDesc();
    }
    public void CycleMouseControls()
    {
        ControlsCarrier.Instance().CycleMouseControl();
        TextToChange.text="MOUSE CONTROLS:\n"+ControlsCarrier.Instance().GetMouseControlTypeDesc();
    }
    public void CycleControls()
    {
        ControlsCarrier.Instance().CycleControls();
        TextToChange.text="CONTROLS:\n"+ControlsCarrier.Instance().GetControlsTypeDesc();
    }
}
