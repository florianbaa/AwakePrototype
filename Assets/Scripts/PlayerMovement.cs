using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{


    public string inputAxisName = "";
    public float speed = 15f;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
       rb.velocity = Vector3.right * speed * Input.GetAxis("Horizontal") + Vector3.up * rb.velocity.y;
      
    }
}

