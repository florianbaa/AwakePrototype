using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartControl : MonoBehaviour
{
    public void ButtonRestart()
    {
        SceneManager.LoadScene(1);
    }

    public void ButtonMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }
}
