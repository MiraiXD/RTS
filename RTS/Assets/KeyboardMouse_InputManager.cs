using UnityEngine;
using KK.CommandPattern;
using UnityEngine.EventSystems;

/// <summary>
/// InputManager for a PC when playing with a mouse and keyboard. Here you can bind or rebind commands and keys. 
/// </summary>
public sealed class KeyboardMouse_InputManager : MonoBehaviour
{
    public ICommand LMB, RMB, alpha1, alpha2, mouseMovement;
    public LayerMask interactableLayerMask;
    private Collider currentColliderHit;
    private IHighlightable currentHighlightable;
    private ISelectable currentSelectable;
    [SerializeField] private Camera camera;

    private LocalGameManager gameManager;
    private UnitManager unitManager;
    public void Init(LocalGameManager gameManager, UnitManager unitManager)
    {
        this.gameManager = gameManager;
        this.unitManager = unitManager;
        //mouseMovement = new PlayerRotateCommand(playerController, this);
        //LMB = new PlayerShootCommand(playerController);
        //alpha1 = new TriggerAbilityCommand(playerController, 0);
        //alpha2 = new TriggerAbilityCommand(playerController, 1);        
    }

    //public void UpdateInput()
    //{
    //    if (!initialized) return;

    //    mouseMovement.Execute();

    //    if (Input.GetKeyDown(KeyCode.Mouse0)) LMB.Execute();
    //    if (Input.GetKeyDown(KeyCode.Alpha1)) alpha1.Execute();
    //    if (Input.GetKeyDown(KeyCode.Alpha2)) alpha2.Execute();

    //    //
    //    // so on for the rest of the keys... 
    //    // 
    //}

    public void UpdateInput()
    {        
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
        if (Input.GetKeyDown(KeyCode.Space)) gameManager.SendSpawnUnit(Entities.BattleUnitModel.UnitType.Knight);
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (unitManager.selectedUnitsCount == unitManager.unitsCount) unitManager.DeselectAllUnits();
            else unitManager.SelectAllUnits();
        }
         if (Input.GetKeyDown(KeyCode.S))
        {
            unitManager.MoveSelectedUnitsRandomly();
        }

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
                    var grid = FindObjectOfType<Grid>();
                    if (unitManager.selectedUnitsCount > 0)
                    {
                        //int nodeXCoord = (int)((hit.point.x - grid.transform.position.x) / grid.cellSize.x);
                        //int nodeYCoord = (int)((hit.point.z - grid.transform.position.z) / grid.cellSize.z);
                        unitManager.MoveSelectedUnits(hit.point);
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

    private bool CameraRay(LayerMask layerMask, out RaycastHit hit)
    {
        return Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 100f, layerMask);
    }
}

