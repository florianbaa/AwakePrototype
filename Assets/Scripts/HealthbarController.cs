using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarController : MonoBehaviour
{
    public Image healthbar;
    public float health;
    public float startHealth;
    public Sprite h100;
    public Sprite h90;
    public Sprite h80;
    public Sprite h70;
    public Sprite h60;
    public Sprite h50;
    public Sprite h40;
    public Sprite h30;
    public Sprite h20;
    public Sprite h10;
    public Sprite h0;

    public void onTakeDamage(int damage)
    {
        health = health - damage;
       
        if(health == 100)
        {
            healthbar.sprite = h100;
        }
        else if (health == 90)
        {
            healthbar.sprite = h90;
        }
        else if (health == 80)
        {
            healthbar.sprite = h80;
        }
        else if (health == 70)
        {
            healthbar.sprite = h70;
        }
        else if (health == 60)
        {
            healthbar.sprite = h60;
        }
        else if (health == 50)
        {
            healthbar.sprite = h50;
        }
        else if (health == 40)
        {
            healthbar.sprite = h40;
        }
        else if (health == 30)
        {
            healthbar.sprite = h30;
        }
        else if (health == 20)
        {
            healthbar.sprite = h20;
        }
        else if (health == 10)
        {
            healthbar.sprite = h10;
        }
        else if (health == 0)
        {
            healthbar.sprite = h0;
        }
    }
}


