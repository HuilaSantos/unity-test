using UnityEngine;

public class CrystalCollector : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Sound played when a crystal is collected.")]
    [SerializeField] private AudioClip collectSound;
    [Range(0f,1f)] [SerializeField] private float collectVolume = 1f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Crystal"))
        {
            // Play one-shot audio at crystal position (3D if listener present).
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, other.transform.position, collectVolume);
            }
            CrystalManager.Instance.CollectCrystal();
            Destroy(other.gameObject);
        }
    }
}
