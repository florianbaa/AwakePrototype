using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Range (1, 10)]
    public float jumpVelocity = 5f;

    IInputReceiver[] inputReceivers;
    Rigidbody rb;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        inputReceivers = GetComponentsInChildren<IInputReceiver>();
    }


    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            foreach(var inputReceiver in inputReceivers)
            {
                inputReceiver.OnFireDown();
            }
        }

        if(Input.GetButtonDown("Jump"))
        {
            rb.velocity = Vector3.up * jumpVelocity;
        }

    }
}
