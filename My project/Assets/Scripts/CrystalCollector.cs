using UnityEngine;

public class CrystalCollector : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Crystal"))
        {
            CrystalManager.Instance.CollectCrystal();
            Destroy(other.gameObject);
        }
    }
}
