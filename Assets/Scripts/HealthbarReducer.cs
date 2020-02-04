using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarReducer : MonoBehaviour
{
    public HealthbarController healthbar;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "EnemieBullet")
        {

            if (healthbar)
            {
                healthbar.onTakeDamage(10);
            }
        }
    }


}
