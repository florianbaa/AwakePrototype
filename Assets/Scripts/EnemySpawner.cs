using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    public float xPos;
    public float yPos;
    public int counter;
/*
    private void Start()
    {
        StartCoroutine(EnemySpawn());
    }


    IEnumerator EnemySpawn()
    {
        xPos = 50;
        yPos = 25;
        while (counter < 50)
        { 
            Instantiate(enemy, new Vector3(xPos, yPos, 0), Quaternion.identity);
            yield return new WaitForSeconds(1);
            counter += 1;
            xPos += 3;
        }
    }

    */


}
