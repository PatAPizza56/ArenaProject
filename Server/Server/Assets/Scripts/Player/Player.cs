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

    public void SetInput(Vector2 playerInputMovement, float playerInputJump)
    {
        this.playerInputMovement = playerInputMovement;

        if (playerInputJump > 0) { handledJump = false; }
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