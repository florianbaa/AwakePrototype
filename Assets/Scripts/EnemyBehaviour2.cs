﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour2 : MonoBehaviour
{
    public GameObject player;
    public float movespeed = 20;
    public float scandistance = 15f;
    public float enemyDistance;
    public float groundDistance;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //calculates distance
        enemyDistance = Vector3.Distance(this.transform.position, player.transform.position);


        //always aims when in range
        if (enemyDistance < 20)
        {
            transform.LookAt(player.transform);
        }

        //decides if it should move and how
        if (enemyDistance < 20 && enemyDistance > 10)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, movespeed * Time.deltaTime);
        }
        else if (enemyDistance < 5)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, -movespeed * 3 * Time.deltaTime);
        }

    }

    private void OnDrawGizmos() //for shooting once implemented
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * scandistance);
    }

}