using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] PlayerMover playerMovement = null;

    List<Message.PlayerInputMessage> inputBuffer = new List<Message.PlayerInputMessage>();
    Message.PlayerInputMessage lastInput = new Message.PlayerInputMessage();

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        HandleInput();

        Debug.Log(inputBuffer.Count);
    }

    public void AddInput(Message.PlayerInputMessage playerInput)
    {
        inputBuffer.Add(playerInput);
    }

    void HandleInput()
    {
        try
        {
            if (inputBuffer.Count > 4)
            {
                Handle(inputBuffer[0]);
                Handle(inputBuffer[0]);
            }
            else if (inputBuffer.Count < 1)
            {
                Handle(lastInput);
            }
            else
            {
                Handle(inputBuffer[0]);
            }
        }
        catch
        {
            Handle(lastInput);
        }

        void Handle(Message.PlayerInputMessage input)
        {
            lastInput = input;

            transform.localEulerAngles = new Vector3(0f, input.CameraRotation.Y, 0f);
            playerMovement.SetMovement(input);

            inputBuffer.Remove(input);
        }
    }
}