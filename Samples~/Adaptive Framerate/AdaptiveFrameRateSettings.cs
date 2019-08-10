using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.UI;

public class AdaptiveFrameRateSettings : MonoBehaviour
{
    public Slider VRRSettingsMin;
    public Text MinText;
    public Slider VRRSettingsMax;
    public Text MaxText;
    public Toggle VRREnabled;
    AdaptiveVariableRefreshRate AdaptiveVRRO;

    void Awake()
    {
        AdaptiveVRRO = GameObject.FindObjectOfType<AdaptiveVariableRefreshRate>();
        if (AdaptiveVRRO)
            VRREnabled.isOn = AdaptiveVRRO.Enabled;

        VRRSettingsMin.value = AdaptiveVRRO.MinBound;
        VRRSettingsMax.value = AdaptiveVRRO.MaxBound;
    }

    public void ToggleVRR()
    {
        if (AdaptiveVRRO)
            AdaptiveVRRO.Enabled = VRREnabled.isOn;
    }

    void Update()
    {
        AdaptiveVRRO.MinBound = VRRSettingsMin.value;
        AdaptiveVRRO.MaxBound = VRRSettingsMax.value;
        MinText.text = $"Min FPS - {VRRSettingsMin.value}";
        MaxText.text = $"Max FPS - {VRRSettingsMax.value}";
    }
}
