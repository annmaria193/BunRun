using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(3);
        GameManager.instance.playerController.gameObject.SetActive(true);  // Activate player on game start
    }
    public void GamePlayScene()
    {
        Debug.Log("Retry Button Clicked!");
        Time.timeScale = 1f; // Unpause the game if it was paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the current scene
    }
    public void LoadHomeback()
    {
        SceneManager.LoadScene(1);
    }


}
