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
    InputFrame input;
    


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

    private void Update()
    {
        for (int i = 0; i < inputManagers.Length; i++)
        {
            inputManagers[i].GatherInput();
            input = inputManagers[i].currentInputFrame;

        }
        isGrounded = inputHandler.IsGrounded(groundChecker );
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

