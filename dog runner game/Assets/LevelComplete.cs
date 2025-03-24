using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    public GameObject levelCompletePanel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            levelCompletePanel.SetActive(true);
            Time.timeScale = 0f;  // Pause the game if needed
        }
    }
}
