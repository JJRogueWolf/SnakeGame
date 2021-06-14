using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagment : MonoBehaviour
{
    public void startGame()
    {
        SceneManager.LoadScene(1);
    }

    public void startThirdGame()
    {
        SceneManager.LoadScene(2);
    }

    public void landingScene()
    {
        SceneManager.LoadScene(0);
    }
}
