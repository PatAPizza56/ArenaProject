using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputConfig", menuName = "InputConfig")]
public class PlayerInputConfig : ScriptableObject
{
    [Header("Movement Input")]
    [SerializeField] public InputAction playerMovement = new InputAction();
    [SerializeField] public InputAction playerJump = new InputAction();

    [Header("Camera Input")]
    [SerializeField] public InputAction cameraMovement = new InputAction();

    void OnEnable()
    {
        playerMovement.Enable();
        playerJump.Enable();

        cameraMovement.Enable();
    }
}