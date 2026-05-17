// ============================================================
// FruitManager.cs — FINAL FIX
// - Total buah diset manual di Inspector
// - Guard agar OnAllFruitsCollected tidak dobel
// ============================================================

using UnityEngine;
using UnityEngine.Events;

public class FruitManager : MonoBehaviour
{
    public static FruitManager Instance { get; private set; }

    [HideInInspector] public UnityEvent<int, int> OnFruitCollected;
    [HideInInspector] public UnityEvent           OnAllFruitsCollected;

    [Header("Settings")]
    [Tooltip("Total buah yang harus dikumpulkan.\n" +
             "Stage 1 Tutorial = 2\n" +
             "Pastikan sama dengan jumlah buah yang di-spawn FruitSpawner.")]
    [SerializeField] private int totalFruits = 2;

    // -------------------------------------------------------
    private int  collectedFruits   = 0;
    private bool allCollectedFired = false;

    public int  TotalFruits     => totalFruits;
    public int  CollectedFruits => collectedFruits;
    public bool AllCollected    => collectedFruits >= totalFruits;

    // -------------------------------------------------------
    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        OnFruitCollected     = new UnityEvent<int, int>();
        OnAllFruitsCollected = new UnityEvent();
    }

    private void Start()
    {
        collectedFruits   = 0;
        allCollectedFired = false;

        // Broadcast kondisi awal ke UI
        OnFruitCollected.Invoke(0, totalFruits);
        Debug.Log($"[FruitManager] Start — target: {totalFruits} buah");
    }

    // -------------------------------------------------------
    // Dipanggil Fruit.cs setiap kali buah diambil
    // -------------------------------------------------------
    public void RegisterCollected()
    {
        if (collectedFruits >= totalFruits) return; // guard overflow

        collectedFruits++;
        Debug.Log($"[FruitManager] Terkumpul: {collectedFruits}/{totalFruits}");

        OnFruitCollected.Invoke(collectedFruits, totalFruits);

        if (AllCollected && !allCollectedFired)
        {
            allCollectedFired = true;
            Debug.Log("[FruitManager] ✅ SEMUA BUAH TERKUMPUL!");
            OnAllFruitsCollected.Invoke();
        }
    }
}