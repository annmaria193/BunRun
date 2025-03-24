using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour
{
    public static HealthManager instance;

    public int maxLives = 3;
    private int currentLives;

    [SerializeField] private Image[] hearts;
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        // Singleton pattern to ensure a single instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        currentLives = maxLives;
        UpdateHearts();
        gameOverPanel.SetActive(false);
    }

    public void LoseLife()
    {
        if (currentLives > 0)
        {
            currentLives--;
            UpdateHearts();

            if (currentLives <= 0)
            {
                GameManager.instance.GameOver();
            }
            else
            {
                GameManager.instance.StartCoroutine(GameManager.instance.DeathCoroutine());
            }
        }
    }



    private void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < currentLives;
        }
    }

    private void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    private void RespawnPlayer()
    {
        GameManager.instance.Death(); // Reset player to starting position
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume the game
        UIManager.instance.fadeFromBlack = true; // Ensure screen fades back when restarting
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Restart the scene
    }


    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }


}
