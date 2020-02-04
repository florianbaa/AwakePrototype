using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonController : MonoBehaviour, IInputReceiver
{
    public BulletPool bulletPool;
    public Transform cannonTransform;
    public Transform Weapon_Barrel;
    public int bulletLayerId;
    public float recoil;
    public GameObject MuzzleFlash;
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
        //cannonTransform.position = Weapon_Barrel.position + new Vector3(0.2f, -2.05f, -0.05f);
        BulletBehaviour newBullet = bulletPool.GetObjectFromPool();
        newBullet.transform.SetPositionAndRotation(Weapon_Barrel.position, Weapon_Barrel.rotation);

        GameObject.Instantiate(MuzzleFlash, Weapon_Barrel.position + new Vector3(0.4f,-0.8f,-0.1f), Weapon_Barrel.rotation);
        newBullet.Fire(Vector3.zero, bulletLayerId);

        newBullet.gameObject.SetActive(true);

        rb.AddRelativeForce(Vector3.forward * -recoil);
  
    }


}
