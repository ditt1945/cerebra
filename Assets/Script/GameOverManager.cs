// ============================================================
// GameOverManager.cs — UPDATED
// - Tetap kompatibel dengan sistem Game Over lama (jatuh ke air)
// - Ditambah Singleton agar bisa dipanggil TimerManager
// - Ditambah TriggerGameOver() untuk dipanggil dari mana saja
// - Ditambah gameOverBackground (panel hitam semi-transparan)
// ============================================================

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────
    public static GameOverManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // ──────────────────────────────────────────────────────

    [Header("UI References")]
    public GameObject gameOverBackground; // ← Panel hitam semi-transparan
    public GameObject gameOverText;       // GameOverText
    public GameObject restartButton;      // RestartButton_Go

    private bool isGameOver = false;

    // -------------------------------------------------------
    public void GameOver()
    {
        TriggerGameOver();
    }

    // -------------------------------------------------------
    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("[GameOverManager] Game Over!");

        if (TimerManager.Instance != null)
            TimerManager.Instance.StopTimer();

        // Tampilkan background hitam dulu, lalu text dan button
        if (gameOverBackground != null) gameOverBackground.SetActive(true);
        if (gameOverText       != null) gameOverText.SetActive(true);
        if (restartButton      != null) restartButton.SetActive(true);

        Time.timeScale = 0f;
    }

    // -------------------------------------------------------
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}