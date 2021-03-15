using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SyncedObjectManager : MonoBehaviour
{
    [HideInInspector] static List<SyncedObject> syncedObjects = new List<SyncedObject>();
    static bool modifiedSyncedObjects = false;

    void FixedUpdate()
    {
        if (modifiedSyncedObjects)
        {
            SendSyncedObjects();
            modifiedSyncedObjects = false;
        }

        if (Server.Send != null)
        {
            SendSyncedObjectPositions();
        }
    }

    public static void AddSyncedObject(SyncedObject syncedObject, SyncedObjectType syncedObjectType)
    {
        int id = 0;
        bool foundID = false;

        while (foundID == false)
        {
            bool containsID = false;
            for (int i = 0; i < syncedObjects.Count; i++)
            {
                if (syncedObjects[i].id == id)
                {
                    containsID = true;
                }
            }

            if (containsID)
            {
                id++;
            }
            else
            {
                foundID = true;
            }
        }

        syncedObject.id = id;
        syncedObject.type = syncedObjectType;

        syncedObjects.Add(syncedObject);
        modifiedSyncedObjects = true;
    }

    public static void RemoveSyncedObject(SyncedObject syncedObject)
    {
        syncedObjects.Remove(syncedObject);
        modifiedSyncedObjects = true;
    }

    public static GameObject SpawnSyncedObject(GameObject syncedObjectPrefab)
    {
        GameObject newSyncedObject = Instantiate(syncedObjectPrefab);

        return newSyncedObject;
    }

    public static void DespawnSyncedObject(GameObject removedSyncedObject)
    {
        RemoveSyncedObject(removedSyncedObject.GetComponent<SyncedObject>());

        Destroy(removedSyncedObject);
    }

    void SendSyncedObjects()
    {
        string syncedObjectsMessage = "";

        for (int i = 0; i < syncedObjects.Count; i++)
        {
            syncedObjectsMessage += $"{syncedObjects[i].id}~{(int)syncedObjects[i].type}~";
        }

        syncedObjectsMessage.Remove(syncedObjectsMessage.Length - 1);

        for (int i = 0; i < Server.players.Count; i++)
        {
            int clientID = Server.players.ElementAt(i).Key;

            Server.Send.SendMessage(clientID, (int)ServerPacketID.ObjectState, syncedObjectsMessage +  Server.players[clientID].GetComponent<SyncedObject>().id);
        }
    }

    void SendSyncedObjectPositions()
    {
        string syncedObjectPositionsMessage = $"";

        for (int i = 0; i < syncedObjects.Count; i++)
        {
            Vector3 position = syncedObjects[i].gameObject.transform.position;
            Vector3 rotation = syncedObjects[i].gameObject.transform.eulerAngles;

            syncedObjectPositionsMessage += $"{syncedObjects[i].id}~{position.x}~{position.y}~{position.z}~{rotation.x}~{rotation.y}~{rotation.z}~";
        }

        syncedObjectPositionsMessage.Remove(syncedObjectPositionsMessage.Length - 1);

        Server.Send.SendMessageAll((int)ServerPacketID.PhysicsState, syncedObjectPositionsMessage);
    }
}

public enum SyncedObjectType
{
    Player = 1,
    Cube,
}