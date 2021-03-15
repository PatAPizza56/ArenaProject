using System.Collections.Generic;
using UnityEngine;

public class SyncedObjectManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] SyncedObjectManagerConfig config = null;

    [HideInInspector] public static string syncedObjectPhysicsMessage = null;
    [HideInInspector] public static string syncedObjectStateMessage = null;
    [HideInInspector] public static bool modifiedSyncedObjectStates = false;

    static List<SyncedObject> syncedObjects = new List<SyncedObject>();

    void OnEnable()
    {
        config.Initialize();
    }

    void FixedUpdate()
    {
        if (modifiedSyncedObjectStates)
        {
            HandleSyncedObjects();
            modifiedSyncedObjectStates = false;
        }
        
        HandleSyncedObjectPositions();
    }

    void HandleSyncedObjects()
    {
        List<SyncedObject> newSyncedObjects = new List<SyncedObject>();
        string[] newSyncedObjectStateMessage = syncedObjectStateMessage.Split('~');

        for (int i = 0; i < newSyncedObjectStateMessage.Length - 2; i += 2)
        {
            int syncedObjectId = int.Parse(newSyncedObjectStateMessage[i]);
            SyncedObjectType syncedObjectType = (SyncedObjectType)int.Parse(newSyncedObjectStateMessage[i + 1]);

            GameObject syncedObject;

            if (syncedObjectId != int.Parse(newSyncedObjectStateMessage[newSyncedObjectStateMessage.Length - 1]))
            {
                syncedObject = Instantiate(config.syncedObjectPrefabs[syncedObjectType]);
            }
            else
            {
                syncedObject = Client.localPlayerObject;
            }

            syncedObject.GetComponent<SyncedObject>().id = syncedObjectId;
            newSyncedObjects.Add(syncedObject.GetComponent<SyncedObject>());
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
        string[] newSyncedObjectPhysicsMessage = syncedObjectPhysicsMessage.Split('~');

        for (int i = 0; i < newSyncedObjectPhysicsMessage.Length - 1; i += 7)
        {
            int syncedObjectId = int.Parse(newSyncedObjectPhysicsMessage[i]);
            Vector3 syncedObjectPosition = new Vector3(float.Parse(newSyncedObjectPhysicsMessage[i + 1]), float.Parse(newSyncedObjectPhysicsMessage[i + 2]), float.Parse(newSyncedObjectPhysicsMessage[i + 3]));
            Vector3 syncedObjectRotation = new Vector3(float.Parse(newSyncedObjectPhysicsMessage[i + 4]), float.Parse(newSyncedObjectPhysicsMessage[i + 5]), float.Parse(newSyncedObjectPhysicsMessage[i + 6]));

            SyncedObject syncedObject = syncedObjects.Find(s => s.id == syncedObjectId);

            if (syncedObject != null)
            {
                syncedObject.transform.position = syncedObjectPosition;
                //syncedObject.transform.eulerAngles = syncedObjectRotation;
            }
        }
    }
}

public enum SyncedObjectType
{
    Player = 1,
    Cube,
}
