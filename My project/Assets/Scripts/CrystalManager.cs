using UnityEngine;
using TMPro;

public class CrystalManager : MonoBehaviour
{
    public static CrystalManager Instance;

    public TMP_Text crystalText;
    private int totalCrystals;

    public bool AllCrystalsCollected => totalCrystals == 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        totalCrystals = GameObject.FindGameObjectsWithTag("Crystal").Length;
        UpdateUI();
    }

    public void CollectCrystal()
    {
        totalCrystals--;
        UpdateUI();
    }

    private void UpdateUI()
    {
        crystalText.text = "Fragmentos Restantes: " + totalCrystals;
    }
}
