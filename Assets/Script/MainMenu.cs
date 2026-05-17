using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void MulaiGame()
    {
        SceneManager.LoadScene("gameplay");
    }

    public void KeluarGame()
    {
        Application.Quit();
        Debug.Log("Game Keluar");
    }
}