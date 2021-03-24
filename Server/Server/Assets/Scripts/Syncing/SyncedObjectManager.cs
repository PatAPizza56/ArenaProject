using Newtonsoft.Json;
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
        List<Message.SyncedObjectMessage> syncedObjectList = new List<Message.SyncedObjectMessage>();

        for (int i = 0; i < syncedObjects.Count; i++)
        {
            syncedObjectList.Add(new Message.SyncedObjectMessage()
            {
                Id = syncedObjects[i].id,

                Type = (int)syncedObjects[i].type,
            });
        }

        for (int i = 0; i < Server.players.Count; i++)
        {
            int clientId = Server.players.ElementAt(i).Key;

            Message message = new Message()
            {
                PacketId = (int)ServerPacketID.ObjectState,
                PacketContent = JsonConvert.SerializeObject(new Message.ObjectStateMessage()
                {
                    ClientId = Server.players[clientId].GetComponent<SyncedObject>().id,
                    SyncedObjects = syncedObjectList.ToArray(),
                }),
            };

            Server.Send.SendMessage(clientId, JsonConvert.SerializeObject(message, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
        }
    }

    void SendSyncedObjectPositions()
    {
        List<Message.SyncedObjectMessage> syncedObjectList = new List<Message.SyncedObjectMessage>();

        for (int i = 0; i < syncedObjects.Count; i++)
        {
            syncedObjectList.Add(new Message.SyncedObjectMessage()
            {
                Id = syncedObjects[i].id,

                Position = new Message.SyncedVector3(syncedObjects[i].gameObject.transform.position.x, syncedObjects[i].gameObject.transform.position.y, syncedObjects[i].gameObject.transform.position.z),
                Rotation = new Message.SyncedVector3(syncedObjects[i].gameObject.transform.eulerAngles.x, syncedObjects[i].gameObject.transform.eulerAngles.y, syncedObjects[i].gameObject.transform.eulerAngles.z),
            });
        }

        Message message = new Message()
        {
            PacketId = (int)ServerPacketID.PhysicsState,
            PacketContent = JsonConvert.SerializeObject(new Message.PhysicsStateMessage()
            {
                SyncedObjects = syncedObjectList.ToArray(),
            }),
        };

        Server.Send.SendMessageAll(JsonConvert.SerializeObject(message, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
    }
}

public enum SyncedObjectType
{
    Player = 1,
    Cube,
}