using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        public void SendMessage(int clientID, string message)
        {
            server.Send(clientID, new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)));
        }

        public void SendMessageAll(string message)
        {
            for (int i = 0; i < players.Count; i++)
            {
                int clientId = players.ElementAt(i).Key;

                SendMessage(clientId, message);
            }
        }

        public void SendMessageAllExcept(int clientID, string message)
        {
            for (int i = 0; i < players.Count; i++)
            {
                int eClientId = players.ElementAt(i).Key;

                if (eClientId != clientID)
                {
                    SendMessage(eClientId, message);
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
            try
            {
                Message message = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(data.Array, data.Offset, data.Count));

                switch (message.PacketId)
                {
                    case (int)ClientPacketID.PlayerInput:
                        PlayerInputMessage(clientID, JsonConvert.DeserializeObject<Message.PlayerInputMessage>(message.PacketContent));
                        break;
                }
            }
            catch
            {
                Debug.Log($"Could not recieve message from client: {clientID}");
            }
        }

        void PlayerInputMessage(int clientID, Message.PlayerInputMessage message)
        {
            players[clientID].GetComponent<Player>().AddInput(new PlayerInput()
            {
                playerCameraPosition = message.CameraPosition.ToVector3(),
                playerCameraRotation = message.CameraRotation.ToVector3(),

                playerMovementInput = message.MoveInput.ToVector2(),
                playerJumpInput = message.JumpInput,
            });
        }
    }
}

public class Message
{
    public int PacketId { get; set; }
    public string PacketContent { get; set; }

    public class ObjectStateMessage
    {
        public long ClientId { get; set; }
        public SyncedObjectMessage[] SyncedObjects { get; set; }
    }

    public class PhysicsStateMessage
    {
        public SyncedObjectMessage[] SyncedObjects { get; set; }
    }

    public class SyncedObjectMessage
    {
        public long Id { get; set; }
        public int Type { get; set; }

        public SyncedVector3 Position { get; set; }
        public SyncedVector3 Rotation { get; set; }
    }

    public class PlayerInputMessage
    {
        public SyncedVector3 CameraPosition { get; set; }
        public SyncedVector3 CameraRotation { get; set; }

        public SyncedVector2 MoveInput { get; set; }
        public float JumpInput { get; set; }
    }

    public struct SyncedVector2
    {
        public SyncedVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; set; }
        public float Y { get; set; }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }
    }

    public struct SyncedVector3
    {
        public SyncedVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
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