using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Server : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] ServerConfig config = null;

    static ServerConfig staticConfig = null;

    static Telepathy.Server server = new Telepathy.Server(50000);

    public static Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

    public static ServerSend Send;
    static ServerHandle Handle;

    void Awake()
    {
        staticConfig = config;
    }

    void Update()
    {
        if (server.Active) server.Tick(100);
    }

    void OnApplicationQuit()
    {
        StopServer();
    }

    void OnGUI()
    {
        GUI.enabled = !server.Active;
        if (GUI.Button(new Rect(0, 25, 120, 20), "Start Server"))
            StartServer();

        GUI.enabled = server.Active;
        if (GUI.Button(new Rect(130, 25, 120, 20), "Stop Server"))
            StopServer();

        GUI.enabled = true;
    }

    public static void StartServer()
    {
        Application.runInBackground = true;

        Telepathy.Log.Info = Debug.Log;
        Telepathy.Log.Warning = Debug.LogWarning;
        Telepathy.Log.Error = Debug.LogError;

        Send = new ServerSend();
        Handle = new ServerHandle();

        server.OnConnected = Handle.OnClientConnect;
        server.OnData = Handle.OnReceiveData;
        server.OnDisconnected = Handle.OnClientDisconnect;

        server.Start(1337);
    }

    public static void StopServer()
    {
        server.Stop();
    }

    public class ServerSend
    {
        public void SendMessage(int clientID, int packetID, string message)
        {
            string sendMessage = $"{packetID}_{message}";

            server.Send(clientID, new ArraySegment<byte>(Encoding.UTF8.GetBytes(sendMessage)));
        }

        public void SendMessageAll(int packetID, string message)
        {
            for (int i = 0; i < players.Count; i++)
            {
                int clientID = players.ElementAt(i).Key;

                SendMessage(clientID, packetID, message);
            }
        }

        public void SendMessageAllExcept(int clientID, int packetID, string message)
        {
            for (int i = 0; i < players.Count; i++)
            {
                int eClientID = players.ElementAt(i).Key;

                if (eClientID != clientID)
                {
                    SendMessage(eClientID, packetID, message);
                }
            }
        }
    }

    public class ServerHandle
    {
        public void OnClientConnect(int clientID)
        {
            int spawnPoint = UnityEngine.Random.Range(0, staticConfig.spawnPoints.Length - 1);

            players.Add(clientID, SyncedObjectManager.SpawnSyncedObject(staticConfig.playerPrefab));
            players[clientID].transform.position = staticConfig.spawnPoints[spawnPoint];
        }

        public void OnClientDisconnect(int clientID)
        {
            SyncedObjectManager.DespawnSyncedObject(players[clientID]);
            players.Remove(clientID);
        }

        public void OnReceiveData(int clientID, ArraySegment<byte> data)
        {
            string[] message = Encoding.UTF8.GetString(data.Array, data.Offset, data.Count).Split('_');

            int packetID = int.Parse(message[0]);
            string packetContent = message[1];

            switch (packetID)
            {
                case (int)ClientPacketID.PlayerInput:
                    PlayerInputMessage(clientID, packetContent);
                    break;
            }
        }

        void PlayerInputMessage(int clientID, string message)
        {
            string[] input = message.Split('~');

            Vector3 cameraPosition = new Vector3(float.Parse(input[0]), float.Parse(input[1]), float.Parse(input[2]));
            Vector3 cameraRotation = new Vector3(float.Parse(input[3]), float.Parse(input[4]), float.Parse(input[5]));

            Vector2 moveInput = new Vector2(float.Parse(input[6]), float.Parse(input[7]));
            float jumpInput = float.Parse(input[8]);

            players[clientID].GetComponent<Player>().AddInput(new PlayerInput()
            {
                playerCameraPosition = cameraPosition,
                playerCameraRotation = cameraRotation,

                playerMovementInput = moveInput,
                playerJumpInput = jumpInput
            });
        }
    }
}

public enum ServerPacketID
{
    ObjectState = 1,
    PhysicsState,
}

public enum ClientPacketID
{
    PlayerInput = 1,
}