using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AdaptivePerformance.Samsung.Android;

public class VariableRefreshRateControl : MonoBehaviour
{
    public Dropdown supportedRefreshRates;
    public Text currentRefreshRate;
    public Slider targetRefreshRate;
    public Text targetRefreshRateLabel;
    public GameObject notSupportedPanel;
    // How long to run the test (in seconds)
    public float timeOut = 80;

    bool didVRRSupportChange = false;
    float timeOuttimer = 0;

    void Start()
    {
        timeOuttimer = timeOut;

        Application.targetFrameRate = 60;
        targetRefreshRate.SetValueWithoutNotify(60);

        if (VariableRefreshRate.Instance == null)
        {
            Debug.Log("[AP VRR] Variable Refresh Rate is not supported on this device.");
            notSupportedPanel.SetActive(true);
            return;
        }

        VariableRefreshRate.Instance.RefreshRateChanged += UpdateDropdown;
        supportedRefreshRates.onValueChanged.AddListener(delegate {
            if (!VariableRefreshRate.Instance.SetRefreshRateByIndex(supportedRefreshRates.value))
                UpdateDropdown();
        });

        targetRefreshRate.onValueChanged.AddListener(delegate {
            Application.targetFrameRate = (int)targetRefreshRate.value;
        });
        UpdateDropdown();
    }

    void Update()
    {
        notSupportedPanel.SetActive(VariableRefreshRate.Instance == null);

        targetRefreshRateLabel.text = $"Target Refresh Rate: {Application.targetFrameRate} Hz";

        timeOuttimer -= Time.deltaTime;

        if (timeOuttimer < 0)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        if (VariableRefreshRate.Instance == null)
        {
            UpdateDropdown();
            didVRRSupportChange = true;
            currentRefreshRate.text = $"Current refresh rate: - Hz";
            return;
        }
        else
        {
            if (didVRRSupportChange)
            {
                didVRRSupportChange = false;
                UpdateDropdown();
            }
        }
        currentRefreshRate.text = $"Current refresh rate: {VariableRefreshRate.Instance.CurrentRefreshRate} Hz";
    }

    void UpdateDropdown()
    {
        var options = new List<Dropdown.OptionData>();
        supportedRefreshRates.ClearOptions();

        if (VariableRefreshRate.Instance == null)
            return;

        var index = -1;
        for (var i = 0; i < VariableRefreshRate.Instance.SupportedRefreshRates.Length; ++i)
        {
            var rr = VariableRefreshRate.Instance.SupportedRefreshRates[i];
            options.Add(new Dropdown.OptionData(rr.ToString()));
            if (rr == VariableRefreshRate.Instance.CurrentRefreshRate)
                index = i;
        }

        supportedRefreshRates.AddOptions(options);
        supportedRefreshRates.SetValueWithoutNotify(index);
    }
}
