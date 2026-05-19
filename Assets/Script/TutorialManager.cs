// ============================================================
// TutorialManager.cs  (versi zone-based + auto-hide)
//
// Cara kerja:
//   - Pesan pertama muncul otomatis saat level mulai.
//   - Pesan otomatis hilang setelah autoHideDuration detik.
//   - Pesan juga bisa berganti/hilang saat TutorialZone
//     memanggil ShowMessage(index) atau HideMessage().
//
// Dipasang di: GameObject "TutorialManager"
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    // -------------------------------------------------------
    // SINGLETON
    // -------------------------------------------------------
    public static TutorialManager Instance { get; private set; }

    // -------------------------------------------------------
    // INSPECTOR FIELDS
    // -------------------------------------------------------
    [Header("Referensi Objek")]
    [Tooltip("Drag GameObject Bera (Player) ke sini")]
    [SerializeField] private Transform player;

    [Tooltip("Drag TutorialCanvas (World Space Canvas) ke sini")]
    [SerializeField] private Canvas worldCanvas;

    [Tooltip("Drag TutorialTeks (TextMeshProUGUI) ke sini")]
    [SerializeField] private TextMeshProUGUI tutorialText;

    [Header("Posisi Bubble di Atas Kepala Player")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2.8f, 0f);

    [Header("Daftar Pesan Tutorial")]
    [Tooltip("Index 0 tampil otomatis saat spawn.")]
    [SerializeField] private List<string> messages = new List<string>
    {
        "Gunakan A / D atau \u2190 \u2192 untuk bergerak",
        "Tekan Space untuk melompat",
        "Kumpulkan semua buah untuk membuka finish!",
    };

    [Header("Timing")]
    [Tooltip("Jeda sebelum pesan pertama muncul setelah spawn (detik)")]
    [SerializeField] private float initialDelay = 1.0f;

    [Tooltip("Durasi fade in / fade out (detik)")]
    [SerializeField] private float fadeDuration = 0.35f;

    [Tooltip("Durasi pesan tampil sebelum otomatis hilang (detik). 0 = tidak auto-hide)")]
    [SerializeField] private float autoHideDuration = 3f;

    [Header("Tampilan")]
    [Tooltip("Canvas selalu menghadap kamera")]
    [SerializeField] private bool billboardMode = true;

    // -------------------------------------------------------
    // PRIVATE STATE
    // -------------------------------------------------------
    private CanvasGroup canvasGroup;
    private Camera      mainCamera;
    private Coroutine   fadeRoutine;
    private Coroutine   autoHideRoutine;
    private int         currentIndex = -1;
    private bool        isVisible    = false;

    // -------------------------------------------------------
    // UNITY LIFECYCLE
    // -------------------------------------------------------
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        canvasGroup = worldCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = worldCanvas.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        worldCanvas.gameObject.SetActive(false);
        mainCamera = Camera.main;
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // Tampilkan pesan index 0 otomatis setelah delay
        if (messages.Count > 0)
            StartCoroutine(ShowAfterDelay(0, initialDelay));
    }

    private void LateUpdate()
    {
        if (player == null || worldCanvas == null) return;

        // Ikuti posisi player
        worldCanvas.transform.position = player.position + offset;

        // Hadap kamera
        if (billboardMode && mainCamera != null)
        {
            worldCanvas.transform.rotation = Quaternion.LookRotation(
                worldCanvas.transform.position - mainCamera.transform.position
            );
        }
    }

    // -------------------------------------------------------
    // API PUBLIK
    // -------------------------------------------------------
    public void ShowMessage(int index)
    {
        if (index < 0 || index >= messages.Count)
        {
            HideMessage();
            return;
        }

        if (index == currentIndex && isVisible) return;

        currentIndex      = index;
        tutorialText.text = messages[index];
        FadeIn();
        StartAutoHide();
    }

    public void ShowCustomMessage(string message)
    {
        currentIndex      = -1;
        tutorialText.text = message;
        FadeIn();
        StartAutoHide();
    }

    public void HideMessage()
    {
        if (!isVisible) return;
        StopAutoHide();
        FadeOut();
    }

    // -------------------------------------------------------
    // AUTO-HIDE
    // -------------------------------------------------------
    private void StartAutoHide()
    {
        StopAutoHide();
        if (autoHideDuration > 0f)
            autoHideRoutine = StartCoroutine(AutoHideRoutine());
    }

    private void StopAutoHide()
    {
        if (autoHideRoutine != null)
        {
            StopCoroutine(autoHideRoutine);
            autoHideRoutine = null;
        }
    }

    private IEnumerator AutoHideRoutine()
    {
        yield return new WaitForSeconds(autoHideDuration);
        HideMessage();
    }

    // -------------------------------------------------------
    // INTERNAL
    // -------------------------------------------------------
    private void FadeIn()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        worldCanvas.gameObject.SetActive(true);
        fadeRoutine = StartCoroutine(FadeTo(1f));
        isVisible   = true;
    }

    private void FadeOut()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeOutRoutine());
        isVisible   = false;
    }

    private IEnumerator FadeTo(float target)
    {
        float start   = canvasGroup.alpha;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed          += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = target;
    }

    private IEnumerator FadeOutRoutine()
    {
        yield return FadeTo(0f);
        worldCanvas.gameObject.SetActive(false);
    }

    private IEnumerator ShowAfterDelay(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowMessage(index);
    }
}