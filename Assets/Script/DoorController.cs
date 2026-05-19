// ============================================================
// DoorController.cs — UPDATED
// - Hanya mengatur SHOW/HIDE pintu berdasarkan buah terkumpul
// - Tidak mengatur buka/tutup (itu urusan DoorOpenTrigger.cs)
// ============================================================

using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Object")]
    [Tooltip("Drag GameObject pintu (End Idle) ke sini.\n" +
             "Akan hidden di awal dan muncul saat semua buah terkumpul.")]
    [SerializeField] private GameObject doorObject;

    [Header("Optional")]
    [Tooltip("Efek/partikel saat pintu pertama muncul (boleh kosong).")]
    [SerializeField] private GameObject appearEffect;

    // -------------------------------------------------------
    private void Start()
    {
        // Sembunyikan pintu di awal
        if (doorObject != null)
        {
            doorObject.SetActive(false);
            Debug.Log("[DoorController] Pintu disembunyikan di awal.");
        }
        else
        {
            Debug.LogError("[DoorController] doorObject belum diisi di Inspector!");
            return;
        }

        // Subscribe ke event semua buah terkumpul
        if (FruitManager.Instance != null)
            FruitManager.Instance.OnAllFruitsCollected.AddListener(OnAllFruitsCollected);
        else
            Debug.LogError("[DoorController] FruitManager.Instance tidak ditemukan!");
    }

    private void OnDestroy()
    {
        if (FruitManager.Instance != null)
            FruitManager.Instance.OnAllFruitsCollected.RemoveListener(OnAllFruitsCollected);
    }

    // -------------------------------------------------------
    // Dipanggil FruitManager saat semua buah terkumpul
    // -------------------------------------------------------
    private void OnAllFruitsCollected()
    {
        Debug.Log("[DoorController] Semua buah terkumpul! Pintu muncul.");

        if (doorObject != null)
            doorObject.SetActive(true);

        if (appearEffect != null)
            appearEffect.SetActive(true);
    }
}