using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] public PlayerInputConfig config = null;

    [HideInInspector] public Vector2 playerMovementInput = Vector2.zero;
    [HideInInspector] public float playerJumpInput = 0f;

    [HideInInspector] public Vector2 cameraMovementInput = Vector2.zero;

    void Update()
    {
        playerMovementInput = config.playerMovement.ReadValue<Vector2>();
        playerJumpInput = config.playerJump.phase == InputActionPhase.Started ? 1f : 0f;

        cameraMovementInput = config.cameraMovement.ReadValue<Vector2>();
    }
}
