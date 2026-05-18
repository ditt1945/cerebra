// ============================================================
// TimerManager.cs
// - Countdown timer per level (diset di Inspector)
// - Timer berjalan terus sampai player menyentuh finish
// - Jika waktu habis → trigger Game Over
// - Timer berhenti saat game over atau finish tercapai
// ============================================================

using UnityEngine;
using UnityEngine.Events;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }

    [Header("Timer Settings")]
    [Tooltip("Durasi waktu level ini dalam detik.\n" +
             "Contoh: 90 = 1 menit 30 detik")]
    [SerializeField] private float levelDuration = 90f;

    [Tooltip("Waktu tersisa (detik) saat warna timer berubah merah sebagai peringatan.")]
    [SerializeField] private float warningThreshold = 15f;

    [Header("Events")]
    [HideInInspector] public UnityEvent<float, float> OnTimerTick;   // (sisa, total)
    [HideInInspector] public UnityEvent               OnTimerWarning; // saat masuk zona warning
    [HideInInspector] public UnityEvent               OnTimerExpired; // saat waktu habis

    // -------------------------------------------------------
    private float timeRemaining;
    private bool  isRunning      = false;
    private bool  isExpired      = false;
    private bool  warningFired   = false;

    public float TimeRemaining => timeRemaining;
    public float LevelDuration => levelDuration;
    public bool  IsRunning     => isRunning;
    public bool  IsExpired     => isExpired;

    // -------------------------------------------------------
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        OnTimerTick    = new UnityEvent<float, float>();
        OnTimerWarning = new UnityEvent();
        OnTimerExpired = new UnityEvent();
    }

    private void Start()
    {
        timeRemaining = levelDuration;
        isRunning     = true;
        isExpired     = false;
        warningFired  = false;

        // Broadcast kondisi awal ke UI
        OnTimerTick.Invoke(timeRemaining, levelDuration);
        Debug.Log($"[TimerManager] Timer mulai: {levelDuration} detik");
    }

    // -------------------------------------------------------
    private void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;

        // Clamp agar tidak negatif
        timeRemaining = Mathf.Max(timeRemaining, 0f);

        // Broadcast ke UI setiap frame
        OnTimerTick.Invoke(timeRemaining, levelDuration);

        // Warning threshold
        if (!warningFired && timeRemaining <= warningThreshold)
        {
            warningFired = true;
            OnTimerWarning.Invoke();
            Debug.Log("[TimerManager] ⚠️ Waktu hampir habis!");
        }

        // Waktu habis
        if (timeRemaining <= 0f && !isExpired)
        {
            isExpired = true;
            isRunning = false;
            Debug.Log("[TimerManager] ⏰ Waktu habis! Game Over.");
            OnTimerExpired.Invoke();

            // Trigger Game Over via GameOverManager
            if (GameOverManager.Instance != null)
                GameOverManager.Instance.TriggerGameOver();
            else
                Debug.LogWarning("[TimerManager] GameOverManager.Instance tidak ditemukan!");
        }
    }

    // -------------------------------------------------------
    // Dipanggil saat player menyentuh finish → hentikan timer
    // -------------------------------------------------------
    public void StopTimer()
    {
        isRunning = false;
        Debug.Log($"[TimerManager] Timer dihentikan. Sisa waktu: {timeRemaining:F1} detik");
    }

    // -------------------------------------------------------
    // Dipanggil jika ada power-up tambah waktu (opsional)
    // -------------------------------------------------------
    public void AddTime(float seconds)
    {
        timeRemaining = Mathf.Min(timeRemaining + seconds, levelDuration);
        Debug.Log($"[TimerManager] +{seconds} detik. Sisa: {timeRemaining:F1}");
    }
}