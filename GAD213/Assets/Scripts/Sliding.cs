using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private MovementSystem playerMovement;
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;
    public float slideYScale;
    private float startYScale;
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;
    bool sliding;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    public float slideSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<MovementSystem>();

        startYScale = playerObj.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
        {
            StartSlide();
        }
        else if (Input.GetKeyUp(slideKey) && sliding) 
        {
            ExitSlide();
        }

    }

    private void FixedUpdate()
    {
        if (sliding)
        {
            SlidingMovement();
            slideTimer -= Time.deltaTime;
        }
    }

    private void StartSlide()
    {
        sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Force);

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //sliding on flat ground
        if (!playerMovement.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            
        }
        //sliding down a slope
        else
        {
            rb.AddForce(playerMovement.GetSlopeMovementDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if (playerMovement.OnSlope() && rb.velocity.y < 0.1f)
        {
            desiredMoveSpeed = slideSpeed;
        }
        else
        {
            desiredMoveSpeed = playerMovement.moveSpeed;
        }

        slideTimer -= Time.deltaTime;
        lastDesiredMoveSpeed = desiredMoveSpeed;
        
    }

    private void ExitSlide()
    {
        sliding = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        //smoothly lerp moveSpeed to desiredMoveSpeed
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - playerMovement.moveSpeed);
        float startValue = playerMovement.moveSpeed;

        while (time < difference) 
        {
            playerMovement.moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime;
            yield return null;
        }

        playerMovement.moveSpeed = desiredMoveSpeed;
    }
}
