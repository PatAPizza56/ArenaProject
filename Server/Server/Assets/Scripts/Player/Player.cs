using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] PlayerController playerMovement;

    Vector2 playerInputMovement = Vector2.zero;
    bool handledJump = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        SetMovement();
    }

    public void AddInput(Vector2 playerMovementInput, float playerJumpInput)
    {
        this.playerInputMovement = playerMovementInput;
        if (playerJumpInput > 0) { handledJump = false; }

        PlayerInput input = new PlayerInput()
        {
            playerMovementInput = playerMovementInput,
            playerJumpInput = playerJumpInput
        };
    }

    void SetMovement()
    {
        float jump;
        if (!handledJump)
        {
            jump = 1;
            handledJump = true;
        }
        else
        {
            jump = 0;
        }

        MovementInput movementInput = new MovementInput()
        {
            HorizontalInput = playerInputMovement.x,
            VerticalInput = playerInputMovement.y,
            JumpInput = jump
        };

        playerMovement.SetMovement(movementInput);
    }
}

public class PlayerInput
{
    public Vector2 playerMovementInput;
    public float playerJumpInput;
}