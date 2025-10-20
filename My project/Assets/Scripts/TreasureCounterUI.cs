using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple UI binder to show remaining and total treasures.
/// Assign a Text component in the inspector.
/// </summary>
public class TreasureCounterUI : MonoBehaviour
{
    public Text label;

    private void OnEnable()
    {
        if (TreasureCounter.Instance != null)
        {
            TreasureCounter.Instance.OnRemainingChanged.AddListener(UpdateLabelRemaining);
            TreasureCounter.Instance.OnTotalChanged.AddListener(UpdateLabelTotal);
            RefreshNow();
        }
    }

    private void OnDisable()
    {
        if (TreasureCounter.Instance != null)
        {
            TreasureCounter.Instance.OnRemainingChanged.RemoveListener(UpdateLabelRemaining);
            TreasureCounter.Instance.OnTotalChanged.RemoveListener(UpdateLabelTotal);
        }
    }

    private void RefreshNow()
    {
        if (label == null || TreasureCounter.Instance == null) return;
        label.text = $"Treasures: {TreasureCounter.Instance.Remaining}/{TreasureCounter.Instance.Total}";
    }

    private void UpdateLabelRemaining(int _)
    {
        RefreshNow();
    }

    private void UpdateLabelTotal(int _)
    {
        RefreshNow();
    }
}
