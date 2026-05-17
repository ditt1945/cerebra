// ============================================================
// TutorialZoneHighlight.cs
//
// Efek glow berdenyut menggunakan LineRenderer.
// Lebih reliable dibanding SpriteRenderer prosedural karena
// LineRenderer adalah built-in Unity yang selalu ter-render.
//
// Cara pasang:
//   1. Pasang script ini di GameObject TutorialZone yang SAMA
//      dengan TutorialZone.cs.
//   2. Atur warna dan kecepatan di Inspector.
//   3. Tidak perlu material tambahan — menggunakan
//      Default-Line material bawaan Unity.
//
// Dipasang di: GameObject TutorialZone (sama dengan TutorialZone.cs)
// ============================================================

using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TutorialZoneHighlight : MonoBehaviour
{
    // -------------------------------------------------------
    // INSPECTOR FIELDS
    // -------------------------------------------------------
    [Header("Warna Glow")]
    [Tooltip("Warna glow saat paling terang.")]
    [SerializeField] private Color glowColor = new Color(1f, 0.92f, 0.2f, 1f);

    [Tooltip("Warna glow saat paling redup.")]
    [SerializeField] private Color dimColor = new Color(1f, 0.92f, 0.2f, 0.05f);

    [Header("Kecepatan")]
    [Tooltip("Kecepatan denyut. Rekomendasi: 1.0 - 2.5")]
    [SerializeField] private float pulseSpeed = 1.5f;

    [Header("Fade Out")]
    [Tooltip("Kecepatan fade out saat Bera masuk zone.")]
    [SerializeField] private float fadeOutSpeed = 3f;

    [Header("Tampilan")]
    [Tooltip("Ketebalan border glow dalam world units.")]
    [SerializeField] private float lineWidth = 0.15f;

    [Tooltip("Padding border di luar collider.")]
    [SerializeField] private float borderPadding = 0.2f;

    // -------------------------------------------------------
    // PRIVATE
    // -------------------------------------------------------
    private LineRenderer lineRenderer;
    private BoxCollider2D boxCollider;

    private enum GlowState { Pulsing, FadingOut, Hidden }
    private GlowState state = GlowState.Pulsing;

    private float currentAlpha = 0f;

    // -------------------------------------------------------
    // UNITY LIFECYCLE
    // -------------------------------------------------------
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        SetupLineRenderer();
    }

    private void Update()
    {
        if (lineRenderer == null) return;

        switch (state)
        {
            case GlowState.Pulsing:   AnimatePulse();   break;
            case GlowState.FadingOut: AnimateFadeOut(); break;
            case GlowState.Hidden:                      break;
        }
    }

    // -------------------------------------------------------
    // SETUP
    // -------------------------------------------------------
    private void SetupLineRenderer()
    {
        // Tambah LineRenderer ke GameObject ini
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        // Gunakan material default Unity — selalu ada, tidak perlu import
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Lebar garis
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth   = lineWidth;

        // Loop tertutup (kotak)
        lineRenderer.loop = true;

        // Tidak terpengaruh skala object
        lineRenderer.useWorldSpace = false;

        // Sorting — di bawah player
        lineRenderer.sortingOrder = -1;

        // Buat 4 titik sudut kotak berdasarkan ukuran collider
        BuildBoxPoints();

        // Warna awal
        SetLineColor(dimColor);
    }

    private void BuildBoxPoints()
    {
        Vector2 size   = boxCollider.size;
        Vector2 offset = boxCollider.offset;
        float   p      = borderPadding;

        float left   = offset.x - size.x / 2f - p;
        float right  = offset.x + size.x / 2f + p;
        float bottom = offset.y - size.y / 2f - p;
        float top    = offset.y + size.y / 2f + p;

        lineRenderer.positionCount = 4;
        lineRenderer.SetPosition(0, new Vector3(left,  bottom, 0));
        lineRenderer.SetPosition(1, new Vector3(right, bottom, 0));
        lineRenderer.SetPosition(2, new Vector3(right, top,    0));
        lineRenderer.SetPosition(3, new Vector3(left,  top,    0));
    }

    // -------------------------------------------------------
    // ANIMASI
    // -------------------------------------------------------
    private void AnimatePulse()
    {
        float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        t = Mathf.SmoothStep(0f, 1f, t);

        currentAlpha = Mathf.Lerp(dimColor.a, glowColor.a, t);
        Color c = Color.Lerp(dimColor, glowColor, t);
        c.a = currentAlpha;
        SetLineColor(c);
    }

    private void AnimateFadeOut()
    {
        currentAlpha = Mathf.MoveTowards(currentAlpha, 0f, fadeOutSpeed * Time.deltaTime);

        Color c = lineRenderer.startColor;
        c.a = currentAlpha;
        SetLineColor(c);

        if (currentAlpha <= 0f)
        {
            state = GlowState.Hidden;
            lineRenderer.enabled = false;
        }
    }

    private void SetLineColor(Color c)
    {
        lineRenderer.startColor = c;
        lineRenderer.endColor   = c;
    }

    // -------------------------------------------------------
    // PUBLIC API
    // -------------------------------------------------------

    /// <summary>
    /// Dipanggil dari TutorialZone saat Bera masuk zone.
    /// Glow fade out lalu hilang.
    /// </summary>
    public void HideGlow()
    {
        if (state == GlowState.Hidden || state == GlowState.FadingOut) return;
        state = GlowState.FadingOut;
    }
}