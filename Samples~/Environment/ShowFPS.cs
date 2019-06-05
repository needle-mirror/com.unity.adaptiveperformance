using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AdaptivePerformance;

public class ShowFPS : MonoBehaviour
{
    public Text TargetFPS, CurrentFPS;

    float frameAverage;
    IPerformanceStatus perfStatus;

    void Start()
    {
        perfStatus = Holder.Instance.PerformanceStatus;
        TargetFPS.text = Application.targetFrameRate.ToString();
    }

    void Update()
    {
        frameAverage = 1 / perfStatus.FrameTiming.AverageFrameTime;
        CurrentFPS.text = frameAverage.ToString("F2");
        TargetFPS.text = Application.targetFrameRate.ToString();
    }
}
