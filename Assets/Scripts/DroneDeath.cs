using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneDeath : MonoBehaviour
{
    public GameObject Effect;

    void Start()
    {
        DeathEffect();
    }

    IEnumerator DeathEffect()
    {
        yield return new WaitForSeconds(1);
        GameObject.Instantiate(Effect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void Update()
    {
        
    }
}
