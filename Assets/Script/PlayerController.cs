using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float strafeSpeed;
    public float jumpForce;

    public Rigidbody hips;
    public bool isGrounded;

    public Animator anim;


    private void Start()
    {
        hips = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if(hips == null) return;
        if(Input.GetKey(KeyCode.W))
        {
            if(Input.GetKey (KeyCode.LeftShift))
            {
                hips.AddForce(hips.transform.forward * speed * 1.5f);
                anim.SetBool("IsWalk", true);
            }
            else
            {
                hips.AddForce(hips.transform.forward * speed);
                anim.SetBool("IsWalk", true);
                anim.speed = 1;
            }

            
        }
        else
        {
            anim.SetBool("IsWalk", false);
        }

        if(Input.GetKey(KeyCode.A))
        {
            hips.AddForce(-hips.transform.right * speed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            hips.AddForce(hips.transform.right * speed);
        }

        if (Input.GetKey(KeyCode.S))
        {
            hips.AddForce(-hips.transform.forward * speed);
        }

        if(Input.GetAxis("Jump") > 0)
        {
            if(isGrounded)
            {
                hips.AddForce(new Vector3(0, jumpForce, 0));
                isGrounded = false;
            }
        }
    }
}
