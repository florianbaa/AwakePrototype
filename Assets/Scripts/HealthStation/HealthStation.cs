using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStation : MonoBehaviour
{
    bool healable = true;
    public GameObject HealableLight;
    public GameObject NonHealableLight;

    // Start is called before the first frame update
    void Start()
    {
        HealableLight.SetActive(false);
        NonHealableLight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (healable = true)
        {
            HealableLight.SetActive(true);
            NonHealableLight.SetActive(false);
        }
        else
        {
            HealableLight.SetActive(false);
            NonHealableLight.SetActive(true);
        }
    }
}
