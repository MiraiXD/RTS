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
    public ushort localPlayerID;
    private Dictionary<ushort, Entities.NetworkedPlayerModel> players;
    
    Dictionary<ushort, Action<Message, MessageReceivedEventArgs>> clientMessage_Actions;
    private LocalGameManager gameManager;
    private float lastWorldUpdateTime;
    private float lastTimeStamp = 0f;
    public float networkDeltaTime { get; private set; }    
    private void OnStartGameMessage(Message message, MessageReceivedEventArgs e)
    {
        Debug.Log("START GAME");

        gameManager = FindObjectOfType<LocalGameManager>();
        gameManager.Init(this);
    }

    public void Init(string playerName)
    {        
        this.playerName = playerName;

        DontDestroyOnLoad(gameObject);

        clientMessage_Actions = new Dictionary<ushort, Action<Message, MessageReceivedEventArgs>>();        

        clientMessage_Actions.Add(Messages.Server.ConnectedPlayers.Tag, OnConnectedPlayersMessage);
        clientMessage_Actions.Add(Messages.Server.PlayerDisconnected.Tag, OnPlayerDisconnected);
        clientMessage_Actions.Add(Messages.Server.LoadMap.Tag, OnLoadMapMessage);
        clientMessage_Actions.Add(Messages.Server.WorldInfo.Tag, OnWorldInfoMessage);
        clientMessage_Actions.Add(Messages.Server.StartGame.Tag, OnStartGameMessage);
        clientMessage_Actions.Add(Messages.Server.WorldUpdate.Tag, OnWorldUpdate);
        clientMessage_Actions.Add(Messages.Server.SpawnUnit.Tag, OnSpawnUnitMessage);        
        client.MessageReceived += OnMessageReceived;        
    }

    private void OnWorldInfoMessage(Message arg1, MessageReceivedEventArgs arg2)
    {
        Debug.Log("WORLD INFO");
    }

    private void OnSpawnUnitMessage(Message message, MessageReceivedEventArgs e)
    {
        using (DarkRiftReader reader = message.GetReader())
        {
            Messages.Server.SpawnUnit spawnUnitMessage = reader.ReadSerializable<Messages.Server.SpawnUnit>();

            gameManager.OnSpawnUnit(spawnUnitMessage);
                }
    }

    public void SendSpawnUnit(Entities.BattleUnitModel.UnitType unitType)
    {
        using(DarkRiftWriter writer = DarkRiftWriter.Create())
        {
            Messages.Client.SpawnUnit spawnUnitMessage = new Messages.Client.SpawnUnit();
            spawnUnitMessage.unitType = unitType;
            writer.Write(spawnUnitMessage);
            using(Message message = Message.Create(Messages.Client.SpawnUnit.Tag, writer))
            {
                client.SendMessage(message, SendMode.Reliable);
            }
        }
    }
    public void SendMoveUnit(NetworkIdentity unitID, int nodeXCoord, int nodeYCoord)
    {
        using (DarkRiftWriter writer = DarkRiftWriter.Create())
        {
            Messages.Client.MoveUnit moveUnitMessage = new Messages.Client.MoveUnit();
            moveUnitMessage.unitID = unitID;
            moveUnitMessage.nodeXCoord = nodeXCoord;
            moveUnitMessage.nodeYCoord = nodeYCoord;
            writer.Write(moveUnitMessage);
            using (Message message = Message.Create(Messages.Client.MoveUnit.Tag, writer))
            {
                client.SendMessage(message, SendMode.Reliable);
            }
        }
    }
    private void OnWorldUpdate(Message message, MessageReceivedEventArgs e)
    {        
        using (DarkRiftReader reader = message.GetReader())
        {
            Messages.Server.WorldUpdate worldUpdateMessage = reader.ReadSerializable<Messages.Server.WorldUpdate>();            
            if (worldUpdateMessage.timeSinceStartup > lastTimeStamp)
            {
                gameManager.OnWorldUpdate(worldUpdateMessage);

                networkDeltaTime = Time.realtimeSinceStartup - lastWorldUpdateTime;
                lastWorldUpdateTime = Time.realtimeSinceStartup;                
                lastTimeStamp = worldUpdateMessage.timeSinceStartup;
            }
            else
            {
                Debug.LogError("LOST PACKET");
            }
        }
    }
    
    

    private void OnLoadMapMessage(Message message, MessageReceivedEventArgs e)
    {
        foreach (var kvp in players)
        {
            if (kvp.Value.playerName == playerName) localPlayerID = kvp.Key;
        }

        using (DarkRiftReader reader = message.GetReader())
        {
            Messages.Server.LoadMap loadMapMessage = reader.ReadSerializable<Messages.Server.LoadMap>();
            StartCoroutine(LoadSceneAsync(loadMapMessage.mapName, () => { OnSceneLoaded(loadMapMessage); }));
        }
    }
    private void OnSceneLoaded(Messages.Server.LoadMap message)
    {
        //Create resources

        Messages.Client.ReadyToStartGame playerMessage = new Messages.Client.ReadyToStartGame();
        using (DarkRiftWriter writer = DarkRiftWriter.Create())
        {
            writer.Write(playerMessage);
            using (Message m = Message.Create(Messages.Client.ReadyToStartGame.Tag, writer))
            {
                client.SendMessage(m, SendMode.Reliable);
            }
        }
    }

    private void OnPlayerDisconnected(Message arg1, MessageReceivedEventArgs arg3)
    {
     //   debugText.text = ("Player disconnected ");
    }

    private void OnConnectedPlayersMessage(Message message ,MessageReceivedEventArgs e)
    {        
        using (DarkRiftReader reader = message.GetReader())
        {
            Messages.Server.ConnectedPlayers connectedPlayers = reader.ReadSerializable<Messages.Server.ConnectedPlayers>();
            Debug.Log("MaxPlayer " + connectedPlayers.maxPlayers);
            players = new Dictionary<ushort, Entities.NetworkedPlayerModel>();
            for(int i=0; i<connectedPlayers.connectedPlayers_Size;i++)
            {
                players.Add(connectedPlayers.IDs[i].ID, connectedPlayers.connectedPlayers[i]);
            }
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
            writer.Write(new Messages.Client.Hello() { playerName = playerName });
            using (Message message = Message.Create(Messages.Client.Hello.Tag, writer))
            {
                client.SendMessage(message, SendMode.Reliable);
            }
        }
    }
    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage())
        {
            if (clientMessage_Actions.TryGetValue(message.Tag, out Action<Message, MessageReceivedEventArgs> action))
            {
                action(message, e);
            }
            else
            {
                debugText.text = ("No such tag!");
            }
        }
    }
    public IEnumerator LoadSceneAsync(string scene, Action callback = null)
    {
        Debug.Log("Loading map: " + scene);
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);        
        while (!asyncOperation.isDone)
        {            
            yield return null;
        }

        callback?.Invoke();
    }
}
