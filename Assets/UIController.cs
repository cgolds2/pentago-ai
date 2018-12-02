using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PentagoAICrossP;

public class UIController : MonoBehaviour
{

    public void RestartGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlayerScene()
    {
        SceneManager.LoadScene("PlayerVsPlayer");
    }

    public void AIScene()
    {
        SceneManager.LoadScene("PlayerVsAI");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
