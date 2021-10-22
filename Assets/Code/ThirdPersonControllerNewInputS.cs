using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonControllerNewInputS : MonoBehaviour
{

    private ThirdpersonPlayerControllerActions playerActionAssets;
    private InputAction move;

    private Rigidbody rb;
    CapsuleCollider _capsuleCollider;
    private float colliderRadius, colliderHeight;
    private bool sneaking = false;

    [SerializeField] float movementForce = 1f;

    [SerializeField] float jumpForce = 5f;

    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float sneakmaxSpeed = 2.5f;

    [SerializeField] float sneakSpeed = 0.5f;
    [SerializeField] float fallMultiplayer = 2f;
    [SerializeField] float rotationSpeed = 2f;


    private Vector3 forceDirection = Vector3.zero;
    private Animator animator;

    [SerializeField] Camera playerCamera;

    private void Awake() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
        animator = this.GetComponent<Animator>();

        rb = this.GetComponent<Rigidbody>();
        playerActionAssets = new ThirdpersonPlayerControllerActions();

        _capsuleCollider = GetComponent<CapsuleCollider>();

        colliderHeight = GetComponent<CapsuleCollider>().height;
    }

    private void OnEnable() {
        playerActionAssets.Player.Jump.started += DoJump;
        playerActionAssets.Player.Sneak.started += Crouching;
        playerActionAssets.Player.Sneak.canceled += NoCrouching;
        move = playerActionAssets.Player.Move;
        playerActionAssets.Player.Enable();
    }

    private void OnDisable() {
        playerActionAssets.Player.Jump.started -= DoJump;
        playerActionAssets.Player.Sneak.started -= Crouching;
        playerActionAssets.Player.Disable();
    }

    private void FixedUpdate() {
            forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * movementForce;
            forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * movementForce;


        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        if (rb.velocity.y < 0f)
        {
           rb.velocity += Vector3.down  * fallMultiplayer * Time.fixedDeltaTime;
        }

        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (sneaking == false) {
            if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed) {
                rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;
            }
        } else {
            if (horizontalVelocity.sqrMagnitude > sneakmaxSpeed * sneakmaxSpeed) {
                rb.velocity = horizontalVelocity.normalized * sneakmaxSpeed + Vector3.up * rb.velocity.y;
            }
        }

        //LookAt();
        PlayerRotation();
    }

    private Vector3 GetCameraRight(Camera playerCamera) {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }

    private Vector3 GetCameraForward(Camera playerCamera) {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private void DoJump(InputAction.CallbackContext obj) {


        if (IsGrounded()) {
            Vector3 horizontalVelocity = rb.velocity;
            horizontalVelocity.y = 0;
            if (horizontalVelocity.sqrMagnitude < 0.1f)
                animator.CrossFadeInFixedTime("Jump", 0.0f);
            else
                animator.CrossFadeInFixedTime("JumpMove", .1f);

            forceDirection += Vector3.up * jumpForce;
        }
    }

    private bool IsGrounded() {
        var radius = _capsuleCollider.radius * 0.9f;
        var dist = 10f;
        Ray ray = new Ray(this.transform.position + new Vector3(0, colliderHeight / 2, 0), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, (colliderHeight / 2) + dist)) {

            dist = transform.position.y - hit.point.y;
            return true;
        } else {
            return false;
        }
    }

    void LookAt() {
        Vector3 direction = rb.velocity;
        direction.y = 0f;

        if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f) {
            this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        } else rb.angularVelocity = Vector3.zero;
    }

    public void PlayerRotation() {
        var movementDir = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if(movementDir != Vector3.zero) {
            Quaternion toRotation = Quaternion.LookRotation(movementDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }


    private void Crouching(InputAction.CallbackContext obj) {

        animator.SetInteger("MovementState", 1);
        sneaking = true;
    }
    private void NoCrouching(InputAction.CallbackContext obj) {

        animator.SetInteger("MovementState", 0);
        sneaking = false;
    }

} // class
