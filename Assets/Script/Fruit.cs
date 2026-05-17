// ============================================================
// Fruit.cs — FINAL FIX
// Root cause: AnimatorOverrideController menggunakan NAMA CLIP
// placeholder, bukan nama state.
// Slot yang tersedia = "Apple_idle" dan "Collected"
// ============================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class Fruit : MonoBehaviour
{
    [Header("Override Clip Names")]
    [Tooltip("Nama CLIP placeholder di state Idle Animator.\n" +
             "Lihat Inspector state Idle → Motion = 'Apple_idle'\n" +
             "Isi field ini dengan nama clip tersebut.")]
    [SerializeField] private string idleClipName      = "Apple_idle"; // ← nama CLIP, bukan nama state

    [Tooltip("Nama CLIP placeholder di state Collected Animator.")]
    [SerializeField] private string collectedClipName = "Collected";

    [Tooltip("Nama STATE Collected di Animator (untuk cek via GetCurrentAnimatorStateInfo).")]
    [SerializeField] private string collectedStateName = "Collected";

    [Tooltip("Delay tambahan setelah animasi Collected selesai sebelum Destroy.")]
    [SerializeField] private float destroyDelay = 0.3f;

    [Header("Audio")]
    [SerializeField] private AudioClip collectSound;
    [SerializeField, Range(0f, 1f)] private float collectVolume = 0.8f;

    // -------------------------------------------------------
    private Animator     animator;
    private Collider2D   col;
    private FruitSpawner spawner;
    private bool         isCollected = false;
    private bool         isSetup     = false;

    // -------------------------------------------------------
    private void Awake()
    {
        animator      = GetComponent<Animator>();
        col           = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    // -------------------------------------------------------
    // Dipanggil FruitSpawner setelah Instantiate
    // -------------------------------------------------------
    public void Setup(AnimationClip idleClip, AnimationClip collectedClip,
                      FruitSpawner fruitSpawner)
    {
        spawner = fruitSpawner;

        // AnimatorOverrideController menggunakan nama CLIP placeholder
        // bukan nama STATE. Sesuai Animator kamu:
        //   state "Idle"      → Motion = "Apple_idle"  → key = "Apple_idle"
        //   state "Collected" → Motion = "Collected"   → key = "Collected"
        var overrideCtrl = new AnimatorOverrideController(
            animator.runtimeAnimatorController
        );

        overrideCtrl[idleClipName]      = idleClip;      // ganti Apple_idle → clip buah yg dipilih
        overrideCtrl[collectedClipName] = collectedClip; // ganti Collected  → clip collected

        animator.runtimeAnimatorController = overrideCtrl;

        isSetup = true;
        Debug.Log($"[Fruit] Setup OK → '{idleClipName}' diganti '{idleClip.name}'");
    }

    // -------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isSetup)                    return;
        if (isCollected)                 return;
        if (!other.CompareTag("Player")) return;

        Collect();
    }

    // -------------------------------------------------------
    private void Collect()
    {
        isCollected = true;
        col.enabled = false; // cegah trigger dobel

        animator.SetTrigger("Collect");

        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound,
                transform.position, collectVolume);

        FruitManager.Instance?.RegisterCollected();
        spawner?.OnFruitCollected();

        StartCoroutine(DestroyAfterAnimation());
    }

    // -------------------------------------------------------
    private IEnumerator DestroyAfterAnimation()
    {
        // Tunggu 1 frame agar Animator proses trigger
        yield return null;

        // Tunggu state Collected aktif (timeout 1.5 detik)
        float timeout = 0f;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(collectedStateName))
        {
            timeout += Time.deltaTime;
            if (timeout > 1.5f)
            {
                Debug.LogWarning($"[Fruit] Timeout — state '{collectedStateName}' tidak aktif. " +
                                 "Periksa nama Collected State Name di Inspector.");
                break;
            }
            yield return null;
        }

        float clipLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(Mathf.Max(clipLength, 0.1f) + destroyDelay);

        Destroy(gameObject);
    }

    // -------------------------------------------------------
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.8f, 0f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}