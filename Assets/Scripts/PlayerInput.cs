﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    
    public float jumpForce = 10f;
    bool jumpable = true;

    public Animator animator;
    IInputReceiver[] inputReceivers;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        inputReceivers = GetComponentsInChildren<IInputReceiver>();
        animator = GetComponentInChildren<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        jumpable = true;
    }


    void Update()
    {

        if (Input.GetButton("Fire1"))
        {
            foreach(var inputReceiver in inputReceivers)
            {
                inputReceiver.OnFireDown();
            }
            animator.SetBool("shooting", true);
            
        }
        if (Input.GetButtonUp("Fire1"))
        {          
            animator.SetBool("shooting", false);
        }


        if (Input.GetButtonDown("Jump"))
        {
            if(jumpable == true)
            {
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                jumpable = false;
                animator.SetTrigger("jump");
                animator.SetTrigger("jump_end");
            }
        }

    }
}
