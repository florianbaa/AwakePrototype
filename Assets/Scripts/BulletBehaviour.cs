using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class BulletBehaviour : MonoBehaviour
{
    Rigidbody rb;
    Collider[] colliders;

    public float initialSpeed = 10f;
    public float lifeTime = 3f;
    public int damageAmount = 1;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
       IDamagable damageReveiver = 
            collision.gameObject.GetComponentInParent< IDamagable > ();

        if(damageReveiver != null)
        {
            damageReveiver.DoDamage(damageAmount);
        }
        DisableSelf();
    }

    public void Fire(Vector3 inheritedVelocity, int bulletLayerId)
    {
        for(int i = 0; i<colliders.Length; i++)
        {
            colliders[i].gameObject.layer = bulletLayerId;
        }

        rb.velocity = inheritedVelocity + transform.forward * initialSpeed;

        Invoke("DisableSelf", lifeTime);
    }

    void DisableSelf()
    {
        CancelInvoke("DisableSelf");
        gameObject.SetActive(false);
    }


}
