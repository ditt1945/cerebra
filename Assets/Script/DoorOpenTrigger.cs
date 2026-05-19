// ============================================================
// DoorOpenTrigger.cs — UPDATED
// - Animator buka/tutup pintu (tetap seperti sebelumnya)
// - Tambahan: suara saat pintu terbuka dan menutup
// - AudioSource ditambahkan otomatis jika belum ada
// ============================================================

using UnityEngine;

public class DoorOpenTrigger : MonoBehaviour
{
    [Header("Animator")]
    [Tooltip("Drag Animator pintu ke sini.")]
    [SerializeField] private Animator doorAnimator;

    [Header("Sound")]
    [Tooltip("Suara saat pintu TERBUKA (player mendekat).")]
    [SerializeField] private AudioClip doorOpenSound;

    [Tooltip("Suara saat pintu MENUTUP (player menjauh).")]
    [SerializeField] private AudioClip doorCloseSound;

    [Tooltip("Volume suara pintu.")]
    [SerializeField, Range(0f, 1f)] private float doorVolume = 1f;

    // -------------------------------------------------------
    private AudioSource audioSource;

    private void Awake()
    {
        // Ambil atau tambahkan AudioSource otomatis
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.volume      = doorVolume;
    }

    // -------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Debug.Log("[DoorOpenTrigger] Player mendekat → pintu terbuka.");

        // Buka animasi
        if (doorAnimator != null)
            doorAnimator.SetBool("Open", true);

        // Suara buka
        PlaySound(doorOpenSound);
    }

    // -------------------------------------------------------
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Debug.Log("[DoorOpenTrigger] Player menjauh → pintu menutup.");

        // Tutup animasi
        if (doorAnimator != null)
            doorAnimator.SetBool("Open", false);

        // Suara tutup
        PlaySound(doorCloseSound);
    }

    // -------------------------------------------------------
    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.clip = clip;
        audioSource.Play();
    }
}