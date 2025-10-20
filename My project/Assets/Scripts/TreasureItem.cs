using UnityEngine;

/// <summary>
/// Attach this to your treasure prefab. It will:
/// - Report itself to TreasureCounter on enable (spawn)
/// - Decrement remaining on collection, then destroy itself
/// Supports two collection modes:
/// 1) Trigger-based (if a Collider with isTrigger enabled is present)
/// 2) Manual call via Collect() from your interaction script
/// </summary>
public class TreasureItem : MonoBehaviour
{
    [Tooltip("If true, will auto-collect when a collider tagged Player enters its trigger.")]
    public bool triggerCollect = true;

    [Tooltip("Optional tag to match the player for trigger-based collect.")]
    public string playerTag = "Player";

    private bool _reportedSpawn;
    private bool _collected;

    private void OnEnable()
    {
        // Report spawn once per enable
        if (!_reportedSpawn && TreasureCounter.Instance != null)
        {
            TreasureCounter.Instance.RegisterSpawn();
            _reportedSpawn = true;
        }
    }

    public void Collect()
    {
        if (_collected) return;
        _collected = true;

        if (TreasureCounter.Instance != null)
        {
            TreasureCounter.Instance.RegisterCollected();
        }

        // Destroy the treasure object after collection
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerCollect) return;
        if (!string.IsNullOrEmpty(playerTag) && !other.CompareTag(playerTag)) return;
        Collect();
    }
}
