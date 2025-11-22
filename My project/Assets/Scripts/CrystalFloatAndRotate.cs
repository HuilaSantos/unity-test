using UnityEngine;

/// <summary>
/// Makes a crystal (or any object) float up and down and optionally rotate.
/// Personalize distance, bob speed (cycles per second), and rotation speed.
/// Attach this script to crystal prefabs.
/// </summary>
public class CrystalFloatAndRotate : MonoBehaviour
{
    [Header("Vertical Movement")]
    [SerializeField] private bool enableVerticalMovement = true;
    [Tooltip("Total peak-to-peak vertical travel distance.")] 
    [SerializeField] private float distance = 0.5f;
    [Tooltip("Bob cycles per second (frequency). 1 = one full up/down per second.")]
    [SerializeField] private float bobSpeed = 1f;
    [Tooltip("Optional curve overriding sine. Time normalized 0-1 per cycle.")]
    [SerializeField] private AnimationCurve movementCurve;

    [Header("Rotation")]
    [SerializeField] private bool enableRotation = false;
    [Tooltip("Degrees per second around each local axis.")]
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 30f, 0f);

    [Header("Options")]
    [Tooltip("Randomizes starting phase so multiple crystals are offset.")]
    [SerializeField] private bool randomizeStartPhase = true;
    [Tooltip("Apply vertical movement using localPosition instead of world position.")]
    [SerializeField] private bool useLocalSpace = true;

    private float _startY;
    private float _phase; // Radians offset for sine; also used for curve time offset.

    private void Awake()
    {
        // Provide a default looping curve if user sets one empty.
        if (movementCurve != null && movementCurve.length == 0)
        {
            movementCurve = new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(0.25f, 1f),
                new Keyframe(0.5f, 0f),
                new Keyframe(0.75f, -1f),
                new Keyframe(1f, 0f)
            );
            movementCurve.preWrapMode = WrapMode.Loop;
            movementCurve.postWrapMode = WrapMode.Loop;
        }
    }

    private void Start()
    {
        _phase = randomizeStartPhase ? Random.value * Mathf.PI * 2f : 0f;
        _startY = useLocalSpace ? transform.localPosition.y : transform.position.y;
    }

    private void Update()
    {
        if (enableVerticalMovement && distance > 0f && bobSpeed > 0f)
        {
            float cycleTime = Time.time * bobSpeed; // cycles per second
            float offset;

            if (movementCurve != null && movementCurve.length > 0)
            {
                // Normalize time for curve evaluation, add phase as fraction of cycle.
                float normalized = (cycleTime + _phase / (Mathf.PI * 2f)) % 1f;
                float curveValue = movementCurve.Evaluate(normalized); // Expect -1..1 similar to sine.
                offset = curveValue * (distance * 0.5f);
            }
            else
            {
                offset = Mathf.Sin(cycleTime * Mathf.PI * 2f + _phase) * (distance * 0.5f);
            }

            if (useLocalSpace)
            {
                var p = transform.localPosition;
                p.y = _startY + offset;
                transform.localPosition = p;
            }
            else
            {
                var p = transform.position;
                p.y = _startY + offset;
                transform.position = p;
            }
        }

        if (enableRotation && rotationSpeed != Vector3.zero)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    /// <summary>
    /// Dynamically adjust bobbing parameters at runtime.
    /// </summary>
    public void SetBob(float newDistance, float newBobSpeed)
    {
        distance = Mathf.Max(0f, newDistance);
        bobSpeed = Mathf.Max(0f, newBobSpeed);
    }

    /// <summary>
    /// Enable/disable rotation and set speeds.
    /// </summary>
    public void SetRotation(Vector3 newRotationSpeed, bool enable = true)
    {
        rotationSpeed = newRotationSpeed;
        enableRotation = enable && newRotationSpeed != Vector3.zero;
    }

    /// <summary>
    /// Force recalculation of start Y if object was moved externally.
    /// </summary>
    public void Reanchor()
    {
        _startY = useLocalSpace ? transform.localPosition.y : transform.position.y;
    }
}
