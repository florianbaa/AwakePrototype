using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float hInput;
    public bool lookingForward = true;
     

    public string inputAxisName = "";
    public float speed = 15f;

    Rigidbody rb;
    public Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    
    private void Update()
    {
        hInput = Input.GetAxis("Horizontal");
        if(hInput <= 1f && hInput > 0)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if( hInput >= -1 && hInput < 0)
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);           
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector3.right * speed * Input.GetAxis("Horizontal") + Vector3.up * rb.velocity.y;      
          
    }
}

