using UnityEngine;
using TMPro;

public enum SettingType
{
    Controls,
    MouseControlType,
    Stabilization,
    Preset
}
public class MainMenuButtonControls : MonoBehaviour
{
    public SettingType SettingType = SettingType.Controls;
    public TMP_Text TextToChange;
    private StabilizationType CachedStabType = StabilizationType.FAOnThrottleForwards;
    private AxisControlType CachedMouseControlType = AxisControlType.Throttle;
    private ControlsType CachedControlsType = ControlsType.PitchYawOnMouse;
    public ControlsPreset CachedPresetType = ControlsPreset.ElitePitchAndYawFA;
    public bool cycleDown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switch (SettingType)
        {
            case SettingType.Controls:
                TextToChange.text = "CONTROLS:\n" + ControlsCarrier.Instance().GetControlsTypeDesc();
                CachedControlsType = ControlsCarrier.Instance().controlsType;
                break;
            case SettingType.MouseControlType:
                TextToChange.text = "MOUSE CONTROLS:\n" + ControlsCarrier.Instance().GetMouseControlTypeDesc();
                CachedMouseControlType= ControlsCarrier.Instance().mouseControlType;
                break;
            case SettingType.Stabilization:
                TextToChange.text = "STABILIZATION:\n" + ControlsCarrier.Instance().GetStabTypeDesc();
                CachedStabType = ControlsCarrier.Instance().stabType;
                break;
            case SettingType.Preset:
                TextToChange.text = "PRESET:\n" + ControlsCarrier.Instance().GetPresetTypeDesc();
                CachedPresetType = ControlsCarrier.Instance().controlsPreset;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (SettingType)
        {
            case SettingType.Controls:
                if (CachedControlsType != ControlsCarrier.Instance().controlsType)
                {
                    CachedControlsType= ControlsCarrier.Instance().controlsType;
                    TextToChange.text = "CONTROLS:\n" + ControlsCarrier.Instance().GetControlsTypeDesc();
                }
                break;
            case SettingType.MouseControlType:
                if (CachedMouseControlType != ControlsCarrier.Instance().mouseControlType)
                {
                    CachedMouseControlType = ControlsCarrier.Instance().mouseControlType;
                    TextToChange.text = "MOUSE CONTROLS:\n" + ControlsCarrier.Instance().GetMouseControlTypeDesc();
                }
                break;
            case SettingType.Stabilization:
                if (CachedStabType != ControlsCarrier.Instance().stabType)
                {
                    CachedStabType= ControlsCarrier.Instance().stabType;
                    TextToChange.text = "STABILIZATION:\n" + ControlsCarrier.Instance().GetStabTypeDesc();
                }
                break;
            case SettingType.Preset:
                if (CachedPresetType != ControlsCarrier.Instance().controlsPreset)
                {
                    TextToChange.text = "PRESET:\n" + ControlsCarrier.Instance().GetPresetTypeDesc();
                    CachedPresetType = ControlsCarrier.Instance().controlsPreset;
                }
                break;
        }
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
            case SettingType.Preset:
                CyclePreset();
                break;
        }
    }
    public void CycleStab()
    {
        ControlsCarrier.Instance().CycleStab(cycleDown);
    }
    public void CycleMouseControls()
    {
        ControlsCarrier.Instance().CycleMouseControl(cycleDown);
    }
    public void CycleControls()
    {
        ControlsCarrier.Instance().CycleControls(cycleDown);
    }
    public void CyclePreset()
    {
        ControlsCarrier.Instance().CyclePreset(cycleDown);
    }
}
