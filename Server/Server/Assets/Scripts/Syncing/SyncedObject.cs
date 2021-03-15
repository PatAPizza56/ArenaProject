using UnityEngine;

public class SyncedObject : MonoBehaviour
{
    [HideInInspector] public long id;
    [SerializeField] public SyncedObjectType type;

    void Start()
    {
        SyncedObjectManager.AddSyncedObject(this, type);
    }
}