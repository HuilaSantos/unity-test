using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Tracks total and remaining (uncollected) treasures in the current scene.
/// Place a single instance in the scene (e.g., on a GameObject named "GameSystems").
/// Call ResetCounts() before spawning a new maze. TreasureItem will auto-report spawns/collections.
/// </summary>
public class TreasureCounter : MonoBehaviour
{
    public static TreasureCounter Instance { get; private set; }

    [Tooltip("Raised when the remaining treasure count changes (passes remaining).")]
    public UnityEvent<int> OnRemainingChanged = new UnityEvent<int>();

    [Tooltip("Raised when the total treasure count changes (passes total).")]
    public UnityEvent<int> OnTotalChanged = new UnityEvent<int>();

    [SerializeField, Tooltip("Number of treasures spawned this run.")]
    private int total;
    [SerializeField, Tooltip("Number of treasures not yet collected.")]
    private int remaining;

    public int Total => total;
    public int Remaining => remaining;
    public int Collected => Mathf.Max(0, total - remaining);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Optional: persist across scenes. Comment out if not desired.
        // DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Reset both total and remaining counts to zero. Call this when regenerating a maze.
    /// </summary>
    public void ResetCounts()
    {
        bool totalChanged = total != 0;
        total = 0;
        if (totalChanged) OnTotalChanged.Invoke(total);

        bool remainingChanged = remaining != 0;
        remaining = 0;
        if (remainingChanged) OnRemainingChanged.Invoke(remaining);
    }

    /// <summary>
    /// Register a newly spawned treasure (called automatically by TreasureItem).
    /// </summary>
    public void RegisterSpawn()
    {
        total++;
        OnTotalChanged.Invoke(total);

        remaining++;
        OnRemainingChanged.Invoke(remaining);
    }

    /// <summary>
    /// Register that a treasure has been collected (called by TreasureItem.Collect()).
    /// </summary>
    public void RegisterCollected()
    {
        if (remaining > 0)
        {
            remaining--;
            OnRemainingChanged.Invoke(remaining);
        }
    }
}
