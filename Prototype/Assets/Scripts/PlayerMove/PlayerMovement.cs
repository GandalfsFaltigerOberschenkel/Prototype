using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb2d;
    private float currentHorizontalSpeed = 0;
    private float coyoteTimeCounter = 0;
    private bool isHovering = false;
    private float hoverCounter = 0;

    public float horizontalAcceleration = 0;
    public float maxHorizontalSpeed = 0;
    public float jumpForce = 0;
    public float hoverDuration = 0;
    public float jumpHoldForce = 0;
    public float jumpHoldAcceleration = 0;
    private float currentJumpHoldForce = 0;
    public float gravityForce = -9;
    public float fallMultiplier;

    // Swinging properties
    public bool isSwinging = false;
    public Vector2 ropeHook;
    public float swingForce = 4f;
    public float speed = 1f;

    public void Initialize(Rigidbody2D rb)
    {
        if (rb2d == null)
            rb2d = rb;

        currentJumpHoldForce = jumpHoldForce;
    }

    public void HandleMovement(InputFrame input, bool isGrounded)
    {
        if (isSwinging)
        {
            HandleSwingingMovement(input);
        }
        else
        {
            HandleGroundMovement(input, isGrounded);
        }
    }

    private void HandleGroundMovement(InputFrame input, bool isGrounded)
    {
        if (input.inputDirection.x != 0)
        {
            currentHorizontalSpeed += input.inputDirection.x * horizontalAcceleration * Time.deltaTime;
            currentHorizontalSpeed = Mathf.Clamp(currentHorizontalSpeed, -maxHorizontalSpeed, maxHorizontalSpeed);
        }
        else
        {
            currentHorizontalSpeed = Mathf.MoveTowards(currentHorizontalSpeed, 0, horizontalAcceleration * Time.deltaTime);
        }

        if (isGrounded)
        {
            coyoteTimeCounter = 0.2f;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        rb2d.linearVelocity = new Vector2(currentHorizontalSpeed, rb2d.linearVelocity.y);
    }

    private void HandleSwingingMovement(InputFrame input)
    {
        var playerToHookDirection = (ropeHook - rb2d.position).normalized;
        Vector2 perpendicularDirection;

        if (input.inputDirection.x < 0)
        {
            perpendicularDirection = new Vector2(-playerToHookDirection.y, playerToHookDirection.x);
        }
        else
        {
            perpendicularDirection = new Vector2(playerToHookDirection.y, -playerToHookDirection.x);
        }

        var force = perpendicularDirection * swingForce;
        rb2d.AddForce(force, ForceMode2D.Force);
    }

    public void HandleCeilingCollision(bool hitCeiling)
    {
        if (hitCeiling && rb2d.linearVelocity.y > 0)
        {
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, 0);
        }
    }

    public void HandleJumping(bool isGrounded, InputFrame input)
    {
        if (isSwinging)
            return;

        if ((isGrounded || coyoteTimeCounter > 0) && input.actionButtonPressed)
        {
            StartJump();
        }
        else if (input.actionButtonHeld && rb2d.linearVelocity.y > 0)
        {
            ContinueJump(input);
        }
        else if (isHovering && hoverCounter <= 0)
        {
            EndHover(isGrounded);
        }
        else if (!input.actionButtonHeld)
        {
            isHovering = false;
        }

        hoverCounter -= Time.deltaTime;
    }

    private void StartJump()
    {
        rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, 0);
        rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        hoverCounter = hoverDuration;
        isHovering = false;
        coyoteTimeCounter = 0;
        currentJumpHoldForce = jumpHoldForce;
    }

    private void ContinueJump(InputFrame input)
    {
        if (input.actionButtonHeld && currentJumpHoldForce > 0)
        {
            rb2d.linearVelocity += Vector2.up * currentJumpHoldForce * Time.deltaTime;
            currentJumpHoldForce -= jumpHoldAcceleration * Time.deltaTime;
        }
        else if (hoverCounter > 0 && !isHovering)
        {
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, 0);
            isHovering = true;
        }
    }

    private void EndHover(bool isGrounded)
    {
        isHovering = false;
        ApplyGravity(isGrounded);
    }

    public void ApplyGravity(bool isGrounded)
    {
        if (!isGrounded && !isHovering)
        {
            float gravity = gravityForce * Time.deltaTime;
            if (rb2d.linearVelocity.y < 0)
            {
                rb2d.linearVelocity += Vector2.up * gravity * fallMultiplier;
            }
            else
            {
                rb2d.linearVelocity += Vector2.up * gravity;
            }
        }
    }

    // Call this to start or stop swinging
    public void ToggleSwinging(bool enableSwinging, Vector2 hookPosition)
    {
        isSwinging = enableSwinging;
        ropeHook = hookPosition;
    }
}
