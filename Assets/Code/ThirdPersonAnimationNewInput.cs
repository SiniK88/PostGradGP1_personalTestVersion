using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonAnimationNewInput : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private float maxSpeed = 5f;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("InputMagnitude", rb.velocity.magnitude / maxSpeed);
    }


}
