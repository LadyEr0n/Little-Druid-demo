using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    private float accelerationForce = 5;

    [SerializeField]
    private float maxSpeed = 5;

    [SerializeField]
    private float jumpForce = 10;

    [SerializeField]
    private Rigidbody2D rb2d;

    [SerializeField]
    private Collider2D playerGroundCollider;

    [SerializeField]
    private PhysicsMaterial2D playerMovingPhysicsMaterial, playerStoppingPhysicsMaterial;

    [SerializeField]
    private Collider2D groundDetectTrigger;

    [SerializeField]
    private ContactFilter2D groundContactFilter;

    private bool hasDoubleJumped;
    private bool isDead;


    private float horizontalInput;
    private bool isOnGround;
    private Collider2D[] groundHitDetectionResults = new Collider2D[16];
    private Animator animator;

    private Checkpoint currentCheckpoint;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update ()
    {
        UpdateIsOnGround();
        UpdateHorizontalInput();
        HandleJumpInput();
        UpdateAnimationParameters();
    }

    private void CheckForRespawn()
    {
        if (isDead)
        {
            if (Input.GetButtonDown("Respawn"))
                Respawn();
        }
    }

    private void UpdateAnimationParameters()
    {
        animator.SetBool("Ground", isOnGround);
        animator.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));
        animator.SetFloat("vSpeed", rb2d.velocity.y);
        animator.SetBool("Dead", isDead);
    }

    private void FixedUpdate()
    {
        UpdatePhysicsMaterial();
        Move();
    }

    private void UpdatePhysicsMaterial()
    {
        if (Mathf.Abs(horizontalInput) > 0)
        {
            playerGroundCollider.sharedMaterial = playerMovingPhysicsMaterial;
        }
        else
        {
            playerGroundCollider.sharedMaterial = playerStoppingPhysicsMaterial;
        }
    }

    private void UpdateIsOnGround()
    {
        isOnGround= groundDetectTrigger.OverlapCollider(groundContactFilter, groundHitDetectionResults)>0;
       if (isOnGround)
        {
            hasDoubleJumped = false;
        }
        //Debug.Log("isOnGround: " + isOnGround);
    }

    private void UpdateHorizontalInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && (isOnGround || !hasDoubleJumped))
        {
            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        if (Input.GetButtonDown("Jump") && !isOnGround)
        {
            hasDoubleJumped = true;
        }
    }

  

    private void Move()
    {
        if (!isDead)
        {
            rb2d.AddForce(Vector2.right * horizontalInput * accelerationForce);
            Vector2 clampedVelocity = rb2d.velocity;
            clampedVelocity.x = Mathf.Clamp(rb2d.velocity.x, -maxSpeed, maxSpeed);
            rb2d.velocity = clampedVelocity;
        }
        
    }

    public void KillPlayer()
    {
        isDead = true;
        
    }

    public void Respawn()
    {
        isDead = false;
        if (currentCheckpoint==null)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        else
        {
            rb2d.velocity = Vector2.zero;
            transform.position = currentCheckpoint.transform.position;
        }
        
    }

    public void SetCurrentCheckpoint(Checkpoint newCurrentCheckpoint)
    {
        if (currentCheckpoint != null)
        
            currentCheckpoint.SetIsActivated(false);

            currentCheckpoint = newCurrentCheckpoint;
            currentCheckpoint.SetIsActivated(true);
        
        
    }
}
