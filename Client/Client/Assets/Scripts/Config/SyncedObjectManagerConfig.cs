using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SyncedObjectManagerConfig", menuName = "SyncedObjectManagerConfig")]
public class SyncedObjectManagerConfig : ScriptableObject
{
    [Header("Synced Object Prefabs")]
    [SerializeField] GameObject player = null;
    [SerializeField] GameObject cube = null;

    [HideInInspector]
    public Dictionary<SyncedObjectType, GameObject> syncedObjectPrefabs = null;

    public void Initialize()
    {
        syncedObjectPrefabs = new Dictionary<SyncedObjectType, GameObject>()
        {
            { SyncedObjectType.Player, player },
            { SyncedObjectType.Cube, cube }
        };
    }
}
