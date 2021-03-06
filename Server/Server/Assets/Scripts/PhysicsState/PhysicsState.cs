using System.Collections.Generic;
using UnityEngine;

public class PhysicsState : MonoBehaviour
{
    [HideInInspector] public static PhysicsState instance = null;
    [HideInInspector] public List<SyncedObject> syncedObjects = new List<SyncedObject>();

    void Awake()
    {
        instance = this;
    }

    void FixedUpdate()
    {
        Server.instance.serverSend.SendMessageAll((int)ServerPacketID.PhysicsState, new string[] { CompilePhysics() }); 
    }

    public string CompilePhysics()
    {
        string physicsStateMessage = $"{syncedObjects.Count}~";

        for (int i = 0; i < syncedObjects.Count; i++)
        {
            Vector3 position = syncedObjects[i].transform.position;
            Vector3 rotation = syncedObjects[i].transform.eulerAngles;

            physicsStateMessage += $"{syncedObjects[i].id}~{position.x}~{position.y}~{position.z}~{rotation.x}~{rotation.y}~{rotation.z}~";
        }

        physicsStateMessage.Remove(physicsStateMessage.Length - 1);

        return physicsStateMessage;
    }
}