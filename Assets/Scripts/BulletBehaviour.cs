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
    public GameObject CollisionFX;


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

        else
        {
            GameObject.Instantiate(CollisionFX, transform.position, Quaternion.identity);
        }

        DisableSelf();

    }

    private void Update()
    {

        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    public void Fire(Vector3 inheritedVelocity, int bulletLayerId)
    {
        for(int i = 0; i<colliders.Length; i++)
        {
            if(colliders[i] != null)
            {
                colliders[i].gameObject.layer = bulletLayerId;
                rb.velocity = inheritedVelocity + transform.forward * initialSpeed;
            }
           
        }


        Invoke("DisableSelf", lifeTime);
    }

    void DisableSelf()
    {
        CancelInvoke("DisableSelf");
        gameObject.SetActive(false);
    }


}
