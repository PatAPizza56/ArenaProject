using System.Collections.Generic;
using UnityEngine;

public class PhysicsState : MonoBehaviour
{
    [HideInInspector] public static PhysicsState instance = null;
    [HideInInspector] public List<SyncedObject> syncedObjects = new List<SyncedObject>();

    [HideInInspector] public string[] physicsStateMessages = null;
    [HideInInspector] public int syncedObjectCount = 0;

    void Awake()
    {
        instance = this;
    }

    void FixedUpdate()
    {
        DecompilePhysics();
    }

    void DecompilePhysics()
    {
        for (int i = 1; i < syncedObjectCount * 7; i += 7)
        {
            int syncedObjectId = int.Parse(physicsStateMessages[i]);
            Vector3 syncedObjectPosition = new Vector3(float.Parse(physicsStateMessages[i + 1]), float.Parse(physicsStateMessages[i + 2]), float.Parse(physicsStateMessages[i + 3]));
            Vector3 syncedObjectRotation = new Vector3(float.Parse(physicsStateMessages[i + 4]), float.Parse(physicsStateMessages[i + 5]), float.Parse(physicsStateMessages[i + 6]));

            SyncedObject syncedObject = syncedObjects.Find(s => s.id == syncedObjectId);
            if (syncedObject != null)
            {
                syncedObject.transform.position = syncedObjectPosition;
                syncedObject.transform.eulerAngles = syncedObjectRotation;
            }
        }
    }
}
