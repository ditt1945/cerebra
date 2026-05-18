// ============================================================
// FinishTrigger.cs — UPDATED
// - Hanya aktif jika pintu sudah muncul (semua buah terkumpul)
// - Trigger finish panel saat player menyentuh pintu
// - Stop timer saat finish
// ============================================================

using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag FinishManager ke sini.")]
    public FinishManager finishManager;

    // -------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // Double check: semua buah sudah terkumpul?
        if (FruitManager.Instance != null && !FruitManager.Instance.AllCollected)
        {
            Debug.Log("[FinishTrigger] Buah belum semua terkumpul, pintu tidak bisa dimasuki.");
            return;
        }

        Debug.Log("[FinishTrigger] Player masuk pintu! Level selesai.");

        // Stop timer
        if (TimerManager.Instance != null)
            TimerManager.Instance.StopTimer();

        // Tampilkan panel finish
        if (finishManager != null)
            finishManager.FinishLevel();
        else
            Debug.LogError("[FinishTrigger] finishManager belum diisi di Inspector!");
    }
}