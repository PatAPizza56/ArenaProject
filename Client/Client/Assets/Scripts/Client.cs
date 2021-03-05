using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Client : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] ClientConfig clientConfig = null;

    public static Client instance;

    Telepathy.Client client = new Telepathy.Client(50000);
    Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

    GameObject localPlayerObject = null;
    LocalPlayer localPlayer = null;

    public ClientSend clientSend = null;
    ClientHandle clientHandle = null;

    void Update()
    {
        client.Tick(100);
    }
    void OnApplicationQuit()
    {
        DisconnectFromServer();
    }

    public void ConnectToServer()
    {
        Application.runInBackground = true;

        Telepathy.Log.Info = Debug.Log;
        Telepathy.Log.Warning = Debug.LogWarning;
        Telepathy.Log.Error = Debug.LogError;

        clientSend = new ClientSend(this);
        clientHandle = new ClientHandle(this);

        client.OnConnected = clientHandle.OnConnect;
        client.OnData = clientHandle.OnReceiveData;
        client.OnDisconnected = clientHandle.OnDisconnect;

        instance = this;

        client.Connect("localhost", 1337);
    }

    public void DisconnectFromServer()
    {
        client.Disconnect();
    }

    public class ClientSend
    {
        Client client = null;
        public ClientSend(Client client)
        {
            this.client = client;
        }

        public void SendMessage(int packetID, string[] message)
        {
            string sendMessage = $"{packetID}_";

            for (int i = 0; i < message.Length; i++)
            {
                sendMessage += $"{message[i]}_";
            }

            sendMessage = sendMessage.Remove(sendMessage.Length - 1);

            client.client.Send(new ArraySegment<byte>(Encoding.UTF8.GetBytes(sendMessage)));
        }
    }
    public class ClientHandle
    {
        Client client = null;
        public ClientHandle(Client client)
        {
            this.client = client;
        }

        public void OnConnect()
        {
            Debug.Log("Connected to the server");

            client.localPlayerObject = Instantiate(client.clientConfig.localPlayerPrefab, Vector3.zero, Quaternion.identity);
            client.localPlayer = client.localPlayerObject.GetComponent<LocalPlayer>();
        }

        public void OnDisconnect()
        {
            Debug.Log("Disconnected from the server");

            foreach (KeyValuePair<int, GameObject> player in client.players)
            {
                Destroy(player.Value);
            }
            client.players.Clear();

            Destroy(client.localPlayerObject);
            client.localPlayer = null;
        }

        public void OnReceiveData(ArraySegment<byte> data)
        {
            string[] message = Encoding.UTF8.GetString(data.Array, data.Offset, data.Count).Split('_');

            try
            {
                switch (int.Parse(message[0]))
                {
                    case (int)ServerPacketID.WelcomeMessage:
                        WelcomeMessage(message);
                        break;
                    case (int)ServerPacketID.PlayerConnected:
                        PlayerConnectedMessage(message);
                        break;
                    case (int)ServerPacketID.PlayerDisconnected:
                        PlayerDisconnectedMessage(message);
                        break;
                    case (int)ServerPacketID.PlayerPosition:
                        PlayerPositionMessage(message);
                        break;
                    case (int)ServerPacketID.PlayerRotation:
                        PlayerRotationMessage(message);
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        void WelcomeMessage(string[] message)
        {
            for (int i = 0; i < message.Length; i++)
            {
                if (i != 0)
                {
                    client.players.Add(int.Parse(message[i]), Instantiate(client.clientConfig.playerPrefab, Vector3.zero, Quaternion.identity));
                }
            }
        }

        void PlayerConnectedMessage(string[] message)
        {
            client.players.Add(int.Parse(message[1]), Instantiate(client.clientConfig.playerPrefab, Vector3.zero, Quaternion.identity));
        }

        void PlayerDisconnectedMessage(string[] message)
        {
            Destroy(client.players[int.Parse(message[1])]);

            client.players.Remove(int.Parse(message[1]));
        }

        void PlayerPositionMessage(string[] message)
        {
            Vector3 playerPosition = new Vector3(float.Parse(message[2]), float.Parse(message[3]), float.Parse(message[4]));

            if (client.players.ContainsKey(int.Parse(message[1])))
            {
                client.players[int.Parse(message[1])].transform.position = playerPosition;
            }
            else
            {
                client.localPlayer.newPosition = playerPosition;
            }
        }

        void PlayerRotationMessage(string[] message)
        {
            float yRotation = float.Parse(message[2]);

            if (client.players.ContainsKey(int.Parse(message[1])))
            {
                client.players[int.Parse(message[1])].transform.eulerAngles = new Vector3(client.players[int.Parse(message[1])].transform.rotation.x, yRotation, client.players[int.Parse(message[1])].transform.rotation.z);
            }
        }
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
}

public enum ServerPacketID
{
    WelcomeMessage = 1,
    PlayerConnected,
    PlayerDisconnected,
    PlayerPosition,
    PlayerRotation,
}

public enum ClientPacketID
{
    PlayerInput = 1,
    PlayerRotation,
}