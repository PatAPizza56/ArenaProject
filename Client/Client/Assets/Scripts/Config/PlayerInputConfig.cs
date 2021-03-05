using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputConfig", menuName = "InputConfig")]
public class PlayerInputConfig : ScriptableObject
{
    [Header("Movement Input")]
    [SerializeField] public InputAction playerMovement = null;
    [SerializeField] public InputAction playerJump = null;

    [Header("Camera Input")]
    [SerializeField] public InputAction cameraMovement = null;

    void OnEnable()
    {
        playerMovement.Enable();
        playerJump.Enable();

        cameraMovement.Enable();
    }
}