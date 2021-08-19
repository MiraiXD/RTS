using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGameManager : MonoBehaviour
{
    private NetworkManager networkManager;
    private UnitManager unitManager;
    //[SerializeField]

    public void Init(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
        unitManager = FindObjectOfType<UnitManager>();
        unitManager.Init();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            networkManager.SendSpawnUnit(Entities.BattleUnitModel.UnitType.Knight);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 100f))
            {
                int nodeXCoord = (int)((hitInfo.point.x - 32.17f) / 1.5f);
                int nodeYCoord = (int)((hitInfo.point.z - 32.17f) / 1.5f);
                Debug.Log(nodeXCoord + " , " + nodeYCoord);
                Debug.Log(hitInfo.point.x + ", " + hitInfo.point.y + " , " + hitInfo.point.z);                
                networkManager.SendMoveUnit(unit.networkEntity.networkID, nodeXCoord, nodeYCoord);
            }
        }

        if(currentChange!= null)
        {
            Vector3 newPos = Vector3.Lerp(unit.transform.position, new Vector3(currentChange.xPos, currentChange.yPos, currentChange.zPos), Time.deltaTime*5f);
            Debug.Log("Speed: " + (unit.transform.position - newPos).magnitude/Time.deltaTime);
            unit.transform.position = newPos;
        }
    }
    BattleUnitController unit;
    SingleWorldChange currentChange;
    public void OnSpawnUnit(Messages.Server.SpawnUnit message)
    {
        Debug.Log("SPAWN UNIT " + message.unitModel.unitType);
         unit = unitManager.CreateUnit(message.unitModel, new Vector3(message.unitModel.position.FirstValue, message.unitModel.position.SecondValue,message.unitModel.position.ThirdValue), Quaternion.identity);
        unit.Init(message.unitID, networkManager.localPlayerID == message.owningPlayerID.ID, message.unitModel);
    }
    public void OnWorldUpdate(Messages.Server.WorldUpdate message)
    {
        currentChange = message.changes[0];
        Debug.Log(currentChange.xPos + ", " + currentChange.yPos + " , " + currentChange.zPos);
    }
}
