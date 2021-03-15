using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] PlayerMover playerMovement = null;
    List<PlayerInput> inputBuffer = new List<PlayerInput>();

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        HandleInput();
    }

    public void AddInput(PlayerInput playerInput)
    {
        inputBuffer.Add(playerInput);
    }

    void HandleInput()
    {
        PlayerInput input = inputBuffer[0];

        transform.localEulerAngles = new Vector3(0f, input.playerCameraRotation.y, 0f);

        playerMovement.SetMovement(input);

        inputBuffer.RemoveAt(0);
    }
}

public class PlayerInput
{
    public Vector3 playerCameraPosition { get; set; }
    public Vector3 playerCameraRotation { get; set; }

    public Vector2 playerMovementInput { get; set; }
    public float playerJumpInput { get; set; }
}