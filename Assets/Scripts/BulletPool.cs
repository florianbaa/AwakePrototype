using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public BulletBehaviour prefab;
    public int initialPoolAmount = 10;
    List<BulletBehaviour> objectlist;

    private void Awake()
    {
        objectlist = new List<BulletBehaviour>();
        for(int i = 0; i < initialPoolAmount; i++)
        {
            AddObjectToPool();
        }
    }

    BulletBehaviour AddObjectToPool()
    {
        BulletBehaviour newBullet = Instantiate(prefab);
        newBullet.gameObject.SetActive(false);
        objectlist.Add(newBullet);
        return newBullet;
    }

    public BulletBehaviour GetObjectFromPool()
    {
        foreach(BulletBehaviour currentObject in objectlist)
        {

            if (!currentObject.gameObject.activeSelf)
            {
                return currentObject;
            }
        }

        return AddObjectToPool();
    }
}
