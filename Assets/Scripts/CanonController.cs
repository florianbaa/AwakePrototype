using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonController : MonoBehaviour, IInputReceiver
{
    public BulletPool bulletPool;
    public Transform cannonTransform;
    public int bulletLayerId;
    public float recoil;
    PlayerMovement playerMovement;
    Rigidbody rb;
    


    void Start()
    {
        rb = GetComponent<Rigidbody>();  
    }

    public void OnFireDown()
    { 
        Shoot();
    }

    void Shoot()
    {
        BulletBehaviour newBullet = bulletPool.GetObjectFromPool();
        newBullet.transform.SetPositionAndRotation(cannonTransform.position,cannonTransform.rotation);

        newBullet.Fire(Vector3.zero, bulletLayerId);

        newBullet.gameObject.SetActive(true);

        rb.AddRelativeForce(Vector3.forward * -recoil);
  
    }


}
