using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    private LocalGameManager gameManager;
    [SerializeField] private List<BattleUnitData> allUnits;
    private Dictionary<Entities.BattleUnitModel.UnitType, GameObject> prefabs;

    private Dictionary<ushort, BattleUnitController> unitsByID;
    private List<BattleUnitController> unitsList;
    private List<BattleUnitController> selectedUnitsList;
    public int unitsCount => unitsList.Count;
    public int selectedUnitsCount => selectedUnitsList.Count;
    public BattleUnitController GetUnit(ushort ID) => unitsByID[ID];
    public void Init(LocalGameManager gameManager)
    {
        this.gameManager = gameManager;

        prefabs = new Dictionary<Entities.BattleUnitModel.UnitType, GameObject>();
        foreach (var data in allUnits)
        {
            prefabs.Add(data.unitType, data.prefab);
        }

        unitsByID = new Dictionary<ushort, BattleUnitController>();
        unitsList = new List<BattleUnitController>();
        selectedUnitsList = new List<BattleUnitController>();
    }

    public BattleUnitController CreateUnit(NetworkIdentity networkID, bool isLocalAuthority, Entities.BattleUnitModel model, Vector3 position, Quaternion rotation)
    {
        if (prefabs.TryGetValue(model.unitType, out GameObject prefab))
        {
            var unitGO = Instantiate(prefab);
            unitGO.transform.parent = transform;
            unitGO.transform.position = position;
            unitGO.transform.rotation = rotation;
            var controller = unitGO.GetComponent<BattleUnitController>();
            controller.Init(networkID, isLocalAuthority, model);

            controller.onUnitSelected += OnUnitSelected;

            unitsByID.Add(controller.networkEntity.networkID.ID, controller);
            unitsList.Add(controller);
            return controller;
        }
        else { Debug.LogError("No such unit"); return null; }
    }

    private void OnUnitSelected(BattleUnitController unit, bool isSelected)
    {
        if (isSelected) { if (!selectedUnitsList.Contains(unit)) selectedUnitsList.Add(unit); }
        else { if (selectedUnitsList.Contains(unit)) selectedUnitsList.Remove(unit); }
    }

    public void UpdateUnits(float deltaTime)
    {
        foreach (var unit in unitsList)
        {
            unit.UpdateUnit(deltaTime);
        }
    }

    public void SelectAllUnits()
    {
        foreach (var unit in unitsList)
        {
            unit.Select();
        }
    }
    public void DeselectAllUnits()
    {
        foreach (var unit in unitsList)
        {
            unit.Deselect();
        }
    }
    public void MoveSelectedUnits(Vector3 position)
    {        
        gameManager.SendMoveUnits(selectedUnitsList.ToArray(), position);
    }
    public void MoveSelectedUnitsRandomly()
    {        
        foreach (var unit in selectedUnitsList)
        {
            Vector3 position = new Vector3(UnityEngine.Random.Range(35f, 70f), 3f, UnityEngine.Random.Range(35f, 70f));                        
            gameManager.SendMoveUnits(new BattleUnitController[] { unit }, position);
        }
    }
}
