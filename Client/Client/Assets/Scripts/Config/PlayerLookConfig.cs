using UnityEngine;

[CreateAssetMenu(fileName = "PlayerLookConfig", menuName = "PlayerLookConfig")]
public class PlayerLookConfig : ScriptableObject
{
    [Header("Camera Position")]
    [SerializeField] public Vector3 offset = new Vector3(0f, 1f, 0f);

    [Header("Camera Clamp")]
    [SerializeField] public float minClamp = -90f;
    [SerializeField] public float maxClamp = 90f;

    [Header("Camera Sensitivity")]
    [SerializeField] public float xSensitivity = 7f;
    [SerializeField] public float ySensitivity = 7f;

    [Header("Camera FOV's")]
    [SerializeField] public float wideFOV = 75f;
    [SerializeField] public float normalFOV = 60f;
    [SerializeField] public float zoomFOV = 50f;
}
