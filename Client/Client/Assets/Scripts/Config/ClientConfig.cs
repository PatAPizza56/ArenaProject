using UnityEngine;

[CreateAssetMenu(fileName = "ClientConfig", menuName = "ClientConfig")]
public class ClientConfig : ScriptableObject
{
    [Header("Prefab Settings")]
    [SerializeField] public GameObject localPlayerPrefab = null;
}
