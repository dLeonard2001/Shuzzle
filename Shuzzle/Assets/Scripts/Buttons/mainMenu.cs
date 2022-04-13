using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // NEEDS WORK 
    // Bug:
        // loads scenes on top of each other
    public void startGame()
    {
        SceneManager.LoadScene("Shuzzle");
        Debug.Log(SceneManager.sceneCount);
    }

    public void controlMenu()
    {
        SceneManager.LoadScene("Controls");
        Debug.Log(SceneManager.sceneCount);
    }

    public void goBack()
    {
        SceneManager.LoadScene("MainMenu");
        Debug.Log(SceneManager.sceneCount);
    }
}
