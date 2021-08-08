using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using System;

using System.Net;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using System.Reflection;

public class NetworkManager : MonoBehaviour
{
    public UnityEngine.UI.Text debugText;
    public string playerName { get; set; }
    [SerializeField] private UnityClient client;

    public Dictionary<ushort, NetworkEntity> networkPlayers = new Dictionary<ushort, NetworkEntity>();
    Dictionary<ushort, Action<Message, object, MessageReceivedEventArgs>> clientMessage_Actions;
    public void Init(string playerName)
    {
        this.playerName = playerName;

        DontDestroyOnLoad(gameObject);

        clientMessage_Actions = new Dictionary<ushort, Action<Message, object, MessageReceivedEventArgs>>();        

        clientMessage_Actions.Add(Messages.Server.ConnectedPlayers.Tag, OnConnectedPlayersMessage);
        clientMessage_Actions.Add(Messages.Server.PlayerDisconnected.Tag, OnPlayerDisconnected);
        clientMessage_Actions.Add(Messages.Server.LoadMap.Tag, OnLoadMapMessage);
        clientMessage_Actions.Add(Messages.Server.StartGame.Tag, OnStartGameMessage);
        clientMessage_Actions.Add(Messages.Server.WorldUpdate.Tag, OnWorldUpdate);
        client.MessageReceived += OnMessageReceived;
    }
    float lastWorldUpdateTime;
    private void OnWorldUpdate(Message arg1, object arg2, MessageReceivedEventArgs arg3)
    {
        Debug.Log(Time.realtimeSinceStartup - lastWorldUpdateTime);
        lastWorldUpdateTime = Time.realtimeSinceStartup;
    }
    GameObject cube;
    private void OnStartGameMessage(Message message, object sender, MessageReceivedEventArgs e)
    {
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    }

    private void OnLoadMapMessage(Message message, object sender, MessageReceivedEventArgs e)
    {        
        using (DarkRiftReader reader = message.GetReader())
        {
            Messages.Server.LoadMap serverMessage = reader.ReadSerializable<Messages.Server.LoadMap>();            
            StartCoroutine(LoadSceneAsync(serverMessage.mapName, () =>
            {               
                Messages.Player.ReadyToStartGame playerMessage = new Messages.Player.ReadyToStartGame();
                using (DarkRiftWriter writer = DarkRiftWriter.Create())
                {
                    writer.Write(playerMessage);
                    using (Message m = Message.Create(Messages.Player.ReadyToStartGame.Tag, writer))
                    {
                        client.SendMessage(m, SendMode.Reliable);   
                    }
                }
            }));
        }
    }

    private void OnPlayerDisconnected(Message arg1, object arg2, MessageReceivedEventArgs arg3)
    {
        debugText.text = ("Player disconnected ");
    }

    private void OnConnectedPlayersMessage(Message message, object sender, MessageReceivedEventArgs e)
    {
        debugText.text = ("CONNECTED PLAYERS ARRIVED");
        using (DarkRiftReader reader = message.GetReader())
        {
            Messages.Server.ConnectedPlayers connectedPlayers = reader.ReadSerializable<Messages.Server.ConnectedPlayers>();
            Debug.Log("MaxPlayer " + connectedPlayers.maxPlayers);
            foreach (var player in connectedPlayers.connectedPlayers)
                Debug.Log("Player " + player.ID + ":" + player.playerName);
        }
    }

    public void Connect(IPAddress ip, int tcpPort, int udpPort)
    {
        debugText.text = ("CONNECTING");
        client.ConnectInBackground(ip, tcpPort, udpPort, true, delegate { OnPlayFabConnectSuccessful(); });
        debugText.text = ("CONNECTING2");
    }

    private void OnPlayFabConnectSuccessful()
    {
        if (client.ConnectionState == ConnectionState.Connected)
        {
            debugText.text = ("SENDING HELLO");
            SendPlayerHello(playerName);
            debugText.text = ("HELLO SENT");
        }
        else
        {
            debugText.text = ("NOT CONNECTED");
        }
    }


    public void SendPlayerHello(string playerName)
    {
        using (DarkRiftWriter writer = DarkRiftWriter.Create())
        {
            writer.Write(new Messages.Player.Hello() { playerName = playerName });
            using (Message message = Message.Create(Messages.Player.Hello.Tag, writer))
            {
                client.SendMessage(message, SendMode.Reliable);
            }
        }
    }
    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage())
        {
            if (clientMessage_Actions.TryGetValue(message.Tag, out Action<Message, object, MessageReceivedEventArgs> action))
            {
                action(message, sender, e);
            }
            else
            {
                debugText.text = ("No such tag!");
            }
        }
    }
    public IEnumerator LoadSceneAsync(string scene, Action callback = null)
    {
        Debug.Log("LOAD MAP2");
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);
        Debug.Log("LOAD MAP3");        
        while (!asyncOperation.isDone)
        {
            Debug.Log("LOADING " + asyncOperation.progress);
            yield return null;
        }

        callback?.Invoke();
    }
}
