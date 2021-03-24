using UnityEngine;
using Newtonsoft.Json;

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
        Message message = new Message()
        {
            PacketId = (int)ClientPacketID.PlayerInput,
            PacketContent = JsonConvert.SerializeObject(new Message.PlayerInputMessage()
            {
                CameraPosition = new Message.SyncedVector3(cameraTransform.position.x, cameraTransform.position.y, cameraTransform.position.z),
                CameraRotation = new Message.SyncedVector3(cameraTransform.eulerAngles.x, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z),

                MoveInput = new Message.SyncedVector2(playerInput.playerMovementInput.x, playerInput.playerMovementInput.y),
                JumpInput = playerInput.playerJumpInput,
            }),
        };

        Client.Send.SendMessage(JsonConvert.SerializeObject(message, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
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
