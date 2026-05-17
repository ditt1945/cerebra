// ============================================================
// TutorialZone.cs
//
// Cara kerja:
//   Pasang script ini pada sebuah GameObject dengan Collider2D
//   (centang isTrigger). Atur di Inspector:
//     - messageIndex : pesan mana (dari daftar di TutorialManager)
//                      yang tampil saat Bera masuk zone ini.
//     - hideOnExit   : apakah pesan hilang saat Bera keluar zone.
//     - triggerOnce  : zone hanya aktif sekali (tidak bisa ulang).
//
// Proteksi spawn:
//   Collider dinonaktifkan saat Awake, lalu diaktifkan kembali
//   setelah triggerDelay detik. Ini mencegah OnTriggerEnter2D
//   terpanggil jika Bera spawn di dalam atau sangat dekat zone.
//
// Dipasang di: setiap trigger zone di scene (objek terpisah)
// ============================================================

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TutorialZone : MonoBehaviour
{
    // -------------------------------------------------------
    // INSPECTOR FIELDS
    // -------------------------------------------------------
    [Header("Pesan yang Ditampilkan")]
    [Tooltip("Index pesan dari daftar 'messages' di TutorialManager.\n" +
             "0 = pesan pertama, 1 = kedua, dst.\n" +
             "Set ke -1 untuk tidak menampilkan pesan (hanya hide).")]
    [SerializeField] private int messageIndex = 1;

    [Header("Behaviour")]
    [Tooltip("Jika true: pesan hilang saat Bera keluar zone ini.")]
    [SerializeField] private bool hideOnExit = false;

    [Tooltip("Jika true: zone hanya trigger satu kali dan tidak bisa aktif lagi.")]
    [SerializeField] private bool triggerOnce = true;

    [Tooltip("Waktu tunggu (detik) sebelum collider diaktifkan.\n" +
             "Collider benar-benar dimatikan selama waktu ini sehingga\n" +
             "OnTriggerEnter2D tidak mungkin terpanggil.\n" +
             "Rekomendasi: 0.5")]
    [SerializeField] private float triggerDelay = 0.5f;

    [Header("Debug")]
    [Tooltip("Warna Gizmo di Scene view agar zone mudah dilihat.")]
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 0.8f, 0.25f);

    // -------------------------------------------------------
    // PRIVATE STATE
    // -------------------------------------------------------
    private bool hasTriggered = false;
    private Collider2D col;
    private TutorialZoneHighlight highlight;

    // -------------------------------------------------------
    // UNITY LIFECYCLE
    // -------------------------------------------------------
    private void Awake()
    {
        col           = GetComponent<Collider2D>();
        col.isTrigger = true;

        // Matikan collider sejak awal — tidak ada trigger yang bisa masuk
        col.enabled = false;

        highlight = GetComponent<TutorialZoneHighlight>();
    }

    private void Start()
    {
        // Aktifkan collider setelah delay
        // Bera sudah pasti sudah keluar dari posisi awal saat ini
        Invoke(nameof(EnableCollider), triggerDelay);
    }

    private void EnableCollider()
    {
        col.enabled = true;
    }

    // -------------------------------------------------------
    // TRIGGER EVENTS
    // -------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggerOnce && hasTriggered) return;

        hasTriggered = true;

        if (messageIndex >= 0)
            TutorialManager.Instance?.ShowMessage(messageIndex);
        else
            TutorialManager.Instance?.HideMessage();

        // Glow fade out saat Bera benar-benar masuk zone
        highlight?.HideGlow();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!hideOnExit) return;

        TutorialManager.Instance?.HideMessage();
    }

    // -------------------------------------------------------
    // GIZMO
    // -------------------------------------------------------
    private void OnDrawGizmos()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c == null) return;

        // Merah = collider belum aktif, warna normal = sudah aktif
        Gizmos.color = (col != null && !col.enabled)
            ? new Color(1f, 0f, 0f, 0.15f)
            : gizmoColor;

        if (c is BoxCollider2D box)
        {
            Vector3 center = transform.position + (Vector3)box.offset;
            Vector3 size = new Vector3(
                box.size.x * transform.lossyScale.x,
                box.size.y * transform.lossyScale.y,
                0.1f
            );
            Gizmos.DrawCube(center, size);

            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.8f);
            Gizmos.DrawWireCube(center, size);
        }

#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 1.2f,
            (col != null && !col.enabled)
                ? $"Zone → msg [{messageIndex}] (waiting...)"
                : $"Zone → msg [{messageIndex}]"
        );
#endif
    }
}