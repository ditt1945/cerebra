using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [Header("References")]
    public FinishManager finishManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        // cek apakah semua buah sudah diambil
        if (FruitManager.Instance != null &&
            !FruitManager.Instance.AllCollected)
        {
            Debug.Log("Buah belum lengkap!");
            return;
        }

        Debug.Log("LEVEL SELESAI!");

        // stop timer
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.StopTimer();
        }

        // tampilkan panel finish
        if (finishManager != null)
        {
            finishManager.FinishLevel();
        }
    }
}