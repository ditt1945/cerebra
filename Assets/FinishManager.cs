using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject finishText;
    public GameObject nextButton;
    public GameObject restartButton;

    public void FinishLevel()
    {
        // tampilkan UI
        finishText.SetActive(true);
        nextButton.SetActive(true);
        restartButton.SetActive(true);

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

        // reload level sekarang
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}