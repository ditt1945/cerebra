// ============================================================
// DoorController.cs
// - Pintu hidden di awal
// - Muncul otomatis saat semua buah terkumpul
// - Subscribe ke FruitManager.OnAllFruitsCollected
// ============================================================

using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Object")]
    [Tooltip("Drag GameObject End (Idle) ke sini.\n" +
             "Akan hidden di awal dan muncul saat semua buah terkumpul.")]
    [SerializeField] private GameObject doorObject;

    [Header("Optional")]
    [Tooltip("Efek/partikel saat pintu muncul (opsional, boleh kosong).")]
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
        }

        // Subscribe ke event semua buah terkumpul
        if (FruitManager.Instance != null)
        {
            FruitManager.Instance.OnAllFruitsCollected.AddListener(OnAllFruitsCollected);
            Debug.Log("[DoorController] Subscribe ke FruitManager.OnAllFruitsCollected.");
        }
        else
        {
            Debug.LogError("[DoorController] FruitManager.Instance tidak ditemukan!");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe saat object dihancurkan
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

        // Tampilkan efek muncul jika ada
        if (appearEffect != null)
            appearEffect.SetActive(true);
    }
}