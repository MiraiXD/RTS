using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerControlledTeam : MonoBehaviour
{
    //public LayerMask selectableLayerMask;
    //public LayerMask highlightableLayerMask;
    [SerializeField] private Camera camera;

    [SerializeField] private Color teamColor;
    private Team team;

    public LayerMask interactableLayerMask;
    private Collider currentColliderHit;
    private IHighlightable currentHighlightable;
    private ISelectable currentSelectable;    

    [SerializeField] private TeamCamp camp;
 
    public void Init()
    {
        team = new Team(teamColor);
        camp.Init(team);
    }

    //public LayerMask interactableLayerMask;
    //private Collider currentColliderHovered;
    //private IHighlightable currentHighlightable;
    //private ISelectable currentSelectable;
    //private List<BattleUnit> selectedUnits = new List<BattleUnit>();

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetKeyDown(KeyCode.Space)) camp.TrainNewUnit( (UnitType )(Mathf.RoundToInt( Time.time)%3)  );
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (camp.selectedUnits_Count == camp.units_Count) camp.DeselectAllUnits();
            else camp.SelectAllUnits();
        }
        // if (Input.GetKeyDown(KeyCode.S)) castle.FollowTargetSelectedUnits(castle.target);

        CameraRay(interactableLayerMask, out RaycastHit interactableHit);

        HandleHovering(interactableHit);
        HandleClicking(interactableHit);
    }

    private void HandleHovering(RaycastHit hit)
    {
        if (hit.collider != currentColliderHit)
        {
            if (currentHighlightable != null)
            {
                currentHighlightable.HighlightOff();
                currentHighlightable = null;
            }

            if (hit.collider != null && hit.collider.TryGetComponent(out currentHighlightable))
            {
                currentHighlightable.HighlightOn();
            }

            currentColliderHit = hit.collider;
        }
    }
    private void HandleClicking(RaycastHit hit)
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (hit.collider == null && currentSelectable != null)
            {
                currentSelectable.Deselect();
                currentSelectable = null;
            }
            else if (hit.collider != null)
            {
                //if(interactableHit.collider.TryGetComponent(out BattleUnit battleUnit))
                //{
                //    currentSelectable?.Deselect();
                //    selectedUnits.Clear();

                //    if (battleUnit == currentSelectable)
                //    {
                //        currentSelectable = null;
                //    }
                //    else
                //    {
                //        currentSelectable = battleUnit;
                //    }

                //    if (currentSelectable != null)
                //    {
                //        currentSelectable.Select();
                //        selectedUnits.Add(battleUnit);
                //    }                    
                //}                                
                if (hit.collider.TryGetComponent(out ISelectable selectable))
                {                    
                    currentSelectable?.Deselect();

                    if (selectable == currentSelectable)
                    {
                        currentSelectable = null;
                    }
                    else
                    {
                        currentSelectable = selectable;
                    }

                    currentSelectable?.Select();
                }
                
                else if (hit.collider.CompareTag("Terrain"))
                {
                    if (camp.selectedUnits_Count > 0)
                    {                        
                        camp.MoveSelectedUnits(hit.point);
                    }
                    else
                    {
                        currentSelectable?.Deselect();
                        currentSelectable = null;
                    }
                }
            }
        }
    }
    //private void HandleHighlighting()
    //{
    //    if (CameraRay(interactableLayerMask, out RaycastHit highlightableHit))
    //    {
    //        if (currentColliderHit != highlightableHit.collider)
    //        {
    //            currentColliderHit = highlightableHit.collider;
    //            if (highlightableHit.collider.TryGetComponent(out IHighlightable highlightable))
    //            {

    //                if (currentHighlightable != null)
    //                {
    //                    currentHighlightable.HighlightOff();
    //                }
    //                currentHighlightable = highlightable;
    //                currentHighlightable.HighlightOn();
    //            }
    //        }
    //    }
    //    else
    //    {
    //        if (currentColliderHit != null) currentColliderHit = null;
    //        if (currentHighlightable != null)
    //        {
    //            currentHighlightable.HighlightOff();
    //            currentHighlightable = null;
    //        }
    //    }
    //}
    //private void HandleSelecting()
    //{
    //    if (Input.GetKeyUp(KeyCode.Mouse0))
    //    {
    //        if (CameraRay(selectableLayerMask, out RaycastHit selectableHit))
    //        {
    //            if (selectableHit.collider.CompareTag("Terrain"))
    //            {
    //                foreach (var unit in selectedUnits)
    //                {
    //                    unit.MoveTo(selectableHit.point);
    //                }
    //            }

    //            if (currentSelectable != null)
    //            {
    //                currentSelectable.Deselect();
    //                currentSelectable = null;
    //            }

    //            ISelectable selectable = selectableHit.collider.GetComponent<ISelectable>();
    //            if (selectable != null)
    //            {
    //                if (currentSelectable == selectable)
    //                {
    //                    currentSelectable.Deselect();
    //                    currentSelectable = null;
    //                }
    //                else if (selectable.CanSelect(team))
    //                {
    //                    currentSelectable = selectable;
    //                    currentSelectable.Select();

    //                    if (selectableHit.collider.TryGetComponent(out BattleUnit battleUnit))
    //                    {
    //                        selectedUnits.Clear();
    //                        selectedUnits.Add(battleUnit);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    private bool CameraRay(LayerMask layerMask, out RaycastHit hit)
    {
        return Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 100f, layerMask);
    }
}
