using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    [SerializeField] private TMP_Text coinText;

    [SerializeField] public PlayerController playerController;

    private int coinCount = 0;
    private int gemCount = 0;
    private bool isGameOver = false;
    private Vector3 playerPosition;

    //Level Complete

    [SerializeField] GameObject levelCompletePanel;
    [SerializeField] TMP_Text leveCompletePanelTitle;
    [SerializeField] TMP_Text levelCompleteCoins;





    private int totalCoins = 0;




    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        playerController.gameObject.SetActive(true);  // Ensure player is active on game start
        UIManager.instance.ResetFade();
        UpdateGUI();
        UIManager.instance.fadeFromBlack = true;
        playerPosition = playerController.transform.position;

        FindTotalPickups();
    }

    public void IncrementCoinCount()
    {
        coinCount++;
        UpdateGUI();
    }
    public void IncrementGemCount()
    {
        gemCount++;
        UpdateGUI();
    }

    private void UpdateGUI()
    {
        coinText.text = coinCount.ToString();

    }

    public void Death()
    {
        if (!isGameOver)
        {
            // Initiate screen fade
            UIManager.instance.fadeToBlack = true;

            // Disable the player object
            playerController.gameObject.SetActive(false);

            // Update health before proceeding with death logic
            HealthManager.instance.LoseLife();
        }
    }

    public void GameOver()
    {
        isGameOver = true;

        // Initiate screen fade
        UIManager.instance.fadeToBlack = true;

        // Start a coroutine to show the Game Over panel after fade
        StartCoroutine(GameOverCoroutine());
    }

    private IEnumerator GameOverCoroutine()
    {
        // Wait for the fade effect to complete (adjust the time as needed)
        yield return new WaitForSeconds(1f);

        // Show the Game Over panel after fading to black
        HealthManager.instance.ShowGameOverPanel();

        // Start fading from black after showing the Game Over panel
        UIManager.instance.fadeFromBlack = true;
    }



    public void FindTotalPickups()
    {

        pickup[] pickups = GameObject.FindObjectsOfType<pickup>();

        foreach (pickup pickupObject in pickups)
        {
            if (pickupObject.pt == pickup.pickupType.coin)
            {
                totalCoins += 1;
            }

        }



    }
    public void LevelComplete()
    {



        levelCompletePanel.SetActive(true);
        leveCompletePanelTitle.text = "LEVEL COMPLETE";



        levelCompleteCoins.text = "COINS COLLECTED: " + coinCount.ToString() + " / " + totalCoins.ToString();

    }

    public IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(1f);

        // Respawn only if the game is not over
        if (!isGameOver)
        {
            playerController.transform.position = playerPosition;

            // Reactivate the player object
            playerController.gameObject.SetActive(true);

            // Fade back from black after respawn
            UIManager.instance.fadeFromBlack = true;
        }
        else
        {
            // If game over, reload the scene or show the GameOverPanel
            HealthManager.instance.ShowGameOverPanel();
            playerController.gameObject.SetActive(true);
        }
    }


}