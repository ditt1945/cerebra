using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    public FinishManager finishManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            finishManager.FinishLevel();
            
            if (TimerManager.Instance != null)
                TimerManager.Instance.StopTimer();
        }
    }
}