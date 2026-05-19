// ============================================================
// BackgroundMusic.cs
// - Play backsound otomatis saat scene dimulai
// - Loop terus menerus
// - Volume bisa diatur di Inspector
// - Taruh 1 script ini di setiap scene dengan lagu berbeda
// ============================================================

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    [Header("Music Settings")]
    [Tooltip("Drag file audio (.mp3/.wav/.ogg) ke sini.")]
    [SerializeField] private AudioClip musicClip;

    [Tooltip("Volume backsound (0 = mute, 1 = full).")]
    [SerializeField, Range(0f, 1f)] private float volume = 0.5f;

    [Tooltip("Mulai dari detik ke berapa (0 = dari awal).")]
    [SerializeField] private float startTime = 0f;

    // -------------------------------------------------------
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Setup AudioSource
        audioSource.clip        = musicClip;
        audioSource.volume      = volume;
        audioSource.loop        = true;
        audioSource.playOnAwake = false;
        audioSource.time        = startTime;
    }

    private void Start()
    {
        if (musicClip == null)
        {
            Debug.LogWarning("[BackgroundMusic] musicClip belum diisi di Inspector!");
            return;
        }

        audioSource.Play();
        Debug.Log($"[BackgroundMusic] Memutar: {musicClip.name}");
    }
}