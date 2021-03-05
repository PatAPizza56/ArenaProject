using System;
using UnityEngine;

public class LocalPlayer : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] PlayerInput playerInput = null;
    [SerializeField] PlayerLook playerLook = null;

    [NonSerialized] public Vector3 newPosition = Vector3.zero;

    public bool ChangedPosition = false;

    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        SendInput();
        SendRotation();

        SetPosition();
        SetCamera();
    }

    void SendInput()
    {
        Client.instance.clientSend.SendMessage((int)ClientPacketID.PlayerInput, new string[] { playerInput.playerMovementInput.x.ToString(), playerInput.playerMovementInput.y.ToString(), playerInput.playerJumpInput.ToString() });
    }

    void SendRotation()
    {
        Client.instance.clientSend.SendMessage((int)ClientPacketID.PlayerRotation, new string[] { transform.eulerAngles.y.ToString() });
    }

    void SetPosition()
    {
        if (transform.position != newPosition) ChangedPosition = true;

        transform.position = newPosition;
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
