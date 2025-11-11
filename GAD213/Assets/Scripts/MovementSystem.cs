using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    public float moveSpeed;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;
    public float playerHeight;
    public LayerMask ground;
    bool grounded;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    public KeyCode jumpKey = KeyCode.Space;
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    public Sliding playerSlide;
    public KeyCode slamKey = KeyCode.F;
    public float slamForce = 50f;           
    public float slamGravityMultiplier = 5f;
    public bool isSlamming;
    public int maxJumps = 2; 
    private int jumpsLeft;
    public float baseSpeed;
    private Sliding slide;
    public float slideJumpBoost = 1.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        slide = GetComponent<Sliding>();
        rb.freezeRotation = true;
        baseSpeed = moveSpeed;
        readyToJump = true;
        jumpsLeft = maxJumps;
    }

    void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);
        //movement function
        MovementInput();
        SpeedControl();
        //handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else 
        {
            rb.drag = 0;
            rb.AddForce(Vector3.down * 4f, ForceMode.Force);
        }
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);


        rb.drag = grounded ? groundDrag : 0f;

        //stop the slam when we hit the ground
        if (isSlamming && grounded)
        {
            isSlamming = false;
            
        }

        //reset jumps when grounded
        if (grounded && jumpsLeft < maxJumps)
            jumpsLeft = maxJumps;

    }

    private void FixedUpdate()
    {
        MovePlayer();

        //handle ground slam force in physics step
        if (isSlamming && !grounded)
        {
            rb.AddForce(Vector3.down * slamForce * slamGravityMultiplier, ForceMode.Acceleration);
        }
    }

    private void MovementInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        { 
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Ground Slam input
        if (Input.GetKeyDown(slamKey) && !grounded)
        {
            StartSlam();
        }

        //double jump input
        if (Input.GetKeyDown(jumpKey) && readyToJump && jumpsLeft > 0)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        //calculate movement direction 
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
            //on grounded
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            //in air
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMovementDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        //turn off gravity on a slop
        rb.useGravity = !OnSlope();

    }

    private void SpeedControl()
    {
        //limit speed on slopes
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        //limit speed on ground or in air
        else 
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

    }

    //for normal jumps and double jumps
    private void Jump()
    {
        exitingSlope = true;

        //reset y value
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        float finalJumpForce = jumpForce;

        // If player recently stopped sliding, boost their jump
        if (slide != null && slide.canSlideJump)
        {
            finalJumpForce *= slideJumpBoost;
            slide.canSlideJump = false; // use it once
        }

        rb.AddForce(Vector3.up * finalJumpForce, ForceMode.Impulse);

        jumpsLeft--;

    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    //checking to see if player is on a slope
    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0; 
        }
        return false;
    }

    public Vector3 GetSlopeMovementDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    //when called the player starts falling down fast
    private void StartSlam()
    {
        isSlamming = true;
        rb.velocity = new Vector3(0, -slamForce, 0); 
    }

    //can cancel the slam with slide
    public void CancelSlam() 
    {
        if (isSlamming)
        {
            isSlamming = false;
            rb.AddForce(orientation.forward * moveSpeed * 2f, ForceMode.Impulse);
        }
    }
}
