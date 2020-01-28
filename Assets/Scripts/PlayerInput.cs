using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    
    public float jumpForce = 10f;
    bool jumpable = true;

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

    private void OnCollisionEnter(Collision collision)
    {
        jumpable = true;
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
            if(jumpable == true)
            {
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                jumpable = false;
            }
        }

    }
}
