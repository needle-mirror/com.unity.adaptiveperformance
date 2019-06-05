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

    IVariableRefreshRate m_VRR;
    float timeOuttimer = 0;

    void Start()
    {
        m_VRR = UnityEngine.AdaptivePerformance.Samsung.Android.VariableRefreshRate.Instance;
        timeOuttimer = timeOut;

        Application.targetFrameRate = 60;
        targetRefreshRate.SetValueWithoutNotify(60);

        if (m_VRR == null)
        {
            Debug.Log("[AP VRR] Variable Refresh Rate is not supported on this device.");
            notSupportedPanel.SetActive(true);
            enabled = false;
            return;
        }

        m_VRR.RefreshRateChanged += UpdateDropdown;
        supportedRefreshRates.onValueChanged.AddListener(delegate {
            if (!m_VRR.SetRefreshRateByIndex(supportedRefreshRates.value))
                UpdateDropdown();
        });

        targetRefreshRate.onValueChanged.AddListener(delegate {
            Application.targetFrameRate = (int)targetRefreshRate.value;
        });
        UpdateDropdown();
    }

    void Update()
    {
        targetRefreshRateLabel.text = $"Target Refresh Rate: {Application.targetFrameRate} Hz";
        currentRefreshRate.text = $"Current refresh rate: {m_VRR.CurrentRefreshRate} Hz";

        timeOuttimer -= Time.deltaTime;

        if (timeOuttimer < 0)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    void UpdateDropdown()
    {
        var options = new List<Dropdown.OptionData>();
        supportedRefreshRates.ClearOptions();

        var index = -1;
        for (var i = 0; i < m_VRR.SupportedRefreshRates.Length; ++i)
        {
            var rr = m_VRR.SupportedRefreshRates[i];
            options.Add(new Dropdown.OptionData(rr.ToString()));
            if (rr == m_VRR.CurrentRefreshRate)
                index = i;
        }

        supportedRefreshRates.AddOptions(options);
        supportedRefreshRates.SetValueWithoutNotify(index);
    }
}
