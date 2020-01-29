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
       IDamagable damageReceiver = 
            collision.gameObject.GetComponentInParent< IDamagable > ();

        if(damageReceiver != null)
        {
            damageReceiver.DoDamage(damageAmount);
        }
        DisableSelf();
    }

    public void Fire(Vector3 inheritedVelocity, int bulletLayerId, bool lookingForward)
    {
        for(int i = 0; i<colliders.Length; i++)
        {
            colliders[i].gameObject.layer = bulletLayerId;
        }

        if (lookingForward)
        {
        rb.velocity = inheritedVelocity + transform.forward * initialSpeed;

        }
        else
        {
            rb.velocity = inheritedVelocity + (transform.forward * -1f) * initialSpeed;

        }

        Invoke("DisableSelf", lifeTime);
    }

    void DisableSelf()
    {
        CancelInvoke("DisableSelf");
        gameObject.SetActive(false);
    }


}
