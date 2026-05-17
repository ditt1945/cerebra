// ============================================================
// FruitUI.cs
//
// HUD counter buah pojok kiri atas.
// Menampilkan: [ikon] 3/8
// Berubah hijau + pop saat semua terkumpul.
//
// Pasang di: GameObject "FruitUI" di dalam Canvas
// ============================================================

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FruitUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fruitIcon;
    [SerializeField] private TextMeshProUGUI counterText;

    [Header("Style")]
    [SerializeField] private Color defaultColor  = Color.white;
    [SerializeField] private Color completeColor = new Color(0.2f, 1f, 0.3f);

    private void Start()
    {
        if (FruitManager.Instance == null)
        {
            Debug.LogWarning("[FruitUI] FruitManager tidak ditemukan!");
            return;
        }

        FruitManager.Instance.OnFruitCollected.AddListener(UpdateUI);
        FruitManager.Instance.OnAllFruitsCollected.AddListener(OnAllCollected);
    }

    private void OnDestroy()
    {
        if (FruitManager.Instance == null) return;
        FruitManager.Instance.OnFruitCollected.RemoveListener(UpdateUI);
        FruitManager.Instance.OnAllFruitsCollected.RemoveListener(OnAllCollected);
    }

    private void UpdateUI(int collected, int total)
    {
        if (counterText != null)
            counterText.text = $"{collected}/{total}";
    }

    private void OnAllCollected()
    {
        if (counterText != null)
            counterText.color = completeColor;

        StartCoroutine(PopAnimation());
    }

    private IEnumerator PopAnimation()
    {
        Vector3 original = transform.localScale;
        Vector3 big      = original * 1.35f;
        float   duration = 0.12f;
        float   t        = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(original, big, t / duration);
            yield return null;
        }

        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(big, original, t / duration);
            yield return null;
        }

        transform.localScale = original;
    }
}