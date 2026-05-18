using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject finishText;
    public GameObject nextButton;
    public GameObject restartButton;
    public GameObject finishBackground; // ← aktifkan saat finish

    public void FinishLevel()
    {
        // tampilkan background dulu, lalu UI
        if (finishBackground != null) finishBackground.SetActive(true);
        if (finishText       != null) finishText.SetActive(true);
        if (nextButton       != null) nextButton.SetActive(true);
        if (restartButton    != null) restartButton.SetActive(true);

        // pause game
        Time.timeScale = 0f;
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("level2");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}