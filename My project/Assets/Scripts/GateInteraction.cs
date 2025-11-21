using UnityEngine;
using TMPro;

public class GateInteraction : MonoBehaviour
{
    public TMP_Text warningText;   // Assign ONLY the warning text object
    public float fadeTime = 2f;

    private Renderer gateRenderer;
    private bool fading = false;

    void Start()
    {
        gateRenderer = GetComponent<Renderer>();
        if (warningText != null)
            warningText.gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        if (fading) return;

        if (CrystalManager.Instance.AllCrystalsCollected)
        {
            StartCoroutine(FadeOutGate());
        }
        else
        {
            ShowWarning();
        }
    }

    void ShowWarning()
    {
        if (warningText == null) return;

        warningText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideWarning));
        Invoke(nameof(HideWarning), 3f);
    }

    void HideWarning()
    {
        if (warningText != null)
            warningText.gameObject.SetActive(false);
    }

    System.Collections.IEnumerator FadeOutGate()
    {
        fading = true;

        Color startColor = gateRenderer.material.color;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, t / fadeTime);
            gateRenderer.material.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
