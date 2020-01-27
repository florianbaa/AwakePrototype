using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    IInputReceiver[] inputReceivers;

    private void OnEnable()
    {
        inputReceivers = GetComponentsInChildren<IInputReceiver>();
    }


    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            foreach(var inputReceiver in inputReceivers)
            {
                inputReceiver.OnFireDown();
            }
        }   
    }
}
