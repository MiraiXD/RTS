using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitController : MonoBehaviour, ISelectable, IHighlightable
{
    public Entities.BattleUnitModel model;
    public NetworkEntity networkEntity { get; private set; }

    public Vector3 serverPosition { get; set; }

    [SerializeField] private SpriteRenderer selectedEffect;
    public Action<BattleUnitController, bool> onUnitSelected;
    [SerializeField] private SpriteRenderer highlightedEffect;
    public Action<BattleUnitController, bool> onUnitHighlighted;

    public void Init(NetworkIdentity networkID, bool isLocalAuthority, Entities.BattleUnitModel model)
    {
        this.model = model;

        networkEntity = GetComponent<NetworkEntity>();
        networkEntity.Init(networkID, isLocalAuthority);
    }    
    public void UpdateUnit(float deltaTime)
    {
        if (serverPosition != Vector3.zero && serverPosition != transform.position)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, serverPosition, model.movementSpeed.Value * deltaTime);
            Quaternion targetRot = Quaternion.LookRotation((newPos - transform.position).normalized);
            Quaternion newRot = Quaternion.RotateTowards(transform.rotation, targetRot, 800f * deltaTime);

            transform.rotation = newRot;
            transform.position = newPos;
        }
    }

    private bool isSelected, isHighlighted;
    public void Select()
    {
        isSelected = true;
        isHighlighted = false;

        highlightedEffect.enabled = isHighlighted;
        selectedEffect.enabled = isSelected;

        onUnitSelected?.Invoke(this, true);
    }

    public void Deselect()
    {
        isSelected = false;
        isHighlighted = false;

        highlightedEffect.enabled = isHighlighted;
        selectedEffect.enabled = isSelected;

        onUnitSelected?.Invoke(this, false);
    }
    public void HighlightOn()
    {
        if (!isSelected)
        {
            isHighlighted = true;
            highlightedEffect.enabled = isHighlighted;

            onUnitHighlighted?.Invoke(this, true);
        }
    }

    public void HighlightOff()
    {
        isHighlighted = false;
        highlightedEffect.enabled = isHighlighted;

        onUnitHighlighted?.Invoke(this, false);
    }
}
