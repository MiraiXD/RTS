using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;

using System.Net;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using UnityEngine.UI;
using System;

public class Lobby : MonoBehaviour
{
    public UnityClient drClient;
    public NetworkManager networkManager;
    // PlayFab settings
    public string region; // The region where we will try to connect
    public string matchmakingQueue; // The name of the matchmaking queue we'll use
    public int matchmakingTimeout; // How long to attempt matchmaking before resetting
    public string playfabTCPPortName; // Playfab's name for the TCP port mapping
    public string playfabUDPPortName; // Playfab's name for the UDP port mapping
                                      // PlayFab Connection //    
    public Button connectButton;
    public Button localConnectButton;
    public InputField nameInputField;
    public Image loadingScreen;
    private void Start()
    {
        Init();
    }
    private void Init()
    {
        loadingScreen.gameObject.SetActive(false);
        connectButton.onClick.AddListener(() => { Connect(nameInputField.text); });
        localConnectButton.onClick.AddListener(() => { LocalConnect(); });
    }
    public void Connect(string clientName)
    {
        connectButton.GetComponentInChildren<Text>().text = "Connecting";
        // Attempt to login to PlayFab
        var request = new LoginWithCustomIDRequest { CustomId = clientName, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, (LoginResult result)=> { StartMatchmakingRequest(result.EntityToken.Entity.Id, result.EntityToken.Entity.Type); }, OnPlayFabError);

        connectButton.interactable = false;
    }

    private void OnPlayFabError(PlayFabError obj)
    {
        connectButton.GetComponentInChildren<Text>().text = "Connect";
        connectButton.interactable = true;
        Debug.LogError(obj.GenerateErrorReport());
    }

    private void StartMatchmakingRequest(string entityID, string entityType)
    {
        connectButton.GetComponentInChildren<Text>().text = "CreatingTicket";
        // Create a matchmaking request
        PlayFabMultiplayerAPI.CreateMatchmakingTicket(
            new CreateMatchmakingTicketRequest
            {
                Creator = new MatchmakingPlayer
                {
                    Entity = new PlayFab.MultiplayerModels.EntityKey
                    {
                        Id = entityID,
                        Type = entityType
                    },
                    Attributes = new MatchmakingPlayerAttributes
                    {
                        DataObject = new
                        {
                            Latencies = new object[] {
                            new {
                                region = region,
                                latency = 100
                            }
                            },
                        },
                    },
                },

                // Cancel matchmaking after this time in seconds with no match found
                GiveUpAfterSeconds = matchmakingTimeout,

                // name of the queue to poll
                QueueName = matchmakingQueue,
            },

            this.OnMatchmakingTicketCreated,
            this.OnPlayFabError
        );
    }
    private void OnMatchmakingTicketCreated(CreateMatchmakingTicketResult createMatchmakingTicketResult)
    {
        // Now we need to start polling the ticket periodically, using a coroutine
        StartCoroutine(PollMatchmakingTicket(createMatchmakingTicketResult.TicketId));
    }
    private IEnumerator PollMatchmakingTicket(string ticketId)
    {
        connectButton.GetComponentInChildren<Text>().text = "Matchmaking";

        yield return new WaitForSeconds(1);
        // Poll the ticket
        PlayFabMultiplayerAPI.GetMatchmakingTicket(
            new GetMatchmakingTicketRequest
            {
                TicketId = ticketId,
                QueueName = matchmakingQueue
            },

            // callbacks
            this.OnGetMatchmakingTicket,
            this.OnPlayFabError
        );        
    }
    private void OnGetMatchmakingTicket(GetMatchmakingTicketResult getMatchmakingTicketResult)
    {
        // When PlayFab returns our matchmaking ticket
        if (getMatchmakingTicketResult.Status == "Matched")
        {
            // If we found a match, we then need to access its server
            MatchFound(getMatchmakingTicketResult);
        }
        else if (getMatchmakingTicketResult.Status == "Canceled")
        {
            // If the matchmaking ticket was canceled we need to reset the input UI
            connectButton.GetComponentInChildren<Text>().text = "Connect";
            connectButton.interactable = true;
        }
        else
        {
            // If we don't have a conclusive matchmaking status, we keep polling the ticket
            StartCoroutine(PollMatchmakingTicket(getMatchmakingTicketResult.TicketId));
        }        
    }
    private void MatchFound(GetMatchmakingTicketResult getMatchmakingTicketResult)
    {
        connectButton.GetComponentInChildren<Text>().text = "MatchFound";
        // When we find a match, we need to request the connection variables to join clients
        PlayFabMultiplayerAPI.GetMatch(
            new GetMatchRequest
            {
                MatchId = getMatchmakingTicketResult.MatchId,
                QueueName = matchmakingQueue
            },

            this.OnGetMatch,
            this.OnPlayFabError
        );
        connectButton.GetComponentInChildren<Text>().text = "GettingMatch";
    }
    private void OnGetMatch(GetMatchResult getMatchResult)
    {
        connectButton.GetComponentInChildren<Text>().text = "Match";
        // Get the server to join
        string ipString = getMatchResult.ServerDetails.IPV4Address;
        int tcpPort = 0;
        int udpPort = 0;

        // Get the ports and names to join
        foreach (Port port in getMatchResult.ServerDetails.Ports)
        {
            if (port.Name == playfabTCPPortName)
                tcpPort = port.Num;

            if (port.Name == playfabUDPPortName)
                udpPort = port.Num;
        }

        // Connect and initialize the DarkRiftClient, hand over control to the NetworkManager
        if (tcpPort != 0 && udpPort != 0)
        {
            LoadSceneAsync("Default_4", delegate {
                networkManager.Init(nameInputField.text);
                networkManager.Connect(IPAddress.Parse(ipString), tcpPort, udpPort);
            });

            
            
        }        
    } 
    public void LocalConnect()
    {
        connectButton.GetComponentInChildren<Text>().text = "Connect1";
        networkManager.Init(nameInputField.text);
        networkManager.Connect(IPAddress.Parse("127.0.0.1"), 4296, 4296);        
    }

    public IEnumerator LoadSceneAsync(string scene, Action callback=null)
    {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);
        while(!asyncOperation.isDone)
        {
            yield return null;
        }

        callback?.Invoke();
    }
}
