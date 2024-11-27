using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public InputHandler inputHandler;
    public PlayerMovement playerMovement;
    public InputManager[] inputManagers;
    public InputFrame input;
    
    public bool isPaused = false;

    public Transform groundChecker;
    public Transform ceilingChecker;
    public LayerMask groundMask;

    public bool isHovering = false;
    private bool isGrounded;
    private bool hitCeiling;
    public Rigidbody2D rb2d;

    private void Start()
    {
        InitializeComponents();
        playerMovement.Initialize(rb2d);
    }
    InputFrame SplitInput(InputFrame[] input)
    {
        if(input.Length == 1)
        {
            return input[0];
        }
        else if(input.Length == 2)
        {
            InputFrame combinedInput = new InputFrame();
            combinedInput.inputDirection = input[0].inputDirection;
            combinedInput.aimDirection = input[1].aimDirection;
            combinedInput.actionButtonPressed = input[0].actionButtonPressed;
            combinedInput.actionButtonHeld = input[0].actionButtonHeld;
            combinedInput.actionButtonReleased = input[0].actionButtonReleased;
            combinedInput.swingButtonPressed = input[1].actionButtonPressed;
            combinedInput.swingButtonHeld = input[1].actionButtonHeld;
            combinedInput.swingButtonReleased = input[1].actionButtonReleased;
            return combinedInput;
        }
        else
        {
            return input[0];
        }
    }

    private void Update()
    {
        if (isPaused)
        {
            return;
        }
        List<InputFrame> inputFrames = new List<InputFrame>();
        for (int i = 0; i < inputManagers.Length; i++)
        {
            inputManagers[i].GatherInput();
            inputFrames.Add(inputManagers[i].currentInputFrame);
        }
        input = SplitInput(inputFrames.ToArray());
        isGrounded = inputHandler.IsGrounded(groundChecker);
        hitCeiling = inputHandler.IsHitCeiling(ceilingChecker);

        playerMovement.HandleMovement(input, isGrounded);
        playerMovement.HandleJumping(isGrounded, input);
        if (input.actionButtonPressed)
        {
            Debug.Log($"Jump Pressed: {input.actionButtonPressed}");
            Debug.Log($"Is Grounded: {isGrounded}");
        }
        playerMovement.HandleCeilingCollision(hitCeiling);
        playerMovement.ApplyGravity(isGrounded);
    }

    private void InitializeComponents()
    {
        input = new InputFrame();
        if(rb2d == null)
        rb2d = GetComponent<Rigidbody2D>();
    }
    

    
}

