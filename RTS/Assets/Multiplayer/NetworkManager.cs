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
        //clientMessage_Actions.Add(Tags.PlayerConnectTag, OnPlayerConnect);
        clientMessage_Actions.Add(Messages.Server.ConnectedPlayers.Tag, OnConnectedPlayersMessage);
        clientMessage_Actions.Add(Messages.Server.PlayerDisconnected.Tag, OnPlayerDisconnected);
        clientMessage_Actions.Add(Messages.Server.Update.Tag, OnUpdateMessage);
        clientMessage_Actions.Add(Messages.Server.LoadMap.Tag, OnLoadMapMessage);
        client.MessageReceived += OnMessageReceived;
    }

    private void OnLoadMapMessage(Message message, object sender, MessageReceivedEventArgs e)
    {
        using (DarkRiftReader reader = message.GetReader())
        {
            Messages.Server.LoadMap serverMessage = reader.ReadSerializable<Messages.Server.LoadMap>();

        }
    }

    float lastUpdateTime;
    int i;
    private void OnUpdateMessage(Message message, object sender, MessageReceivedEventArgs e)
    {
        float ping = Time.realtimeSinceStartup - lastUpdateTime;
        ping *= 1000f;
        debugText.text = ping.ToString() + " : " + i++.ToString();
        lastUpdateTime = Time.realtimeSinceStartup;
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
        client.ConnectInBackground(ip, tcpPort, udpPort, true, delegate{ OnPlayFabConnectSuccessful(); });
        debugText.text = ("CONNECTING2");
    }

    private void OnPlayFabConnectSuccessful()
    {
        if(client.ConnectionState == ConnectionState.Connected)
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
            writer.Write(new Messages.Player.Hello() {playerName= playerName });
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
}
