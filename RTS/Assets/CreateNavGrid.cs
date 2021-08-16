using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEditor;
using System;
using System.IO;
namespace KK.NavGrid
{

    [ExecuteInEditMode]
    [RequireComponent(typeof(Grid))]
    public class CreateNavGrid : MonoBehaviour
    {
        public class Node
        {
            public int x, y;
            public float elevation;
            public override string ToString()
            {
                return x + ":" + y;// + ":" + elevation;
            }
        }
        [SerializeField] private Grid grid;

        public Vector2Int gridSize;
        private Dictionary<Vector2Int, Node> allNodes;
        private List<Node> walkableNodes;
        public List<Node> obstacles;
        public bool manualObstaclesAdding = false;

        public Node WorldToCell(Vector3 worldPos)
        {
            Vector3Int cellPos = grid.WorldToCell(worldPos);
            return allNodes[new Vector2Int(cellPos.x, cellPos.z)];
        }
        public Vector3 GetCellCenterWorld(Node node)
        {
            var center = grid.GetCellCenterWorld(new Vector3Int(node.x, 0, node.y));
            return new Vector3(center.x, node.elevation, center.z);
        }
        public Vector3 CellToLocal(Vector2Int cell)
        {
            return grid.CellToLocal(new Vector3Int(cell.x, 0, cell.y));
        }

        public LayerMask terrainLM;
        public LayerMask obstacleLM;
        public void Bake()
        {
            allNodes = new Dictionary<Vector2Int, Node>();
            walkableNodes = new List<Node>();
            obstacles = new List<Node>();

            for (int i = 0; i < gridSize.y; i++)
            {
                Debug.Log("Baking: " + ((float)i / (float)gridSize.y) * 100f + "%");
                for (int j = 0; j < gridSize.x; j++)
                {
                    Node node = new Node() { x = j, y = i, elevation = 0f };
                    if (Physics.Raycast(GetCellCenterWorld(node) + Vector3.up * 10f, Vector3.down, out RaycastHit hitInfo, 100f, terrainLM))
                    {
                        node.elevation = hitInfo.point.y;
                        allNodes.Add(new Vector2Int(node.x, node.y), node);

                        if (hitInfo.point.y > 4.9f)
                        {
                            obstacles.Add(node);
                        }
                        else if (Physics.CheckSphere(GetCellCenterWorld(node) + Vector3.up * 1f, .3f, obstacleLM))
                        //else if(Physics.CheckCapsule(GetCellCenterWorld(node) + Vector3.up * 0.3f, GetCellCenterWorld(node) + Vector3.up * .5f, 0.2f, obstacleLM))
                        {
                            obstacles.Add(node);
                        }
                        else
                        {
                            walkableNodes.Add(node);
                        }
                    }
                    else
                    {
                        Debug.Log("Node not found");
                    }

                }
            }

            string content;
            content = "WalkableNodes:" + Environment.NewLine;
            foreach (var n in walkableNodes)
            {
                content += n.ToString() +Environment.NewLine;
            }
            content += "Obstacles:" + Environment.NewLine;
            foreach (var n in obstacles)
            {
                content += n.ToString() + Environment.NewLine;
            }
            
            File.WriteAllText("Default_4_players.txt", content);
            Debug.Log("Baking finished");
        }
        private void OnValidate()
        {
            if (manualObstaclesAdding != lastManualObstaclesAdding)
            {
                lastManualObstaclesAdding = manualObstaclesAdding;

                if (manualObstaclesAdding)
                    SceneView.duringSceneGui += OnSceneGUI;
                else
                    SceneView.duringSceneGui -= OnSceneGUI;
            }
        }
        private void OnSceneGUI(SceneView obj)
        {
            var e = Event.current;
            //Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(e.mousePosition);
            Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(new Vector3(e.mousePosition.x, Screen.height - e.mousePosition.y - 36, 0)); //Upside-down and offset a little because of menus

            if (e.isMouse && Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    Node node = WorldToCell(hitInfo.point);
                    if (!obstacles.Contains(node))
                    {
                        obstacles.Add(node);
                    }
                }

            }
        }

        public bool showGrid;
        bool lastManualObstaclesAdding = false;
        private void OnDrawGizmos()
        {
            if (showGrid)
            {
                //for (int i = 0; i < gridSize.y; i++)
                //{
                //    for (int j = 0; j < gridSize.x; j++)
                //    {
                //        Vector2Int node = new Vector2Int(j, i);

                //        Gizmos.color = Color.white;
                //        Gizmos.DrawWireCube(GetCellCenterWorld(allNodes[node]), grid.cellSize * .95f);
                //    }
                //}
                foreach (var n in walkableNodes)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireCube(GetCellCenterWorld(n), grid.cellSize * .95f);
                }

                foreach (var n in obstacles)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(GetCellCenterWorld(n), grid.cellSize * .95f);
                }
            }
        }
    }

}