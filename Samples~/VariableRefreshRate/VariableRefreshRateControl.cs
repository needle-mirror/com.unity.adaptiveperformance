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

        Application.targetFrameRate = 120;

        if (m_VRR == null)
        {
            Debug.Log("[AP VRR] Variable Refresh Rate is not supported on this device.");
            notSupportedPanel.SetActive(true);

            return;
        }
        else
        {
            RefreshRateChanged();
        }

        m_VRR.RefreshRateChanged += RefreshRateChanged;
        supportedRefreshRates.onValueChanged.AddListener(delegate {
            UpdateCurrentRefreshRate(supportedRefreshRates.value);
        });

        targetRefreshRate.onValueChanged.AddListener(delegate {
            Application.targetFrameRate = (int)targetRefreshRate.value;
            UpdateTargetRefreshRateText(Application.targetFrameRate);
        });
        targetRefreshRate.value = Application.targetFrameRate;
    }

    void Update()
    {
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

    void UpdateCurrentRefreshRate(int refreshRateIndex)
    {
        UpdateCurrentRefreshRateText(m_VRR.SupportedRefreshRates[refreshRateIndex]);
        if (!m_VRR.SetRefreshRateByIndex(refreshRateIndex))
        {
            Debug.Log("[AP VRR] Setting Variable Refresh Rate to {0} is not supported." + m_VRR.SupportedRefreshRates[refreshRateIndex]);
            return;
        }
        supportedRefreshRates.value = refreshRateIndex;
    }

    void UpdateCurrentRefreshRateText(int refreshRateHz)
    {
        currentRefreshRate.text = $"Current refresh rate: {refreshRateHz} Hz";
    }

    void UpdateTargetRefreshRateText(int targetRateHz)
    {
        targetRefreshRateLabel.text = $"Target Refresh Rate: {targetRateHz} Hz";
    }

    void OnApplicationPause(bool pause)
    {
        if (!pause && m_VRR != null)
        {
            RefreshRateChanged();
        }
    }

    void RefreshRateChanged()
    {
        var refreshRate = m_VRR.CurrentRefreshRate;
        var options = new List<Dropdown.OptionData>();
        var index = -1;

        supportedRefreshRates.ClearOptions();

        for (var i = 0; i < m_VRR.SupportedRefreshRates.Length; ++i)
        {
            var rr = m_VRR.SupportedRefreshRates[i];
            if (rr == refreshRate)
                index = i;
            options.Add(new Dropdown.OptionData(rr.ToString()));
        }

        supportedRefreshRates.AddOptions(options);

        if (index != -1)
            supportedRefreshRates.value = index;

        UpdateCurrentRefreshRateText(m_VRR.CurrentRefreshRate);
        UpdateTargetRefreshRateText(Application.targetFrameRate);
    }
}
