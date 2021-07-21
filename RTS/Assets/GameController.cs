using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KK.NavGrid;
public class GameController : MonoBehaviour
{    
    public static NavGrid navGrid;    


    public PlayerControlledTeam team;
    private void Start()
    {
        FindObjectOfType<UnitManager>().Init();

        navGrid = GetComponent<NavGrid>();
        navGrid.Init();

        team.Init();
        //agents = FindObjectsOfType<NavGridAgent>();

        //foreach (var unit in units)
        //{
        //    unit.transform.position = navGrid.GetCellCenterWorld(navGrid.WorldToCell(unit.transform.position));
        //    unit.Init(new Team(Color.red), navGrid, unit.transform.position);            
        //}

    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Mouse0))
    //    {
    //        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f))
    //        {
    //           foreach(var u in units) u.MoveTo(hit.point);
    //        }
    //    }

    //    //foreach (var agent in agents) agent.UpdateAgent(Time.deltaTime);
    //}
}
