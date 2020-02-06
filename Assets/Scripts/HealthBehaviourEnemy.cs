using System.Collections;
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
        HealthBehaviour.quickEnemyCount = HealthBehaviour.quickEnemyCount + 1;
        Debug.Log("QEC: " + HealthBehaviour.quickEnemyCount);

        currentHp -= amount;
        if (currentHp <= 0)
        {
            GameObject.Find("Player").GetComponent<HealthBehaviour>().enemyCounter += 1;
            Die();
        }

        void Die()
        {
            GameObject.Instantiate(DeathAnimEnemy, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
