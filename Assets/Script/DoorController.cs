using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Object")]
    [Tooltip("Drag End (Idle) ke sini")]
    [SerializeField] private GameObject doorObject;

    [Header("Optional Effect")]
    [SerializeField] private GameObject appearEffect;

    // =====================================================
    private void Start()
    {
        // hidden di awal
        if (doorObject != null)
        {
            doorObject.SetActive(false);

            Debug.Log("[DoorController] Pintu hidden.");
        }
        else
        {
            Debug.LogError("[DoorController] doorObject belum diisi!");
        }

        // subscribe event semua buah terkumpul
        if (FruitManager.Instance != null)
        {
            FruitManager.Instance
                .OnAllFruitsCollected
                .AddListener(OnAllFruitsCollected);
        }
        else
        {
            Debug.LogError("[DoorController] FruitManager tidak ditemukan!");
        }
    }

    // =====================================================
    private void OnDestroy()
    {
        if (FruitManager.Instance != null)
        {
            FruitManager.Instance
                .OnAllFruitsCollected
                .RemoveListener(OnAllFruitsCollected);
        }
    }

    // =====================================================
    private void OnAllFruitsCollected()
    {
        Debug.Log("[DoorController] Semua buah terkumpul!");

        // munculkan pintu
        if (doorObject != null)
        {
            doorObject.SetActive(true);
        }

        // efek opsional
        if (appearEffect != null)
        {
            appearEffect.SetActive(true);
        }
    }
}