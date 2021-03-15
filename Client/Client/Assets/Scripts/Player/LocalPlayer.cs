using UnityEngine;

public class LocalPlayer : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] PlayerInput playerInput = null;
    [SerializeField] PlayerLook playerLook = null;
    [SerializeField] Transform cameraTransform = null;

    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {        
        SetCamera();
    }

    void FixedUpdate()
    {
        SendInput();
    }

    void SendInput()
    {
        Client.Send.SendMessage((int)ClientPacketID.PlayerInput, 
            $"{cameraTransform.position.x}~{cameraTransform.position.y}~{cameraTransform.position.z}~" +
            $"{cameraTransform.eulerAngles.x}~{cameraTransform.eulerAngles.y}~{cameraTransform.eulerAngles.z}~" +
            $"{playerInput.playerMovementInput.x}~{playerInput.playerMovementInput.y}~{playerInput.playerJumpInput}");
    }

    void SetCamera()
    {
        PlayerLookInput cameraInput = new PlayerLookInput()
        {
            VerticalInput = playerInput.cameraMovementInput.y,
            HorizontalInput = playerInput.cameraMovementInput.x
        };

        playerLook.SetCamera(cameraInput);
    }
}
