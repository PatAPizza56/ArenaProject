using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Server : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] ServerConfig serverConfig;

    public static Server instance;

    Telepathy.Server server = new Telepathy.Server(50000);
    Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public int currentPlayers { get { return players.Count; } }

    public ServerSend serverSend;
    ServerHandle serverHandle;

    public event Action onPlayerPrimaryFire;

    void Update() { if (server.Active) server.Tick(100); }
    void OnApplicationQuit() { StopServer(); }

    void StartServer()
    {
        Application.runInBackground = true;

        Telepathy.Log.Info = Debug.Log;
        Telepathy.Log.Warning = Debug.LogWarning;
        Telepathy.Log.Error = Debug.LogError;

        serverSend = new ServerSend(this);
        serverHandle = new ServerHandle(this);

        server.OnConnected = serverHandle.OnClientConnect;
        server.OnData = serverHandle.OnReceiveData;
        server.OnDisconnected = serverHandle.OnClientDisconnect;

        instance = this;

        server.Start(1337);
    }

    void StopServer()
    {
        server.Stop();

        foreach (KeyValuePair<int, GameObject> player in players)
        {
            Destroy(player.Value);
        }

        players.Clear();
    }

    public class ServerSend
    {
        Server server;
        public ServerSend(Server server) { this.server = server; }

        public void SendMessage(int clientID, int packetID, string[] message)
        {
            string sendMessage = $"{packetID}_";

            for (int i = 0; i < message.Length; i++)
            {
                sendMessage += $"{message[i]}_";
            }

            sendMessage = sendMessage.Remove(sendMessage.Length - 1);

            server.server.Send(clientID, new ArraySegment<byte>(Encoding.UTF8.GetBytes(sendMessage)));
        }

        public void SendMessageAll(int packetID, string[] message)
        {
            for (int i = 0; i < server.players.Count; i++)
            {
                int clientID = server.players.ElementAt(i).Key;

                SendMessage(clientID, packetID, message);
            }
        }

        public void SendMessageAllExcept(int clientID, int packetID, string[] message)
        {
            for (int i = 0; i < server.players.Count; i++)
            {
                int _clientID = server.players.ElementAt(i).Key;

                if (_clientID != clientID)
                {
                    SendMessage(_clientID, packetID, message);
                }
            }
        }
    }

    public class ServerHandle
    {
        Server server;
        public ServerHandle(Server server) { this.server = server; }

        public void OnClientConnect(int clientID)
        {
            Debug.Log($"{clientID} connected to the server!");

            List<string> currentPlayers = new List<string>();
            for (int i = 0; i < server.players.Count; i++)
            {
                int playerID = server.players.ElementAt(i).Key;
                currentPlayers.Add($"{playerID}");
            }

            server.serverSend.SendMessageAll((int)ServerPacketID.PlayerConnected, new string[] { clientID.ToString() });
            server.serverSend.SendMessage(clientID, (int)ServerPacketID.WelcomeMessage, currentPlayers.ToArray());

            int spawnPoint = UnityEngine.Random.Range(0, server.serverConfig.spawnPoints.Length - 1);
            server.players.Add(clientID, Instantiate(server.serverConfig.playerPrefab, server.serverConfig.spawnPoints[spawnPoint], Quaternion.identity));
        }

        public void OnClientDisconnect(int clientID)
        {
            Debug.Log($"{clientID} disconnected from the server!");

            Destroy(server.players[clientID]);
            server.players.Remove(clientID);

            server.serverSend.SendMessageAll((int)ServerPacketID.PlayerDisconnected, new string[] { clientID.ToString() });
        }

        public void OnReceiveData(int clientID, ArraySegment<byte> data)
        {
            string[] message = Encoding.UTF8.GetString(data.Array, data.Offset, data.Count).Split('_');

            try
            {
                switch (int.Parse(message[0]))
                {
                    case (int)ClientPacketID.PlayerInput:
                        PlayerInputMessage(clientID, message);
                        break;
                    case (int)ClientPacketID.PlayerRotation:
                        PlayerRotationMessage(clientID, message);
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        void PlayerInputMessage(int clientID, string[] message)
        {
            List<float> input = new List<float>();

            for (int i = 0; i < message.Length; i++)
            {
                if (i != 0)
                {
                    input.Add(float.Parse(message[i]));
                }
            }

            server.players[clientID].GetComponent<Player>().AddInput(new Vector2(input[0], input[1]), input[2]);

            server.serverSend.SendMessageAll((int)ServerPacketID.PlayerPosition, new string[] { clientID.ToString(), server.players[clientID].transform.position.x.ToString(), server.players[clientID].transform.position.y.ToString(), server.players[clientID].transform.position.z.ToString() });
        }

        void PlayerRotationMessage(int clientID, string[] message)
        {
            float yRotation = float.Parse(message[1]);

            server.players[clientID].transform.eulerAngles = new Vector3(server.players[clientID].transform.rotation.x, yRotation, server.players[clientID].transform.rotation.z);

            server.serverSend.SendMessageAllExcept(clientID, (int)ServerPacketID.PlayerRotation, new string[] { clientID.ToString(), yRotation.ToString() });
        }
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
}

public enum ServerPacketID
{
    WelcomeMessage = 1,
    PlayerConnected,
    PlayerDisconnected,
    PhysicsState,
    PlayerPosition,
    PlayerRotation
}

public enum ClientPacketID
{
    PlayerInput = 1,
    PlayerRotation,
}