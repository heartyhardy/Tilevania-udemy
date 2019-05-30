using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour {

    [Header("Movement")]
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 2.5f;

    [Header("VFX")]
    [SerializeField] GameObject deathVfx;

    private Rigidbody2D playerRigidBody;
    float currentGravity;

    bool isAlive = true;

    // Use this for initialization
    void Start () {
        playerRigidBody = GetComponent<Rigidbody2D>();
        currentGravity = playerRigidBody.gravityScale;
    }
	
	// Update is called once per frame
	void Update () {
        if (isAlive)
        {
            Run();
            Climb();
            Jump();
            FacePlayerTowardsMovement();
            Die();
        }
	}

    private void Run()
    {
        var deltaX = CrossPlatformInputManager.GetAxis("Horizontal") * runSpeed;
        Vector2 runVector = new Vector2(deltaX, playerRigidBody.velocity.y);

        playerRigidBody.velocity = runVector;

        bool isRunning = Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon;

        if (isRunning)
        {
            ChangeStateToRunning();
        }
        else
        {
            ChangeStateToIdle();
        }
    }

    private void Climb()
    {
        bool isOnLadder = GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.GetMask("Ladders"));

        if (isOnLadder)
        {
            float climbDeltaY = Input.GetAxis("Vertical");
            Vector2 climbVector = new Vector2(playerRigidBody.velocity.x / 2f , climbDeltaY * climbSpeed);

            playerRigidBody.velocity = climbVector;
            playerRigidBody.gravityScale = 0;

            bool isClimbing = Mathf.Abs(playerRigidBody.velocity.y) > Mathf.Epsilon;

            if (isClimbing)
            {
                ChangeStateToClimbing();
            }
        }
        else
        {
            ChangeStateToNonClimbing();
            playerRigidBody.gravityScale = currentGravity;
        }
    }

    private void Jump()
    {
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            bool isOnGround = GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.GetMask("Ground"));

            if (isOnGround)
            {
                Vector2 jumpVector = new Vector2(0f, jumpSpeed);
                playerRigidBody.velocity += jumpVector;
            }
            else return;
        }
    }

    private void Die()
    {
        CapsuleCollider2D playerCollider = GetComponent<CapsuleCollider2D>();

        if (playerCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            isAlive = false;
            PlayDeathAnimation();
        }
    }

    private void PlayDeathAnimation()
    {
        deathVfx.GetComponent<ParticleSystem>().Play();

        playerRigidBody.gravityScale = currentGravity / 10;
        playerRigidBody.velocity += new Vector2(UnityEngine.Random.Range(1f, 2f), UnityEngine.Random.Range(30f, 100f));
        ChangeStateToDead();

        Destroy(deathVfx, 5f);
        Destroy(gameObject, 5f);
    }

    private void FacePlayerTowardsMovement()
    {
        bool isOnMove = Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon;

        if (isOnMove)
        {
            transform.localScale = new Vector2(Mathf.Sign(playerRigidBody.velocity.x), 1f);
        }
    }

    private void ChangeStateToRunning()
    {
        GetComponent<Animator>().SetBool("Running", true);
    }

    private void ChangeStateToIdle()
    {
        GetComponent<Animator>().SetBool("Running", false);
    }

    private void ChangeStateToClimbing()
    {
        GetComponent<Animator>().SetBool("Climbing", true);
    }

    private void ChangeStateToNonClimbing()
    {
        GetComponent<Animator>().SetBool("Climbing", false);
    }

    private void ChangeStateToDead()
    {
        GetComponent<Animator>().SetBool("Running", false);
        GetComponent<Animator>().SetTrigger("Dead");
    }
}
