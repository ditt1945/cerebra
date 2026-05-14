using UnityEngine;

public class Item : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Item diambil!");
            Destroy(gameObject);
        }
    }
}