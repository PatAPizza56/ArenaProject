using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

public class Client : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] ClientConfig config = null;

    static ClientConfig staticConfig = null;

    static Telepathy.Client client = new Telepathy.Client(50000);

    public static Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public static GameObject localPlayerObject = null;

    public static ClientSend Send = null;
    static ClientHandle Handle = null;

    void Awake()
    {
        staticConfig = config;
    }

    void Update()
    {
        client.Tick(100);
    }

    void OnApplicationQuit()
    {
        DisconnectFromServer();
    }

    void OnGUI()
    {
        GUI.enabled = !client.Connected;
        if (GUI.Button(new Rect(0f, 0f, 120f, 20f), "Connect Client"))
            ConnectToServer();

        GUI.enabled = client.Connected;
        if (GUI.Button(new Rect(130f, 0f, 120f, 20f), "Disconnect Client"))
            DisconnectFromServer();

        GUI.enabled = true;
    }

    public static void ConnectToServer()
    {
        Application.runInBackground = true;

        Telepathy.Log.Info = Debug.Log;
        Telepathy.Log.Warning = Debug.LogWarning;
        Telepathy.Log.Error = Debug.LogError;

        Send = new ClientSend();
        Handle = new ClientHandle();

        client.OnConnected = Handle.OnConnect;
        client.OnData = Handle.OnReceiveData;
        client.OnDisconnected = Handle.OnDisconnect;

        client.Connect("localhost", 1337);
    }

    public static void DisconnectFromServer()
    {
        client.Disconnect();
    }

    public class ClientSend
    {
        public void SendMessage(string message)
        {
            client.Send(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)));
        }
    }

    public class ClientHandle
    {
        public void OnConnect()
        {
            localPlayerObject = Instantiate(staticConfig.localPlayerPrefab, Vector3.zero, Quaternion.identity);
        }

        public void OnDisconnect()
        {
            foreach (KeyValuePair<int, GameObject> player in players)
            {
                Destroy(player.Value);
            }
            players.Clear();

            Destroy(localPlayerObject);
        }

        public void OnReceiveData(ArraySegment<byte> data)
        {
            try
            {
                Message message = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(data.Array, data.Offset, data.Count));

                switch (message.PacketId)
                {
                    case (int)ServerPacketID.ObjectState:
                        ObjectStateMessage(JsonConvert.DeserializeObject<Message.ObjectStateMessage>(message.PacketContent));
                        break;
                    case (int)ServerPacketID.PhysicsState:
                        PhysicsStateMessage(JsonConvert.DeserializeObject<Message.PhysicsStateMessage>(message.PacketContent));
                        break;
                }
            }
            catch
            {
                Debug.Log("Could not recieve message from server");
            }
        }

        void ObjectStateMessage(Message.ObjectStateMessage message)
        {
            SyncedObjectManager.syncedObjectStateMessage = message;
            SyncedObjectManager.modifiedSyncedObjectStates = true;
        }

        void PhysicsStateMessage(Message.PhysicsStateMessage message)
        {
            SyncedObjectManager.syncedObjectPhysicsMessage = message;
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