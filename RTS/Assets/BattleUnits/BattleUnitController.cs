using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitController : MonoBehaviour
{
    public Entities.BattleUnitModel model;
    public NetworkEntity networkEntity { get; private set; }
    public void Init(NetworkIdentity networkID, bool isLocalAuthority, Entities.BattleUnitModel model)
    {
        this.model = model;

        networkEntity = GetComponent<NetworkEntity>();
        networkEntity.Init(networkID, isLocalAuthority);
    }
}
