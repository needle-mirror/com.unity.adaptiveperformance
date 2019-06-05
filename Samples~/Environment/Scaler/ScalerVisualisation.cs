using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AdaptivePerformance;

public class ScalerVisualisation : MonoBehaviour
{
    public Text Name;
    public Slider Level;
    public Toggle Override;
    public AdaptivePerformanceScaler Scaler;

    public void SetOverride()
    {
        Scaler.OverrideLevel = Override.isOn ? Scaler.CurrentLevel : -1;
        Level.interactable = Override.isOn;
    }

    public void SetLevel(float value)
    {
        var level = (int)(value * Scaler.MaxLevel);
        if (Scaler.OverrideLevel != -1)
            Scaler.OverrideLevel = level;
        Level.value = (float)level / Scaler.MaxLevel;
    }

    private void Start()
    {
        Name.text = Scaler.GetType().Name;
        Level.value = (float)Scaler.CurrentLevel / Scaler.MaxLevel;
        Override.isOn = Scaler.OverrideLevel != -1;
    }

    private void Update()
    {
        Level.interactable = Scaler.OverrideLevel != -1;
        if (Scaler.OverrideLevel == -1)
            Level.value = (float)Scaler.CurrentLevel / Scaler.MaxLevel;
    }
}
