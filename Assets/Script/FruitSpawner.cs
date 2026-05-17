// ============================================================
// FruitSpawner.cs — FINAL FIX
// - Spawn 1 buah di Start()
// - Spawn berikutnya HANYA jika belum semua terkumpul
// - Guard isSpawning cegah dobel spawn
// - totalToSpawn mengikuti FruitManager.TotalFruits
// ============================================================

using System.Collections;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [Header("Fruit Prefab")]
    [Tooltip("Drag FruitPrefab dari folder Assets/Prefabs ke sini.")]
    [SerializeField] private GameObject fruitPrefab;

    [Header("Fruit Animations")]
    [Tooltip("Drag semua AnimationClip IDLE buah ke sini (8 clip).\n" +
             "Urutan bebas, akan dipilih random setiap spawn.")]
    [SerializeField] private AnimationClip[] fruitIdleClips;

    [Tooltip("1 clip animasi Collected untuk semua buah.")]
    [SerializeField] private AnimationClip collectedClip;

    [Header("Spawn Settings")]
    [Tooltip("Jarak max horizontal dari posisi buah sebelumnya.")]
    [SerializeField] private float maxSpawnRadius = 5f;

    [Tooltip("Tinggi raycast start dari posisi terakhir.")]
    [SerializeField] private float raycastStartHeight = 5f;

    [Tooltip("Layer ground untuk raycast.")]
    [SerializeField] private LayerMask groundLayer;

    [Tooltip("Offset buah di atas ground.")]
    [SerializeField] private float heightOffset = 0.6f;

    [Tooltip("Berapa kali coba cari posisi valid.")]
    [SerializeField] private int maxAttempts = 15;

    [Tooltip("Delay sebelum spawn buah berikutnya (detik).")]
    [SerializeField] private float spawnDelay = 0.8f;

    [Header("First Spawn")]
    [Tooltip("Posisi buah pertama muncul.\n" +
             "Buat empty GameObject di posisi yang diinginkan, drag ke sini.")]
    [SerializeField] private Transform firstSpawnPoint;

    // -------------------------------------------------------
    private Vector3 lastSpawnPosition;
    private int     lastClipIndex = -1;
    private bool    isSpawning    = false;
    private int     spawnedCount  = 0;   // jumlah buah yang sudah pernah di-spawn

    // -------------------------------------------------------
    private void Start()
    {
        // Validasi
        if (fruitPrefab == null)
        {
            Debug.LogError("[FruitSpawner] fruitPrefab belum diisi di Inspector!");
            return;
        }
        if (fruitIdleClips == null || fruitIdleClips.Length == 0)
        {
            Debug.LogError("[FruitSpawner] fruitIdleClips kosong! Drag clip .anim ke Inspector.");
            return;
        }
        if (collectedClip == null)
        {
            Debug.LogError("[FruitSpawner] collectedClip belum diisi!");
            return;
        }

        lastSpawnPosition = firstSpawnPoint != null
            ? firstSpawnPoint.position
            : transform.position;

        // Spawn hanya 1 buah pertama
        SpawnFruit();
    }

    // -------------------------------------------------------
    // Dipanggil oleh Fruit.cs saat buah diambil
    // -------------------------------------------------------
    public void OnFruitCollected()
    {
        // Jika semua buah sudah terkumpul → tidak spawn lagi
        if (FruitManager.Instance != null && FruitManager.Instance.AllCollected)
        {
            Debug.Log("[FruitSpawner] Semua buah selesai, tidak spawn lagi.");
            return;
        }

        // Jika sudah spawn sebanyak totalFruits → tidak spawn lagi
        // (cegah spawn buah ke-3 dst ketika total = 2)
        if (FruitManager.Instance != null &&
            spawnedCount >= FruitManager.Instance.TotalFruits)
        {
            Debug.Log($"[FruitSpawner] Spawn limit ({FruitManager.Instance.TotalFruits}) tercapai.");
            return;
        }

        // Guard: cegah spawn dobel
        if (isSpawning) return;

        StartCoroutine(SpawnAfterDelay());
    }

    // -------------------------------------------------------
    private IEnumerator SpawnAfterDelay()
    {
        isSpawning = true;
        yield return new WaitForSeconds(spawnDelay);
        SpawnFruit();
        isSpawning = false;
    }

    // -------------------------------------------------------
    private void SpawnFruit()
    {
        Vector3 spawnPos = FindValidSpawnPosition();
        int     clipIndex = GetRandomClipIndex();

        GameObject fruit = Instantiate(fruitPrefab, spawnPos, Quaternion.identity);
        lastClipIndex     = clipIndex;
        lastSpawnPosition = spawnPos;
        spawnedCount++;

        Fruit fruitScript = fruit.GetComponent<Fruit>();
        if (fruitScript == null)
        {
            Debug.LogError("[FruitSpawner] FruitPrefab tidak punya komponen Fruit.cs!");
            return;
        }

        fruitScript.Setup(fruitIdleClips[clipIndex], collectedClip, this);

        Debug.Log($"[FruitSpawner] Spawn #{spawnedCount}: " +
                  $"'{fruitIdleClips[clipIndex].name}' di {spawnPos}");
    }

    // -------------------------------------------------------
    private Vector3 FindValidSpawnPosition()
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            float randomX = lastSpawnPosition.x +
                            Random.Range(-maxSpawnRadius, maxSpawnRadius);

            Vector2 origin = new Vector2(randomX,
                lastSpawnPosition.y + raycastStartHeight);

            RaycastHit2D hit = Physics2D.Raycast(
                origin, Vector2.down,
                raycastStartHeight * 2.5f,
                groundLayer
            );

            if (hit.collider != null)
                return new Vector3(randomX, hit.point.y + heightOffset, 0f);
        }

        Debug.LogWarning("[FruitSpawner] Ground tidak ditemukan, spawn di posisi terakhir.");
        return lastSpawnPosition + Vector3.up * heightOffset;
    }

    // -------------------------------------------------------
    private int GetRandomClipIndex()
    {
        if (fruitIdleClips.Length == 1) return 0;

        int index;
        int tries = 0;
        do
        {
            index = Random.Range(0, fruitIdleClips.Length);
            tries++;
        }
        while (index == lastClipIndex && tries < 20);

        return index;
    }

    // -------------------------------------------------------
    private void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying
            ? lastSpawnPosition
            : (firstSpawnPoint != null ? firstSpawnPoint.position : transform.position);

        Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.25f);
        Gizmos.DrawWireSphere(center, maxSpawnRadius);
    }
}