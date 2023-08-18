using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Animator anim;
    private bool isFacingRight = true;
    private float dirX;

    private bool isWallSlide;
    private float wallSlideSpeed = 2f;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Rigidbody2D rb;

    private enum MovementState { idle, running, jumping, falling }

    [SerializeField] private AudioSource jumpSoundEffect;

    private bool isGrounded;
    private int remainingJumps;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        rb.freezeRotation = true;

        remainingJumps = 1;
    }

    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");

        isGrounded = IsGrounded();
        isWallSlide = isWalled();
        print(isWallSlide);
        print(remainingJumps);


        if (isGrounded)
        {
            remainingJumps = 1;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || remainingJumps > 0)
            {
                if (!isGrounded)
                {
                    remainingJumps--;
                }
  
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpSoundEffect.Play();

            }
            
            
        }

        if (isWallSlide)
        {
            remainingJumps = 1;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (isWallSlide || remainingJumps > 0)
            {
                if (!isWallSlide)
                {
                    remainingJumps--;
                }

                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpSoundEffect.Play();

            }


        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        wallSlide();
        wallJump();

        if (isWallJumping)
        {
            Jump();
        }

        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.4f, groundLayer);
    }

    private bool isWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.4f, wallLayer);
    }

    private void wallSlide()
    {
        if (isWalled() && !IsGrounded() && dirX > 0)
        {
            isWallSlide = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            isWallSlide = false;
        }
    }

    private void wallJump()
    {
        if (isWallSlide)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }
    private void Jump()
    {
        if (isFacingRight && dirX < 0f || isFacingRight && dirX > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > 0.1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -0.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
    }
}
