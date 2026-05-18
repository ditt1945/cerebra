// ============================================================
// TimerUI.cs
// - Menampilkan countdown timer di UI
// - Format: MM:SS
// - Warna normal → merah saat warning
// - Efek shake/pulse saat warning
// ============================================================

using System.Collections;
using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag Text (TMP) untuk tampilan timer ke sini.")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Colors")]
    [Tooltip("Warna teks timer saat waktu normal.")]
    [SerializeField] private Color normalColor  = Color.white;

    [Tooltip("Warna teks timer saat waktu hampir habis (warning).")]
    [SerializeField] private Color warningColor = Color.red;

    [Header("Warning Effect")]
    [Tooltip("Aktifkan efek pulse saat warning.")]
    [SerializeField] private bool  enablePulse     = true;

    [Tooltip("Kecepatan efek pulse.")]
    [SerializeField] private float pulseSpeed      = 3f;

    [Tooltip("Skala maksimal saat pulse.")]
    [SerializeField] private float pulseScaleMax   = 1.2f;

    // -------------------------------------------------------
    private bool      isWarning       = false;
    private Vector3   originalScale;
    private Coroutine pulseCoroutine;

    // -------------------------------------------------------
    private void Awake()
    {
        if (timerText == null)
        {
            Debug.LogError("[TimerUI] timerText belum diisi di Inspector!");
            return;
        }

        originalScale      = timerText.transform.localScale;
        timerText.color    = normalColor;
    }

    private void Start()
    {
        // Subscribe ke events TimerManager
        if (TimerManager.Instance == null)
        {
            Debug.LogError("[TimerUI] TimerManager.Instance tidak ditemukan!");
            return;
        }

        TimerManager.Instance.OnTimerTick.AddListener(UpdateTimerDisplay);
        TimerManager.Instance.OnTimerWarning.AddListener(ActivateWarning);
    }

    private void OnDestroy()
    {
        // Unsubscribe saat object dihancurkan
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.OnTimerTick.RemoveListener(UpdateTimerDisplay);
            TimerManager.Instance.OnTimerWarning.RemoveListener(ActivateWarning);
        }
    }

    // -------------------------------------------------------
    // Update tampilan timer setiap tick
    // -------------------------------------------------------
    private void UpdateTimerDisplay(float timeRemaining, float totalTime)
    {
        if (timerText == null) return;

        // Format MM:SS
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    // -------------------------------------------------------
    // Aktifkan mode warning: warna merah + pulse
    // -------------------------------------------------------
    private void ActivateWarning()
    {
        isWarning          = true;
        timerText.color    = warningColor;

        if (enablePulse)
        {
            if (pulseCoroutine != null)
                StopCoroutine(pulseCoroutine);
            pulseCoroutine = StartCoroutine(PulseEffect());
        }

        Debug.Log("[TimerUI] Warning aktif!");
    }

    // -------------------------------------------------------
    private IEnumerator PulseEffect()
    {
        while (isWarning)
        {
            // Scale up
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * pulseSpeed;
                float scale = Mathf.Lerp(1f, pulseScaleMax, t);
                timerText.transform.localScale = originalScale * scale;
                yield return null;
            }

            // Scale down
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * pulseSpeed;
                float scale = Mathf.Lerp(pulseScaleMax, 1f, t);
                timerText.transform.localScale = originalScale * scale;
                yield return null;
            }
        }

        // Reset scale saat selesai
        timerText.transform.localScale = originalScale;
    }
}