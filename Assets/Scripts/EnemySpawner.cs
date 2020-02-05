using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    public float xPos;
    public float yPos;
    public int counter;
    
    private void Start()
    {
        StartCoroutine(EnemySpawn());
    }


    IEnumerator EnemySpawn()
    {
        while (counter < 100)
        {
            xPos = Random.Range(50, 210);
            yPos = Random.Range(10, 25);
            Instantiate(enemy, new Vector3(xPos, yPos, 0), Quaternion.identity);
            yield return new WaitForSeconds(5);
            counter += 1;
        }
    }




}
