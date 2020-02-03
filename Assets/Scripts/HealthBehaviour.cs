using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBehaviour : MonoBehaviour, IDamagable
{

    public int initialHp;
    int currentHp;
    public Animator animator;

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
            animator.SetTrigger("death");            
        }
    }
}
