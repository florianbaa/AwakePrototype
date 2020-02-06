using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{

   public void ButtonStart()
    {
        SceneManager.LoadScene(2);
    }

    public void ButtonCredit()
    {
        SceneManager.LoadScene(8);
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }
 
}
