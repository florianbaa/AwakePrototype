using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class PlayerInput : MonoBehaviour
{
    
    public float jumpForce = 10f;
    bool jumpable = true;
    public float cooldown = 0.5f;
    float timer = 0;
    public GameObject Weapon;
    public Transform Hand;

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
        animator.SetTrigger("jump_end");
    }


    void Update()
    {
        Weapon.transform.position = Hand.position + new Vector3(0.2f, -2.05f, -0.05f);

        if (timer < cooldown)
        {
            timer += Time.deltaTime;
        }
        else
        {
            if (Input.GetButton("Fire1"))
            {
                foreach (var inputReceiver in inputReceivers)
                {
                    inputReceiver.OnFireDown();
                }
                    //Weapon.transform.position = Hand.position + new Vector3(0.2f, -2.05f, -0.05f);
                    CameraShaker.Instance.ShakeOnce(4f, 4f, .1f, 1f);
                    Weapon.SetActive(true);
                    animator.SetBool("shooting", true); 
            }
            else
            {
                Weapon.SetActive(false);
                animator.SetBool("shooting", false);
            }

            timer = 0;
        }
      
        if (Input.GetButtonDown("Jump"))
        {
            if(jumpable == true)
            {
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                jumpable = false;
                animator.SetTrigger("jump");
                animator.ResetTrigger("jump_end");
            }
        }
    }
}
