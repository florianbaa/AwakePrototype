using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public GameObject player;
    public GameObject explosion;
    public float movespeed = 20;
    public float scandistance = 15f;
    float enemyDistance;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        enemyDistance = Vector3.Distance(GameObject.FindWithTag("enemy").transform.position, GameObject.FindWithTag("Player").transform.position); ;
        if (enemyDistance < 20 && enemyDistance > 10)
        {
            transform.LookAt(player.transform);
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, movespeed * Time.deltaTime);
        }
        else if (enemyDistance < 7)
        {
                        transform.LookAt(player.transform);
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, -movespeed * Time.deltaTime);
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * scandistance);
    }

}
