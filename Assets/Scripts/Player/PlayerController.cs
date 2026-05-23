using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
    [SerializeField] float wallJumpForceX = 22f;
    [SerializeField] float wallJumpForceY = 22f;
    [SerializeField] float wallSlideSpeed = 2f;
    [SerializeField] float wallJumpLockTime = 0.4f;

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
    bool isWallJumping;

    int wallDirection;
    float dashTimer;
    float dashCooldownTimer;
    float moveInput;
    float wallJumpCoyoteTime = 0.15f;
    float wallJumpCoyoteTimer;
    bool wasTouchingWall;
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
        if (!isWallJumping) HandleFlip();
    }

    void FixedUpdate()
    {
        if (isDashing) return;
        if (!isWallJumping) Move();
    }

    // Llamado desde PlayerInputHandler

    public void SetMoveInput(float value) => moveInput = value;

    public void Jump()
    {
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
        // Sale SIEMPRE en dirección opuesta a la pared, ignorando el input
        float jumpDir = -wallDirection;

        rb.linearVelocity = new Vector2(wallJumpForceX * jumpDir, wallJumpForceY);

        // Flip inmediato
        facingRight = jumpDir > 0;
        transform.localScale = new Vector3(
            facingRight ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z);

        canDoubleJump = true;
        StartCoroutine(WallJumpRoutine(jumpDir));
    }

    IEnumerator WallJumpRoutine(float jumpDir)
    {
        isWallJumping = true;
        float timer = 0f;

        while (timer < wallJumpLockTime)
        {
            // Mantiene la velocidad horizontal en la dirección del salto
            // ignorando completamente el input del jugador
            rb.linearVelocity = new Vector2(
                Mathf.Lerp(wallJumpForceX * jumpDir, moveInput * moveSpeed, timer / wallJumpLockTime),
                rb.linearVelocity.y);

            timer += Time.deltaTime;
            yield return null;
        }

        isWallJumping = false;
    }

    IEnumerator DashRoutine()
    {
        AudioManager.Instance?.PlayDash();
        isDashing = true;
        canDash = false;
        float dir = facingRight ? 1f : -1f;

        rb.linearVelocity = new Vector2(dir * dashForce, 0f);
        rb.gravityScale = 0f;

        float timer = 0f;
        while (timer < dashDuration)
        {
            // Fuerza Y a 0 en cada frame durante el dash
            rb.linearVelocity = new Vector2(dir * dashForce, 0f);
            timer += Time.deltaTime;
            yield return null;
        }

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
            transform.position, Vector2.right, wallCheckDistance, groundLayer);
        isTouchingWallLeft = Physics2D.Raycast(
            transform.position, Vector2.left, wallCheckDistance, groundLayer);

        if (isTouchingWallRight) wallDirection = 1;
        else if (isTouchingWallLeft) wallDirection = -1;
    }

    void HandleWallSlide()
    {
        bool onWall = (isTouchingWallRight || isTouchingWallLeft) && !isGrounded;

        isWallSliding = onWall && rb.linearVelocity.y < 0;

        if (isWallSliding)
        {
            rb.linearVelocity = new Vector2(0f, -wallSlideSpeed);
        }
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