using System;
using System.Collections.Generic;
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
        public void SendMessage(int packetID, string message)
        {
            string sendMessage = $"{packetID}_{message}";

            client.Send(new ArraySegment<byte>(Encoding.UTF8.GetBytes(sendMessage)));
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
            string[] message = Encoding.UTF8.GetString(data.Array, data.Offset, data.Count).Split('_');

            int packetId = int.Parse(message[0]);
            string packetContent = message[1];

            switch (packetId)
            {
                case (int)ServerPacketID.ObjectState:
                    ObjectStateMessage(packetContent);
                    break;
                case (int)ServerPacketID.PhysicsState:
                    PhysicsStateMessage(packetContent);
                    break;
            }
        }

        void ObjectStateMessage(string message)
        {
            SyncedObjectManager.syncedObjectStateMessage = message;
            SyncedObjectManager.modifiedSyncedObjectStates = true;
        }

        void PhysicsStateMessage(string message)
        {
            SyncedObjectManager.syncedObjectPhysicsMessage = message;
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