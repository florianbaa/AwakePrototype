using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Die : MonoBehaviour
{
    public IEnumerator LoadScene()
    {

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(2);

    }

    public void DoNow()
    {
        StartCoroutine(LoadScene());
    }
}
