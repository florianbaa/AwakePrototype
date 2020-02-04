﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBehaviourEnemy : MonoBehaviour, IDamagable
{

    public int initialHp;
    int currentHp;
    public Animator animator;
    public GameObject DeathAnimEnemy;


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        currentHp = initialHp;
    }


    public void DoDamage(int amount)
    {
        currentHp -= amount;
        if (currentHp <= 0)
        {
            Die();
        }

        void Die()
        {
            GameObject.Instantiate(DeathAnimEnemy, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}