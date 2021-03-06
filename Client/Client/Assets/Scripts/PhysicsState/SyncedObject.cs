using UnityEngine;

public class SyncedObject : MonoBehaviour
{
    [SerializeField] public long id;

    void Start()
    {
        if (PhysicsState.instance != null) PhysicsState.instance.syncedObjects.Add(this);
    }

    void OnEnable()
    {
        if (PhysicsState.instance != null) PhysicsState.instance.syncedObjects.Add(this);
    }

    void OnDisable()
    {
        if (PhysicsState.instance != null) PhysicsState.instance.syncedObjects.Remove(this);
    }
}
