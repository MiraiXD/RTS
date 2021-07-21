

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace KK.NavGrid
{
    public class NavGridAgent : MonoBehaviour
    {
        public PathStatus pathStatus { get; private set; }
        public Path path;
        public NavGrid navGrid { get; private set; }

        public Vector2Int currentNode { get; private set; }
        //public Vector2Int prevNode { get; private set; }
        public Vector2Int nextNode { get; private set; }
        public Vector2Int destinationNode { get; private set; }
        //private Func<float, Vector3> PathFunction;
        private int currentPathNodeIndex = 0;

        //private int currentSteeringTargetIndex;
        //public List<Vector3> steeringTargets { get; set; }
        public Vector3 currentSteeringTarget { get; set; }
        //public int steeringTargets_Number = 10;
        public Vector3 nextPosition { get; private set; }
        public float speed { get; set; } = 5f;
        public int stoppingDistance { get; set; } = 0;

        public Action onDestinationReached;
        public void Init(NavGrid navGrid, Vector2Int startingNode)
        {
            this.navGrid = navGrid;
            currentNode = startingNode;
            nextNode = Vector2Int.left;

            navGrid.SetNodeOccupied(currentNode);

            //steeringTargets = new List<Vector3>();
        }

        private bool tryAgainFindingPath = false;
        private int tryAgain_FrameCounter;
        public void SetPath(Path path)
        {
            this.path = path;
            if (path == null)
            {                
                pathStatus = PathStatus.PathNotFound;
                tryAgainFindingPath = true;
                tryAgain_FrameCounter = 5;                
            }
            else
            {
                pathStatus = PathStatus.PathComplete;
                currentSteeringTarget = transform.position;
                currentPathNodeIndex = -1;

            }
        }

        public void SetDestination(Vector2Int destinationNode)
        {
            this.destinationNode = destinationNode;
            
            if(path!=null)
                navGrid.SetNodeUnoccupied(nextNode);

            path = null;

            navGrid.FindPath(currentNode, destinationNode, this);
        }


        public bool UpdateAgent(float deltaTime)
        {           
            if (path == null)
            {                
                if (tryAgainFindingPath)
                {                    
                    if (--tryAgain_FrameCounter <= 0)
                    {                        
                        SetDestination(destinationNode);
                        tryAgainFindingPath = false;
                    }
                }

                nextPosition = transform.position;                
                return true;
            }
            else
            {
                float movementDelta = deltaTime * speed;
                nextPosition = Vector3.MoveTowards(transform.position, currentSteeringTarget, movementDelta);
                if (nextPosition == currentSteeringTarget)
                {
                    movementDelta -= (transform.position - currentSteeringTarget).magnitude;


                    currentPathNodeIndex++;
                    if (currentPathNodeIndex < path.nodes.Length - 1 - stoppingDistance)
                    {
                        //prevNode = path.nodes[Mathf.Clamp(currentPathNodeIndex - 1, 0, path.nodes.Length - 1)];
                        currentNode = path.nodes[currentPathNodeIndex];
                        nextNode = path.nodes[Mathf.Clamp(currentPathNodeIndex + 1, 0, path.nodes.Length - 1)];
                    }
                    else
                    {
                        path = null;
                        onDestinationReached?.Invoke();
                        nextPosition = transform.position;

                        //prevNode = currentNode;
                        currentNode = nextNode;
                        return true;
                    }

                    if (navGrid.IsNodeOccupied(nextNode))
                    {
                        if (nextNode == destinationNode)
                        {
                            path = null;
                            nextPosition = transform.position;
                            return true;
                        }
                        else
                        {
                            path = null;
                            navGrid.FindPath(currentNode, destinationNode, this, nextNode);
                            nextNode = Vector2Int.left;
                            nextPosition = transform.position;
                            return true;
                        }
                    }
                    else
                    {
                        navGrid.SetNodeOccupied(nextNode);
                        navGrid.SetNodeUnoccupied(currentNode);

                        currentSteeringTarget = navGrid.GetCellCenterWorld(nextNode);

                        nextPosition = Vector3.MoveTowards(nextPosition, currentSteeringTarget, movementDelta);
                    }
                }

                return true;
            }
        }
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//namespace KK.NavGrid
//{
//    public class NavGridAgent : MonoBehaviour
//    {
//        public PathStatus pathStatus { get; private set; }
//        public Path path;
//        public NavGrid navGrid { get; private set; }

//        public Vector2Int currentNode { get; private set; }
//        public Vector2Int prevNode { get; private set; }
//        public Vector2Int nextNode { get; private set; }
//        public Vector2Int destinationNode { get; private set; }
//        private Func<float, Vector3> PathFunction;
//        private int currentPathNodeIndex = 0;

//        private int currentSteeringTargetIndex;
//        public List<Vector3> steeringTargets { get; set; }
//        public Vector3 currentSteeringTarget { get; set; }
//        public int steeringTargets_Number = 10;
//        public Vector3 nextPosition { get; private set; }
//        public float speed { get; set; } = 5f;
//        public int stoppingDistance { get; set; } = 0;

//        public Action onDestinationReached;
//        public void Init(NavGrid navGrid, Vector2Int startingNode)
//        {
//            this.navGrid = navGrid;
//            currentNode = startingNode;
//            prevNode = currentNode;

//            navGrid.SetNodeOccupied(currentNode);

//            steeringTargets = new List<Vector3>();
//        }

//        public void SetPath(Path path)
//        {
//            this.path = path;
//            if (path == null) pathStatus = PathStatus.PathInvalid;
//            else
//            {
//                pathStatus = PathStatus.PathComplete;

//                currentPathNodeIndex = -1;
//                currentSteeringTargetIndex = -1;
//                currentSteeringTarget = transform.position;
//                steeringTargets.Clear();
//                //prevNode = path.nodes[currentPathNodeIndex];
//                //currentNode = path.nodes[currentPathNodeIndex];
//                //nextNode = path.nodes[currentPathNodeIndex + 1];

//                //if (!navGrid.IsNodeOccupied(nextNode))
//                //{
//                //    navGrid.SetNodeOccupied(nextNode);
//                //    navGrid.SetNodeUnoccupied(currentNode);
//                //    CreateSteeringTargets(prevNode, currentNode, nextNode, steeringTargets_Number);

//                //    currentSteeringTargetIndex = 0;
//                //    currentSteeringTarget = steeringTargets[currentSteeringTargetIndex];
//                //}
//                //else
//                //{
//                //    path = null;
//                //    navGrid.FindPath(currentNode, destinationNode, this, nextNode);
//                //}

//                //currentPathNodeIndex = 0;
//                //prevNode = path.nodes[currentPathNodeIndex];
//                //currentNode = path.nodes[currentPathNodeIndex];
//                //nextNode = path.nodes[currentPathNodeIndex + 1];

//                //if (!navGrid.IsNodeOccupied(nextNode))
//                //{
//                //    navGrid.SetNodeOccupied(nextNode);
//                //    navGrid.SetNodeUnoccupied(currentNode);
//                //    CreateSteeringTargets(prevNode, currentNode, nextNode, steeringTargets_Number);

//                //    currentSteeringTargetIndex = 0;
//                //    currentSteeringTarget = steeringTargets[currentSteeringTargetIndex];
//                //}
//                //else
//                //{
//                //    path = null;
//                //    navGrid.FindPath(currentNode, destinationNode, this, nextNode);
//                //}
//            }
//        }

//        public void SetDestination(Vector2Int destinationNode)
//        {
//            this.destinationNode = destinationNode;
//            navGrid.FindPath(currentNode, destinationNode, this);
//        }


//        public bool UpdateAgent(float deltaTime)
//        {
//            foreach (var target in steeringTargets)
//                Debug.DrawLine(target + Vector3.down, target + Vector3.up * 3f);

//            if (path == null)
//            {
//                return false;
//            }
//            else
//            {
//                float movementDelta = deltaTime * speed;
//                nextPosition = Vector3.MoveTowards(transform.position, currentSteeringTarget, movementDelta);
//                if(nextPosition == currentSteeringTarget)
//                {
//                    movementDelta -= (transform.position - currentSteeringTarget).magnitude;

//                    if (currentSteeringTargetIndex < steeringTargets.Count - 1)
//                    {
//                        currentSteeringTargetIndex++;                        
//                    }
//                    else
//                    {
//                        currentPathNodeIndex++;
//                        if (currentPathNodeIndex < path.nodes.Length - 1 - stoppingDistance)
//                        {
//                            print("huj2");
//                            //prevNode = currentNode;
//                            prevNode = path.nodes[Mathf.Clamp(currentPathNodeIndex - 1, 0, path.nodes.Length-1)];
//                            currentNode = path.nodes[currentPathNodeIndex];
//                            nextNode = path.nodes[currentPathNodeIndex + 1];
//                        }
//                        else if (currentPathNodeIndex == path.nodes.Length - 1 - stoppingDistance)
//                        {
//                            print("huj");
//                            prevNode = currentNode;
//                            currentNode = path.nodes[currentPathNodeIndex];
//                            nextNode = path.nodes[currentPathNodeIndex];
//                        }
//                        else
//                        {
//                            path = null;
//                            onDestinationReached?.Invoke();
//                            return true;
//                        }


//                        if (!navGrid.IsNodeOccupied(nextNode))
//                        {
//                            print("UNOCCUPIED");
//                            navGrid.SetNodeOccupied(nextNode);
//                            navGrid.SetNodeUnoccupied(currentNode);
//                            CreateSteeringTargets(prevNode, currentNode, nextNode, steeringTargets_Number);

//                            currentSteeringTargetIndex = 0;
//                            currentSteeringTarget = steeringTargets[currentSteeringTargetIndex];
//                        }
//                        else
//                        {
//                            print("OCCUPIED");
//                            path = null;
//                            navGrid.FindPath(currentNode, destinationNode, this, nextNode);
//                            return false;
//                        }

//                    }
//                    currentSteeringTarget = steeringTargets[currentSteeringTargetIndex];

//                    nextPosition = Vector3.MoveTowards(nextPosition, currentSteeringTarget, movementDelta);
//                }

//                return true;              
//            }
//        }
//        private void CreateSteeringTargets(Vector2Int prevNode, Vector2Int currentNode, Vector2Int nextNode, int howManyTargets)
//        {
//            Vector3 leftPoint = navGrid.GetCellCenterWorld(currentNode) - (navGrid.CellToLocal(currentNode) - navGrid.CellToLocal(prevNode)) / 2f;
//            Vector3 leftMiddlePoint = navGrid.GetCellCenterWorld(currentNode);
//            Vector3 rightMiddlePoint = navGrid.GetCellCenterWorld(currentNode);
//            Vector3 rightPoint = navGrid.GetCellCenterWorld(currentNode) + (navGrid.CellToLocal(nextNode) - navGrid.CellToLocal(currentNode)) / 2f;

//            float resolution = 1f / howManyTargets;
//            steeringTargets.Clear();

//            for (int i = 1; i <= howManyTargets; i++)
//            {
//                steeringTargets.Add(DeCasteljausAlgorithm(leftPoint, leftMiddlePoint, rightMiddlePoint, rightPoint, i * resolution));
//            }
//        }
//        private Vector3 DeCasteljausAlgorithm(Vector3 startPoint, Vector3 controlPoint1, Vector3 controlPoint2, Vector3 endPoint, float t)
//        {
//            float oneMinusT = 1f - t;

//            Vector3 Q = oneMinusT * startPoint + t * controlPoint1;
//            Vector3 R = oneMinusT * controlPoint1 + t * controlPoint2;
//            Vector3 S = oneMinusT * controlPoint2 + t * endPoint;

//            Vector3 P = oneMinusT * Q + t * R;
//            Vector3 T = oneMinusT * R + t * S;

//            Vector3 U = oneMinusT * P + t * T;

//            return U;
//        }
//    }
//}

