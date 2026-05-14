using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float jumpForce = 18f;
    [SerializeField] float dashForce = 18f;
    [SerializeField] float dashDuration = 0.15f;
    [SerializeField] float dashCooldown = 1f;

    [Header("Wall Jump")]
    [SerializeField] float wallJumpForceX = 10f;
    [SerializeField] float wallJumpForceY = 14f;
    [SerializeField] float wallSlideSpeed = 2f;

    [Header("Checks")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.15f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform wallCheckRight;
    [SerializeField] Transform wallCheckLeft;
    [SerializeField] float wallCheckDistance = 0.3f;

    // State
    bool isGrounded;
    bool isTouchingWallRight;
    bool isTouchingWallLeft;
    bool isWallSliding;
    bool canDoubleJump;
    bool isDashing;
    bool canDash = true;
    bool isBraking;

    float dashTimer;
    float dashCooldownTimer;
    float moveInput;
    bool facingRight = true;

    public bool IsGrounded => isGrounded;
    public bool IsDashing => isDashing;
    public bool IsBraking => isBraking;
    public bool FacingRight => facingRight;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDashing) return;

        HandleGroundCheck();
        HandleWallCheck();
        HandleWallSlide();
        HandleDashCooldown();
        HandleFlip();
    }

    void FixedUpdate()
    {
        if (isDashing) return;
        Move();
    }

    // Llamado desde PlayerInputHandler

    public void SetMoveInput(float value) => moveInput = value;

    public void Jump()
    {
        AudioManager.Instance?.PlayJump();
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = true;
        }
        else if (isWallSliding)
        {
            WallJump();
        }
        else if (canDoubleJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = false;
        }
    }

    public void Dash()
    {
        if (!canDash || isDashing) return;
        StartCoroutine(DashRoutine());
    }

    // Movimiento

    void Move()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void WallJump()
    {
        float direction = isTouchingWallRight ? -1f : 1f;
        rb.linearVelocity = new Vector2(wallJumpForceX * direction, wallJumpForceY);
        canDoubleJump = true;
    }

    System.Collections.IEnumerator DashRoutine()
    {
        AudioManager.Instance?.PlayDash();
        isDashing = true;
        canDash = false;
        float dir = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * dashForce, 0f);
        rb.gravityScale = 0f;

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = 3f;
        isDashing = false;

        dashCooldownTimer = dashCooldown;
    }

    // Checks

    void HandleGroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
            canDoubleJump = true;
    }

    void HandleWallCheck()
    {
        isTouchingWallRight = Physics2D.Raycast(
            wallCheckRight.position, Vector2.right, wallCheckDistance, groundLayer);
        isTouchingWallLeft = Physics2D.Raycast(
            wallCheckLeft.position, Vector2.left, wallCheckDistance, groundLayer);
    }

    void HandleWallSlide()
    {
        isWallSliding = (isTouchingWallRight || isTouchingWallLeft)
                        && !isGrounded
                        && rb.linearVelocity.y < 0;

        if (isWallSliding)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
    }

    void HandleDashCooldown()
    {
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0) canDash = true;
        }
    }

    void HandleFlip()
    {
        // Frenado: tiene velocidad horizontal pero el input va en dirección contraria
        isBraking = (rb.linearVelocity.x > 0.5f && moveInput < 0)
                 || (rb.linearVelocity.x < -0.5f && moveInput > 0);

        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(
            -transform.localScale.x,
            transform.localScale.y,
            transform.localScale.z);
    }

    // Gizmos (visibles en el editor)

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        if (wallCheckRight != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallCheckRight.position,
                wallCheckRight.position + Vector3.right * wallCheckDistance);
        }
        if (wallCheckLeft != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallCheckLeft.position,
                wallCheckLeft.position + Vector3.left * wallCheckDistance);
        }
    }
}