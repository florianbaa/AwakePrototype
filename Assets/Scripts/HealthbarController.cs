﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarController : MonoBehaviour
{
    public Image healthbar;
    public float health;
    public float startHealth;

    public void onTakeDamage(int damage)
    {
        health = health - damage;
        healthbar.fillAmount = health / startHealth;
    }


}

