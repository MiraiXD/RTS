using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace KK.NavGrid
{

    [RequireComponent(typeof(Grid))]
    [RequireComponent(typeof(Pathfinding))]
    public class NavGrid : MonoBehaviour
    {
        [SerializeField] private Grid grid;
        [SerializeField] private Pathfinding pathfinding;

        private List<PathRequest> pathRequests;

        public Vector2Int gridSize;
        public List<Vector2Int> obstacles;
        public List<Vector2Int> occupiedNodes;
        //public List<Vector2Int> reservedNodes;

        public void Init()
        {
            occupiedNodes = new List<Vector2Int>();
            //reservedNodes = new List<Vector2Int>();

            pathfinding.Init(gridSize, obstacles);

            pathRequests = new List<PathRequest>();
        }
        //public bool IsNodeAvailable(Vector2Int node)
        //{
        //    return !occupiedNodes.Contains(node) && !reservedNodes.Contains(node);
        //}
        //public bool ReserveNode(Vector2Int node)
        //{
        //    if (reservedNodes.Contains(node))
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        reservedNodes.Add(node);
        //        return true;
        //    }
        //}
        //public bool UnreserveNode(Vector2Int node)
        //{
        //    if (reservedNodes.Contains(node))
        //    {
        //        reservedNodes.Remove(node);
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //public bool OccupyNode(Vector2Int node)
        //{
        //    if (occupiedNodes.Contains(node))
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        occupiedNodes.Add(node);
        //        return true;
        //    }
        //}
        //public bool UnoccupyNode(Vector2Int node)
        //{
        //    if (occupiedNodes.Contains(node))
        //    {
        //        occupiedNodes.Remove(node);
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        public bool IsNodeOccupied(Vector2Int node)
        {
            return occupiedNodes.Contains(node);
        }
        public void SetNodeOccupied(Vector2Int node)
        {
            occupiedNodes.Add(node);
        }
        public void SetNodeUnoccupied(Vector2Int node)
        {
            if (occupiedNodes.Contains(node)) occupiedNodes.Remove(node);
        }

        public Vector2Int WorldToCell(Vector3 worldPos)
        {
            //Vector3Int cellPos = grid.WorldToCell(worldPos /*- leftBottomTilePos.position*/);// + grid.cellSize / 2f);
            Vector3Int cellPos = grid.WorldToCell(worldPos);
            return new Vector2Int(cellPos.x, cellPos.z);
        }
        public Vector3 GetCellCenterWorld(Vector2Int cell)
        {
            //return /*leftBottomTilePos.position + */grid.GetCellCenterWorld(new Vector3Int(cell.x, 0, cell.y));// - grid.cellSize / 2f;
            return grid.GetCellCenterWorld(new Vector3Int(cell.x, 0, cell.y));
        }
        public Vector3 CellToLocal(Vector2Int cell)
        {
            return grid.CellToLocal(new Vector3Int(cell.x, 0, cell.y));
        }




        public void FindPath(Vector2Int startPosition, Vector2Int endPosition, NavGridAgent agent, Vector2Int excludedNode)
        {
            foreach (var obstacle in obstacles)
            {
                if (obstacle == endPosition)
                {
                    Debug.Log("End position is an obstacle. Returning");
                    return;
                }
            }

            PathRequest pathRequest = new PathRequest(new int2(startPosition.x, startPosition.y), new int2(endPosition.x, endPosition.y), agent, new int2(excludedNode.x, excludedNode.y));
            pathRequests.Add(pathRequest);
        }
        public void FindPath(Vector2Int startPosition, Vector2Int endPosition, NavGridAgent agent)
        {
            foreach (var obstacle in obstacles)
            {
                if (obstacle == endPosition)
                {
                    Debug.Log("End position is an obstacle. Returning");
                    return;
                }
            }

            PathRequest pathRequest = new PathRequest(new int2(startPosition.x, startPosition.y), new int2(endPosition.x, endPosition.y), agent);
            pathRequests.Add(pathRequest);
        }
        private void LateUpdate()
        {
            if (pathRequests.Count != 0)
            {
                float startTime = Time.realtimeSinceStartup;
                Path[] paths = pathfinding.FindAllPathsParallel(pathRequests);
                for (int i = 0; i < pathRequests.Count; i++)
                {
                    if (pathRequests[i].agent != null)
                        pathRequests[i].agent.SetPath(paths[i]);
                }

                //Debug.Log(pathRequests.Count + " requests done in: " + ((Time.realtimeSinceStartup - startTime) * 1000f) + "ms");
                pathRequests.Clear();
            }
        }


        public void Bake()
        {
            obstacles = new List<Vector2Int>();
            for (int i = 0; i < gridSize.y; i++)
            {
                for (int j = 0; j < gridSize.x; j++)
                {
                    Vector2Int node = new Vector2Int(j, i);
                    if (Physics.Raycast(GetCellCenterWorld(node) + Vector3.up * 3f, Vector3.down, out RaycastHit hitInfo, 10f))
                    {
                        if (Mathf.Abs(hitInfo.point.y - 2f) > 0.1f)
                        {
                            obstacles.Add(node);
                        }

                    }

                }
            }
        }
        public bool showGrid;
        public bool showBounds = false;
        public MeshFilter[] filters;
        private void OnDrawGizmos()
        {
            if (showGrid)
            {
                for (int i = 0; i < gridSize.y; i++)
                {
                    for (int j = 0; j < gridSize.x; j++)
                    {
                        Vector2Int node = new Vector2Int(j, i);

                        //bool isWalkable = true;
                        //foreach (var n in obstacles)
                        //{
                        //    if (n == node) isWalkable = false;
                        //}

                        Gizmos.color = Color.white;
                        Gizmos.DrawWireCube(GetCellCenterWorld(node), grid.cellSize * .95f);
                    }
                }

                foreach (var n in obstacles)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawWireCube(GetCellCenterWorld(n), grid.cellSize * .95f);
                }

                foreach (var n in occupiedNodes)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(GetCellCenterWorld(n), grid.cellSize * .95f);
                }
            }
            //if (showBounds)
            //{
            //    if (filters == null) filters = FindObjectsOfType<MeshFilter>();

            //    foreach (var f in filters)
            //    {
            //        var vertices = f.sharedMesh.vertices;

            //        if (vertices.Length <= 0) return;

            //        // TransformPoint converts the local mesh vertice dependent on the transform
            //        // position, scale and orientation into a global position
            //        var min = f.transform.TransformPoint(vertices[0]);
            //        var max = min;

            //        // Iterate through all vertices
            //        // except first one
            //        for (var i = 1; i < vertices.Length; i++)
            //        {
            //            var V = f.transform.TransformPoint(vertices[i]);

            //            // Go through X,Y and Z of the Vector3
            //            for (var n = 0; n < 3; n++)
            //            {
            //                max[n] = Mathf.Max(V[n], max[n]);
            //                min[n] = Mathf.Min(V[n], min[n]);
            //            }
            //        }

            //        var bounds = new Bounds();
            //        bounds.SetMinMax(min, max);

            //        // ust to compare it to the original bounds
            //        //Gizmos.DrawWireCube(mesh_renderer.bounds.center, mesh_renderer.bounds.size);
            //        //Gizmos.DrawWireSphere(mesh_renderer.bounds.center, 0.3f);

            //        Gizmos.color = Color.green;
            //        Gizmos.DrawWireCube(bounds.center, bounds.size);
            //    }
            //}
        }
    }

}