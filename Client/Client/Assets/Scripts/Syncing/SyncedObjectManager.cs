using System.Collections.Generic;
using UnityEngine;

public class SyncedObjectManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] SyncedObjectManagerConfig config = null;
    static SyncedObjectManagerConfig staticConfig = null;

    [HideInInspector] public static Message.PhysicsStateMessage syncedObjectPhysicsMessage = null;
    static List<SyncedObject> syncedObjects = new List<SyncedObject>();

    void OnEnable()
    {
        config.Initialize();
        staticConfig = config;
    }

    void FixedUpdate()
    {
        if (syncedObjectPhysicsMessage != null)
        {
            HandleSyncedObjectPositions();
        }
    }

    public static void HandleSyncedObjects(Message.ObjectStateMessage syncedObjectStateMessage)
    {
        List<SyncedObject> newSyncedObjects = new List<SyncedObject>();

        for (int i = 0; i < syncedObjectStateMessage.SyncedObjects.Length; i++)
        {
            SyncedObject syncedObject;

            if (syncedObjectStateMessage.SyncedObjects[i].Id != syncedObjectStateMessage.ClientId)
            {
                syncedObject = Instantiate(staticConfig.syncedObjectPrefabs[(SyncedObjectType)syncedObjectStateMessage.SyncedObjects[i].Type]).GetComponent<SyncedObject>();
            }
            else
            {
                syncedObject = Client.localPlayerObject.GetComponent<SyncedObject>();
            }

            syncedObject.id = syncedObjectStateMessage.SyncedObjects[i].Id;

            newSyncedObjects.Add(syncedObject);
        }

        for (int i = 0; i < newSyncedObjects.Count; i++)
        {
            if (!syncedObjects.Contains(newSyncedObjects[i]))
            {
                syncedObjects.Add(newSyncedObjects[i]);
            }
        }

        for (int i = 0; i < syncedObjects.Count; i++)
        {
            if (!newSyncedObjects.Contains(syncedObjects[i]))
            {
                Destroy(syncedObjects[i].gameObject);
                syncedObjects.Remove(syncedObjects[i]);
            }
        }
    }

    void HandleSyncedObjectPositions()
    {
        for (int i = 0; i < syncedObjectPhysicsMessage.SyncedObjects.Length; i++)
        {
            SyncedObject syncedObject = syncedObjects.Find(s => s.id == syncedObjectPhysicsMessage.SyncedObjects[i].Id);

            if (syncedObject != null && syncedObject.gameObject != Client.localPlayerObject)
            {
                syncedObject.transform.position = syncedObjectPhysicsMessage.SyncedObjects[i].Position.ToVector3();
                syncedObject.transform.eulerAngles = syncedObjectPhysicsMessage.SyncedObjects[i].Rotation.ToVector3();

                syncedObject.velocity = syncedObjectPhysicsMessage.SyncedObjects[i].Velocity.ToVector3();
            }
            else if (syncedObject != null)
            {
                syncedObject.transform.position = syncedObjectPhysicsMessage.SyncedObjects[i].Position.ToVector3();

                syncedObject.velocity = syncedObjectPhysicsMessage.SyncedObjects[i].Velocity.ToVector3();
            }
        }
    }
}

public enum SyncedObjectType
{
    Player = 1,
    Cube,
}
