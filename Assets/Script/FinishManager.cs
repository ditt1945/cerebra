// ============================================================
// FinishManager.cs
// - Tampilkan panel finish saat level selesai
// - Tombol Next Level dan Restart
// ============================================================

using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Drag FinishText ke sini.")]
    public GameObject finishText;

    [Tooltip("Drag tombol Next Level ke sini.")]
    public GameObject nextButton;

    [Tooltip("Drag tombol Restart ke sini.")]
    public GameObject restartButton;

    [Tooltip("Drag background panel finish ke sini.")]
    public GameObject finishBackground;

    // -------------------------------------------------------
    public void FinishLevel()
    {
        // Tampilkan background dulu, lalu UI
        if (finishBackground != null) finishBackground.SetActive(true);
        if (finishText       != null) finishText.SetActive(true);
        if (nextButton       != null) nextButton.SetActive(true);
        if (restartButton    != null) restartButton.SetActive(true);

        // Pause game
        Time.timeScale = 0f;

        Debug.Log("[FinishManager] Level selesai! Panel finish ditampilkan.");
    }

    // -------------------------------------------------------
    public void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("level2");
    }

    // -------------------------------------------------------
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}