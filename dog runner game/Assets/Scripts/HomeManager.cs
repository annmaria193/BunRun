using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    public void LoadGamePlayScene()
    {
        SceneManager.LoadScene(2);
        GameManager.instance.playerController.gameObject.SetActive(true);  // Activate player on game start
    }
    public void HomeInfo()
    {

    }
    public void Quit()
    {
        Application.Quit();
    }

    

}
