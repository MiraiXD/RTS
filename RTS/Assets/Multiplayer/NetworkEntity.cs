using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkEntity : MonoBehaviour
{
    public NetworkIdentity networkID;    
    public bool isLocalAuthority { get; private set; }
    public void Init(NetworkIdentity networkID, bool isLocalAuthority)
    {
        this.networkID = networkID;
        this.isLocalAuthority = isLocalAuthority;
    }
}