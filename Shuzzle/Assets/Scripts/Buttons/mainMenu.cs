using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    // NEEDS WORK 
    // Bug:
        // loads scenes on top of each other
    public void startGame()
    {
        SceneManager.LoadSceneAsync("Shuzzle");
        // SceneManager.LoadScene("Shuzzle");
    }
    public void goBack()
    {
        SceneManager.LoadSceneAsync("MainMenu");
        Debug.Log(SceneManager.sceneCount);
    }

    public void closeGame()
    {
        Application.Quit();
    }
}
