﻿using UnityEngine;

[CreateAssetMenu(fileName = "ServerConfig", menuName = "ServerConfig")]
public class ServerConfig : ScriptableObject
{
    [Header("Player Settings")]
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public Vector3[] spawnPoints;
}