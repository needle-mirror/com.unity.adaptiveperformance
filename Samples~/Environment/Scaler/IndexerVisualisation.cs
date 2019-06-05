using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AdaptivePerformance;
using System.Collections.Generic;

public class IndexerVisualisation : MonoBehaviour
{
    public Text Timer;
    public Text Bottleneck;
    public Text PerformanceAction;
    public Text ThermalAction;
    public Text GpuFrameTime;
    public Text CpuFrameTime;
    public Transform Content;
    public ScalerVisualisation ScalerVisualisationPrefab;

    private List<ScalerVisualisation> m_ScalerVisualisations;
    private List<AdaptivePerformanceScaler> m_AppliedScalers;
    private List<AdaptivePerformanceScaler> m_UnappliedScalers;

    private void Start()
    {
        var ap = Holder.Instance;
        if (ap == null || !ap.Active)
        {
            Debug.Log("[AP APC] Adaptive Performance not active");
            enabled = false;
            return;
        }
        m_ScalerVisualisations = new List<ScalerVisualisation>();
        m_AppliedScalers = new List<AdaptivePerformanceScaler>();
        m_UnappliedScalers = new List<AdaptivePerformanceScaler>();
    }

    private void Update()
    {
        Apply();
    }

    private void Apply()
    {
        Debug.Assert(Timer);
        Debug.Assert(Bottleneck);
        Debug.Assert(PerformanceAction);
        Debug.Assert(ThermalAction);
        Debug.Assert(GpuFrameTime);
        Debug.Assert(CpuFrameTime);
        Debug.Assert(Content);
        Debug.Assert(ScalerVisualisationPrefab);

        var indexer = Holder.Instance.Indexer;
        if (indexer == null)
            return;

        Timer.text = $"  Timer: <b>{Mathf.RoundToInt(indexer.TimeUntilNextAction)}</b>";
        Bottleneck.text =
            $"  Bottleneck: <b>{Holder.Instance.PerformanceStatus.PerformanceMetrics.PerformanceBottleneck}</b>";

        var perfAction = indexer.PerformanceAction;
        if (perfAction != StateAction.Decrease && perfAction != StateAction.FastDecrease)
            PerformanceAction.text = $"  Perf action: <color=lime>{perfAction}</color>";
        else
            PerformanceAction.text = $"  Perf action: <color=red>{perfAction}</color>";

        var tempAction = indexer.ThermalAction;
        if (tempAction != StateAction.Decrease && tempAction != StateAction.FastDecrease)
            ThermalAction.text = $"  Thermal action: <color=lime>{tempAction}</color>";
        else
            ThermalAction.text = $"  Thermal action: <color=red>{tempAction}</color>";

        GpuFrameTime.text =
            $"  GPU frame time: {Holder.Instance.PerformanceStatus.FrameTiming.AverageGpuFrameTime:0.####}s";
        CpuFrameTime.text =
            $"  CPU frame time: {Holder.Instance.PerformanceStatus.FrameTiming.AverageCpuFrameTime:0.####}s";

        indexer.GetAppliedScalers(ref m_AppliedScalers);
        foreach (var scaler in m_AppliedScalers)
            CreateScalerVisualisation(scaler);

        indexer.GetUnappliedScalers(ref m_UnappliedScalers);
        foreach (var scaler in m_UnappliedScalers)
            CreateScalerVisualisation(scaler);
    }

    private void CreateScalerVisualisation(AdaptivePerformanceScaler scaler)
    {
        if (ContainsScaler(scaler))
            return;

        var scalerVisualisation = Instantiate(ScalerVisualisationPrefab);
        scalerVisualisation.Scaler = scaler;
        Transform transform1;
        (transform1 = scalerVisualisation.transform).SetParent(Content);
        transform1.localScale = Vector3.one;
        scalerVisualisation.gameObject.SetActive(true);
        m_ScalerVisualisations.Add(scalerVisualisation);
    }

    private bool ContainsScaler(AdaptivePerformanceScaler scaler)
    {
        foreach (var current in m_ScalerVisualisations)
        {
            if (current.Scaler == scaler)
                return true;
        }
        return false;
    }
}
