using UnityEngine;

public class SyncedObject : MonoBehaviour
{
    [SerializeField] public SyncedObjectType type;

    [HideInInspector] public long id;
    [HideInInspector] public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        SyncedObjectManager.AddSyncedObject(this, type);
    }
}