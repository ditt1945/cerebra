// ============================================================
// PlayerAudio.cs
//
// Sistem audio untuk Player Bera.
// Menangani suara:
//   - Footstep (jalan di tanah) — berhenti saat player diam
//   - Jump (lompat pertama)
//   - Double Jump / Air Roll
//   - Masuk ke area air (splash)
//
// Fix:
//   - Footstep menggunakan AudioSource dedicated dengan
//     isPlaying check — tidak akan dobel dan berhenti
//     otomatis saat player diam atau di udara.
//   - SFX (jump, roll, splash) dicek isPlaying sebelum
//     diputar agar tidak overlap satu sama lain.
//
// Cara pasang:
//   1. Pasang script ini di GameObject Player (Bera).
//   2. Isi semua AudioClip di Inspector dengan file WAV.
//   3. Tag area air dengan "Water", centang isTrigger.
//   4. Tag semua platform/tanah dengan "Ground".
//
// Dipasang di: GameObject Player
// ============================================================

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAudio : MonoBehaviour
{
    // -------------------------------------------------------
    // INSPECTOR FIELDS
    // -------------------------------------------------------
    [Header("Footstep")]
    [Tooltip("Suara langkah kaki saat Bera berjalan di tanah.\nFormat: WAV.")]
    [SerializeField] private AudioClip footstepClip;

    [Tooltip("Volume suara langkah kaki. Rekomendasi: 0.4 - 0.6")]
    [SerializeField, Range(0f, 1f)] private float footstepVolume = 0.5f;

    [Tooltip("Jeda antar langkah dalam detik.\n" +
             "Sesuaikan dengan animasi walk Bera. Rekomendasi: 0.3 - 0.5")]
    [SerializeField] private float footstepInterval = 0.35f;

    [Header("Jump")]
    [Tooltip("Suara lompat pertama (dari tanah).\nFormat: WAV.")]
    [SerializeField] private AudioClip jumpClip;

    [Tooltip("Volume suara lompat. Rekomendasi: 0.6 - 0.8")]
    [SerializeField, Range(0f, 1f)] private float jumpVolume = 0.7f;

    [Header("Air Roll")]
    [Tooltip("Suara double jump / air roll.\nFormat: WAV.")]
    [SerializeField] private AudioClip rollClip;

    [Tooltip("Volume suara roll. Rekomendasi: 0.6 - 0.8")]
    [SerializeField, Range(0f, 1f)] private float rollVolume = 0.7f;

    [Header("Water")]
    [Tooltip("Suara splash saat Bera masuk ke area air.\nFormat: WAV.")]
    [SerializeField] private AudioClip splashClip;

    [Tooltip("Volume suara splash. Rekomendasi: 0.7 - 1.0")]
    [SerializeField, Range(0f, 1f)] private float splashVolume = 0.8f;

    [Header("Settings")]
    [Tooltip("Kecepatan minimum Bera agar footstep berbunyi.\n" +
             "Mencegah suara langkah saat Bera diam.")]
    [SerializeField] private float minSpeedForFootstep = 0.1f;

    // -------------------------------------------------------
    // PRIVATE
    // -------------------------------------------------------

    // AudioSource terpisah agar footstep dan SFX tidak saling ganggu
    private AudioSource audioSourceFootstep;
    private AudioSource audioSourceSFX;

    private Rigidbody2D rb;

    private bool isGrounded  = false;
    private bool isInWater   = false;

    private float footstepTimer = 0f;

    // -------------------------------------------------------
    // UNITY LIFECYCLE
    // -------------------------------------------------------
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SetupAudioSources();
    }

    private void Update()
    {
        HandleFootstep();
    }

    // -------------------------------------------------------
    // SETUP — buat 2 AudioSource yang benar-benar terpisah
    // -------------------------------------------------------
    private void SetupAudioSources()
    {
        AudioSource[] existing = GetComponents<AudioSource>();

        // AudioSource 1: khusus footstep
        audioSourceFootstep = existing.Length > 0
            ? existing[0]
            : gameObject.AddComponent<AudioSource>();

        audioSourceFootstep.playOnAwake = false;
        audioSourceFootstep.loop        = false;
        audioSourceFootstep.volume      = footstepVolume;

        // AudioSource 2: khusus SFX (jump, roll, splash)
        audioSourceSFX = existing.Length > 1
            ? existing[1]
            : gameObject.AddComponent<AudioSource>();

        audioSourceSFX.playOnAwake = false;
        audioSourceSFX.loop        = false;
    }

    // -------------------------------------------------------
    // FOOTSTEP — timer berbasis interval, berhenti jika diam
    // -------------------------------------------------------
    private void HandleFootstep()
    {
        if (footstepClip == null) return;

        // Kondisi footstep boleh berbunyi:
        // grounded + tidak di air + bergerak cukup cepat
        bool shouldPlay = isGrounded
                       && !isInWater
                       && Mathf.Abs(rb.linearVelocity.x) > minSpeedForFootstep;

        if (shouldPlay)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                // Cek isPlaying — jika suara sebelumnya belum selesai,
                // tidak akan spawn suara baru (tidak dobel)
                if (!audioSourceFootstep.isPlaying)
                {
                    audioSourceFootstep.clip   = footstepClip;
                    audioSourceFootstep.volume = footstepVolume;
                    audioSourceFootstep.Play();
                }

                footstepTimer = footstepInterval;
            }
        }
        else
        {
            // Player diam, di udara, atau di air —
            // hentikan footstep dan reset timer
            if (audioSourceFootstep.isPlaying)
                audioSourceFootstep.Stop();

            footstepTimer = 0f;
        }
    }

    // -------------------------------------------------------
    // COLLISION — deteksi ground
    // -------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    // -------------------------------------------------------
    // TRIGGER — deteksi air
    // -------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater  = true;
            isGrounded = false;

            // Stop footstep dulu sebelum splash
            audioSourceFootstep.Stop();

            PlaySFX(splashClip, splashVolume);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
            isInWater = false;
    }

    // -------------------------------------------------------
    // PUBLIC API — dipanggil dari Player.cs
    // -------------------------------------------------------

    /// <summary>
    /// Dipanggil dari Player.cs saat Bera lompat dari tanah.
    /// </summary>
    public void PlayJump()
    {
        PlaySFX(jumpClip, jumpVolume);
    }

    /// <summary>
    /// Dipanggil dari Player.cs saat Bera melakukan air roll.
    /// </summary>
    public void PlayRoll()
    {
        PlaySFX(rollClip, rollVolume);
    }

    /// <summary>
    /// Dipanggil dari Player.cs di LateUpdate setelah ground check.
    /// </summary>
    public void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
    }

    // -------------------------------------------------------
    // HELPER
    // -------------------------------------------------------
    private void PlaySFX(AudioClip clip, float volume)
    {
        if (clip == null || audioSourceSFX == null) return;

        // Jika SFX sebelumnya masih playing, stop dulu
        // lalu langsung ganti — tidak akan dobel
        if (audioSourceSFX.isPlaying)
            audioSourceSFX.Stop();

        audioSourceSFX.clip   = clip;
        audioSourceSFX.volume = volume;
        audioSourceSFX.Play();
    }
}