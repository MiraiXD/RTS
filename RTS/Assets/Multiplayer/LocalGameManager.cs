using DarkRift;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGameManager : MonoBehaviour
{
    private NetworkManager networkManager;
    private UnitManager unitManager;
    private KeyboardMouse_InputManager inputManager;
    //[SerializeField]

    private void Awake()
    {
        enabled = false;
    }  
    public void Init(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
        unitManager = FindObjectOfType<UnitManager>();
        unitManager.Init(this);
        inputManager = FindObjectOfType<KeyboardMouse_InputManager>();
        inputManager.Init(this,unitManager);
        enabled = true;
    }


    private void Update()
    {        
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 100f))
        //    {
        //        int nodeXCoord = (int)((hitInfo.point.x - 32.17f) / 1.5f);
        //        int nodeYCoord = (int)((hitInfo.point.z - 32.17f) / 1.5f);
        //        Debug.Log(nodeXCoord + " , " + nodeYCoord);
        //        Debug.Log(hitInfo.point.x + ", " + hitInfo.point.y + " , " + hitInfo.point.z);
        //        networkManager.SendMoveUnits(unit.networkEntity.networkID, nodeXCoord, nodeYCoord);
        //    }
        //}

        unitManager.UpdateUnits(Time.deltaTime);
        inputManager.UpdateInput();
    }
    public void SendMoveUnits(BattleUnitController[] units, Vector3 position)
    {
        Messages.Client.MoveUnits message = new Messages.Client.MoveUnits();
        message.unitsCount = (ushort)units.Length;
        message.unitIDs = new NetworkIdentity[message.unitsCount];
        message.worldPositionX = position.x;
        message.worldPositionY = position.y;
        message.worldPositionZ = position.z;
        //message.nodeXCoords = new ushort[message.unitsCount];
        //message.nodeYCoords = new ushort[message.unitsCount];
        for (int i = 0; i < message.unitsCount; i++)
        {
            message.unitIDs[i] = units[i].networkEntity.networkID;
            //message.nodeXCoords[i] = (ushort)nodeXCoord;
            //message.nodeYCoords[i] = (ushort)nodeYCoord;
        }
        networkManager.SendMessageToServer(message, Messages.Client.MoveUnits.Tag, SendMode.Reliable);
    }
    public void SendSpawnUnit(Entities.BattleUnitModel.UnitType unitType)
    {
        Messages.Client.SpawnUnit spawnUnitMessage = new Messages.Client.SpawnUnit();
        spawnUnitMessage.unitType = unitType;
        networkManager.SendMessageToServer(spawnUnitMessage, Messages.Client.SpawnUnit.Tag, SendMode.Reliable);
    }
    BattleUnitController unit;
    public void OnSpawnUnit(Messages.Server.SpawnUnit message)
    {
        Debug.Log("SPAWN UNIT " + message.unitModel.unitType);
        unit = unitManager.CreateUnit(message.unitID, networkManager.localPlayerID == message.owningPlayerID.ID, message.unitModel, new Vector3(message.unitModel.position.FirstValue, message.unitModel.position.SecondValue, message.unitModel.position.ThirdValue), Quaternion.identity);
    }
    public void OnWorldUpdate(Messages.Server.WorldUpdate message)
    {
        for (int i = 0; i < message.changeCount; i++)
        {
            var unit = unitManager.GetUnit(message.IDs[i]);
            var change = message.changes[i];
            unit.serverPosition = new Vector3(change.xPos, change.yPos, change.zPos);
            unit.model.currentHealth.Value = change.currentHealth;
        }
    }
}
