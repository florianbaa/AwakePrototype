using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthBehaviour : MonoBehaviour, IDamagable
{

    public int initialHp;
    int currentHp;
    public Animator animator;
    public GameObject Gore;
    public GameObject DeathAnim;
    public GameObject SceneQuit;
    public GameObject TextKillAll;
    public GameObject TextProceed;

    public int enemyCounter;
    public static int quickEnemyCount;
    
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        currentHp = initialHp;
        SceneQuit.SetActive(false);
        TextKillAll.SetActive(true);
        TextProceed.SetActive(false);
    }
    public void Update()
    {
        if (enemyCounter >= 20)
        {
            //SceneQuit.SetActive(true);
            //TextKillAll.SetActive(false);
            //TextProceed.SetActive(true);
        }

        if (quickEnemyCount >= 20)
        {
            SceneQuit.SetActive(true);
            TextKillAll.SetActive(false);
            TextProceed.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "EndGate")
        {
            SceneManager.LoadScene(7);
        }

        if (other.gameObject.tag == "Tutorial3")
        {
            SceneManager.LoadScene(3);
        }
        if (other.gameObject.tag == "Tutorial4")
        {
            SceneManager.LoadScene(6);
        }
    }

    public void DoDamage(int amount)
    {
        currentHp -= amount;
        if (currentHp <= 0)
        {
            GameObject.Find("SceneLoad").GetComponent<Die>().DoNow();
            GameObject.Instantiate(Gore, transform.position, Quaternion.identity);
            GameObject.Instantiate(DeathAnim, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
        public IEnumerator Die()
        {
            
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene(1);
            
        }
}
