using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagment : MonoBehaviour
{
    public void startGame()
    {
        // Load 2d Game
        SceneManager.LoadScene(1);
    }

    public void startThirdGame()
    {
        //Load 3d game
        SceneManager.LoadScene(2);
    }

    public void landingScene()
    {
        //Load landing scene
        SceneManager.LoadScene(0);
    }
}
