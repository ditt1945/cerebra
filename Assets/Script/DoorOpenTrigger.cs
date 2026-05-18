using UnityEngine;

public class DoorOpenTrigger : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("PLAYER DEKAT PINTU");

            if (doorAnimator != null)
            {
                doorAnimator.SetBool("Open", true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("PLAYER MENJAUH");

            if (doorAnimator != null)
            {
                doorAnimator.SetBool("Open", false);
            }
        }
    }
}