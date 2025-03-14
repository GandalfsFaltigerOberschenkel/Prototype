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
    public bool isGrounded;
    private bool hitCeiling;
    public PlayerAnimationController animationController;
    public Rigidbody2D rb2d;
    public List<UpgradeBase> upgrades = new List<UpgradeBase>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        UpgradeBase upgrade = collision.gameObject.GetComponent<UpgradeBase>();
        if (upgrade != null)
        {
            UnlockUpgrade(upgrades.IndexOf(upgrades.FirstOrDefault(e => e.id == upgrade.id)));
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Player Hit");
            Vector2 bulletPos = collision.gameObject.transform.position;
            Vector2 playerPos = transform.position;
            Vector2 direction = (playerPos - bulletPos).normalized;
            rb2d.AddForce(direction * 10, ForceMode2D.Impulse);
            StartCoroutine(GetComponent<FallThroughPlattforms>().FallThrough());
            Destroy(collision.gameObject);
        }
    }

    public void UnlockUpgrade(int index)
    {
        upgrades[index].unlocked = true;
    }


    private void Start()
    {
        InitializeComponents();
        playerMovement.Initialize(rb2d);
    }

    InputFrame SplitInput(InputFrame[] input)
    {
        if (input.Length == 1)
        {
            return input[0];
        }
        else if (input.Length == 2)
        {
            InputFrame combinedInput = new InputFrame();
            combinedInput.inputDirection = input[0].inputDirection;
            combinedInput.aimDirection = input[1].aimDirection;
            combinedInput.actionButtonPressed = input[0].actionButtonPressed;
            combinedInput.actionButtonHeld = input[0].actionButtonHeld;
            combinedInput.actionButtonReleased = input[0].actionButtonReleased;
            combinedInput.swingButtonPressed = input[1].swingButtonPressed;
            combinedInput.swingButtonHeld = input[1].swingButtonHeld;
            combinedInput.swingButtonReleased = input[1].swingButtonReleased;
            combinedInput.upgradeButton1Pressed = input[0].upgradeButton1Pressed;
            combinedInput.upgradeButton2Pressed = input[0].upgradeButton2Pressed;
            combinedInput.upgradeButton3Pressed = input[0].upgradeButton3Pressed;
            return combinedInput;
        }
        else
        {
            return input[0];
        }
    }

    private void FixedUpdate()
    {
        isGrounded = inputHandler.IsGrounded(groundChecker);
        hitCeiling = inputHandler.IsHitCeiling(ceilingChecker);
    }

    private void Update()
    {
        if (isPaused)
        {
            return;
        }

        Collider2D[] droneEnemiesBellow = Physics2D.OverlapCircleAll(groundChecker.transform.position, 0.1f);
        Collider2D enemy = droneEnemiesBellow.FirstOrDefault(e => e.GetComponent<DroneEnemy>() != null);
        if (enemy != null)
        {
            enemy.GetComponent<DroneEnemy>().TakeDamage(100);
            if (GetComponent<FallThroughPlattforms>().isFallingThrough)
            {
                GetComponent<FallThroughPlattforms>().isFallingThrough = false;

            }
        }
       
        List<InputFrame> inputFrames = new List<InputFrame>();
        for (int i = 0; i < inputManagers.Length; i++)
        {
            inputManagers[i].GatherInput();
            inputFrames.Add(inputManagers[i].currentInputFrame);
        }
        input = SplitInput(inputFrames.ToArray());
        // In PlayerController's Update() method:
        animationController.SetWalkingSpeed(input.inputDirection.x);
        animationController.SetGrounded(isGrounded);
        if (input.upgradeButton1Pressed)
        {
            UpgradeUIController.Instance.upgradeUIs[0].OnActivateButtonClicked();
        }
        if (input.upgradeButton2Pressed)
        {
            UpgradeUIController.Instance.upgradeUIs[1].OnActivateButtonClicked();
        }
        if (input.upgradeButton3Pressed)
        {
            UpgradeUIController.Instance.upgradeUIs[2].OnActivateButtonClicked();
        }

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
        if (rb2d == null)
            rb2d = GetComponent<Rigidbody2D>();
    }
}

