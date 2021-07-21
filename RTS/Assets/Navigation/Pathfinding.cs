using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel;
using Unity.Jobs;
using Unity.Burst;

namespace KK.NavGrid
{
    public enum PathStatus
    {
        PathComplete, PathNotFound, PathPending
    }

    public class Path
    {
        public Vector2Int[] nodes { get; private set; }
        public Path(Vector2Int[] nodes)
        {
            this.nodes = nodes;
        }
    }
    public class PathRequest
    {
        public int2 startNode;
        public int2 endNode;
        public NavGridAgent agent;
        public int2 excludedNode;
        public PathRequest(int2 startNode, int2 endNode, NavGridAgent agent, int2 excludedNode)
        {
            this.startNode = startNode;
            this.endNode = endNode;
            this.agent = agent;
            this.excludedNode = excludedNode;
        }
        public PathRequest(int2 startNode, int2 endNode, NavGridAgent agent)
        {
            this.startNode = startNode;
            this.endNode = endNode;
            this.agent = agent;
            this.excludedNode = new int2(-1,-1);
        }
    }


    public class Pathfinding : MonoBehaviour
    {
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        private int2 gridSize;
        private int2[] obstacles;


        public void Init(Vector2Int gridSize, List<Vector2Int> obstacles)
        {
            this.gridSize = new int2(gridSize.x, gridSize.y);
            this.obstacles = new int2[obstacles.Count];
            for (int i = 0; i < obstacles.Count; i++)
            {
                this.obstacles[i] = new int2(obstacles[i].x, obstacles[i].y);
            }
        }

        public Path[] FindAllPathsParallel(List<PathRequest> pathRequests)
        {            
            NativeArray<JobHandle> jobHandleArray = new NativeArray<JobHandle>(pathRequests.Count, Allocator.TempJob);
            NativeList<int2>[] paths = new NativeList<int2>[pathRequests.Count];

            NativeArray<int2>[] obstaclesList = new NativeArray<int2>[pathRequests.Count];

            for (int i = 0; i < pathRequests.Count; i++)
            {
                paths[i] = new NativeList<int2>(Allocator.TempJob);

                if (pathRequests[i].excludedNode.x == -1 && pathRequests[i].excludedNode.y == -1)
                {
                    obstaclesList[i] = new NativeArray<int2>(obstacles.Length, Allocator.TempJob);
                    for (int j = 0; j < obstacles.Length; j++)
                    {
                        obstaclesList[i][j] = obstacles[j];
                    }
                }else
                {
                    obstaclesList[i] = new NativeArray<int2>(obstacles.Length+1, Allocator.TempJob);
                    obstaclesList[i][0] = pathRequests[i].excludedNode;
                    for (int j = 1; j < obstacles.Length+1; j++)
                    {
                        obstaclesList[i][j] = obstacles[j-1];
                    }
                }

                FindPathJob findPathJob = new FindPathJob(gridSize, obstaclesList[i], pathRequests[i].startNode, pathRequests[i].endNode, paths[i]);

                jobHandleArray[i] = findPathJob.Schedule();


            }

            JobHandle.CompleteAll(jobHandleArray);

            Path[] pathsArray = new Path[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                var path = paths[i];

                if (paths[i].Length == 0)
                {
                    pathsArray[i] = null;
                }
                else
                {
                    Vector2Int[] nodes = new Vector2Int[path.Length];
                    for (int j = 0; j < path.Length; j++)
                    {
                        // The path's node list is reversed
                        nodes[path.Length - 1 - j] = new Vector2Int(path[j].x, path[j].y);
                    }
                    pathsArray[i] = new Path(nodes);
                }
            }

            foreach (var p in paths) p.Dispose();
            foreach (var o in obstaclesList) o.Dispose();

            jobHandleArray.Dispose();            

            return pathsArray;
        }
        IEnumerator Test()
        {
            int2 startPosition = new int2(0, 0);
            int2 endPosition = new int2(199, 199);
            while (true)
            {

                float startTime = Time.realtimeSinceStartup;

                int findPathJobCount = 1;
                NativeArray<JobHandle> jobHandleArray = new NativeArray<JobHandle>(findPathJobCount, Allocator.TempJob);
                NativeList<int2>[] paths = new NativeList<int2>[findPathJobCount];

                NativeArray<int2>[] obstaclesList = new NativeArray<int2>[findPathJobCount];

                for (int i = 0; i < findPathJobCount; i++)
                {
                    paths[i] = new NativeList<int2>(Allocator.TempJob);
                    obstaclesList[i] = new NativeArray<int2>(obstacles.Length, Allocator.TempJob);
                    for (int j = 0; j < obstacles.Length; j++)
                    {
                        obstaclesList[i][j] = obstacles[j];//new int2(obstacles[j].x, obstacles[j].y);
                    }

                    FindPathJob findPathJob = new FindPathJob(gridSize, obstaclesList[i], startPosition, endPosition, paths[i]);
                    //FindPathJob findPathJob = new FindPathJob(startPosition, endPosition, paths[i]);

                    jobHandleArray[i] = findPathJob.Schedule();


                }

                JobHandle.CompleteAll(jobHandleArray);
                //foreach (var a in paths[0]) Debug.Log(a);            




                foreach (var p in paths) p.Dispose();
                foreach (var o in obstaclesList) o.Dispose();

                jobHandleArray.Dispose();

                Debug.Log("Time: " + ((Time.realtimeSinceStartup - startTime) * 1000f));

                yield return new WaitForSeconds(1f);
            }
        }



        [BurstCompile]
        private struct FindPathJob : IJob
        {
            public int2 gridSize;
            public int2 startPosition;
            public int2 endPosition;
            public NativeList<int2> path;

            public NativeArray<int2> obstacles;
            public FindPathJob(int2 gridSize, NativeArray<int2> obstacles, int2 startPosition, int2 endPosition, NativeList<int2> path)
            {
                this.gridSize = gridSize;
                this.startPosition = startPosition;
                this.endPosition = endPosition;
                this.path = path;

                this.obstacles = obstacles;
            }

            public void Execute()
            {
                NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);

                for (int x = 0; x < gridSize.x; x++)
                {
                    for (int y = 0; y < gridSize.y; y++)
                    {
                        PathNode pathNode = new PathNode();
                        pathNode.x = x;
                        pathNode.y = y;
                        pathNode.index = CalculateIndex(x, y, gridSize.x);

                        pathNode.gCost = int.MaxValue;
                        pathNode.hCost = CalculateDistanceCost(new int2(x, y), endPosition);
                        pathNode.CalculateFCost();

                        pathNode.isWalkable = true;
                        pathNode.cameFromNodeIndex = -1;

                        pathNodeArray[pathNode.index] = pathNode;
                    }
                }

                for (int i = 0; i < obstacles.Length; i++)
                {
                    PathNode walkablePathNode = pathNodeArray[CalculateIndex(obstacles[i].x, obstacles[i].y, gridSize.x)];
                    walkablePathNode.SetIsWalkable(false);
                    pathNodeArray[CalculateIndex(obstacles[i].x, obstacles[i].y, gridSize.x)] = walkablePathNode;
                }


                NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Unity.Collections.Allocator.Temp);

                neighbourOffsetArray[0] = new int2(-1, 0); // Left
                neighbourOffsetArray[1] = new int2(+1, 0); // Right
                neighbourOffsetArray[2] = new int2(0, +1); // Up
                neighbourOffsetArray[3] = new int2(0, -1); // Down
                neighbourOffsetArray[4] = new int2(-1, -1); // Left Down
                neighbourOffsetArray[5] = new int2(-1, +1); // Left Up
                neighbourOffsetArray[6] = new int2(+1, -1); // Right Down
                neighbourOffsetArray[7] = new int2(+1, +1); // Right Up

                int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);

                PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
                startNode.gCost = 0;
                startNode.CalculateFCost();
                pathNodeArray[startNode.index] = startNode;

                NativeList<int> openList = new NativeList<int>(Allocator.Temp);
                NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

                openList.Add(startNode.index);

                while (openList.Length > 0)
                {
                    int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
                    PathNode currentNode = pathNodeArray[currentNodeIndex];

                    if (currentNodeIndex == endNodeIndex)
                    {
                        // reached our destination
                        break;
                    }

                    for (int i = 0; i < openList.Length; i++)
                    {
                        if (openList[i] == currentNodeIndex)
                        {
                            openList.RemoveAtSwapBack(i);
                            break;
                        }
                    }

                    closedList.Add(currentNodeIndex);

                    for (int i = 0; i < neighbourOffsetArray.Length; i++)
                    {
                        int2 neighbourOffset = neighbourOffsetArray[i];
                        int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                        if (!IsPositionInsideGrid(neighbourPosition, gridSize))
                        {
                            // Neighbour not valid position
                            continue;
                        }

                        int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);

                        if (closedList.Contains(neighbourNodeIndex))
                        {
                            // Already searched this node
                            continue;
                        }

                        PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
                        if (!neighbourNode.isWalkable)
                        {
                            // Not walkable
                            continue;
                        }
                        int2 currentNodePosition = new int2(currentNode.x, currentNode.y);

                        int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                        if (tentativeGCost < neighbourNode.gCost)
                        {
                            neighbourNode.cameFromNodeIndex = currentNodeIndex;
                            neighbourNode.gCost = tentativeGCost;
                            neighbourNode.CalculateFCost();
                            pathNodeArray[neighbourNodeIndex] = neighbourNode;

                            if (!openList.Contains(neighbourNode.index))
                            {
                                openList.Add(neighbourNode.index);
                            }
                        }
                    }
                }

                PathNode endNode = pathNodeArray[endNodeIndex];
                if (endNode.cameFromNodeIndex == -1)
                {// Did not find a path
                    Debug.Log("Did not find a path");
                }
                else
                {// Found a path                          
                    CalculatePath(path, pathNodeArray, endNode);
                }

                pathNodeArray.Dispose();
                neighbourOffsetArray.Dispose();
                openList.Dispose();
                closedList.Dispose();
            }

            private void CalculatePath(NativeList<int2> path, NativeArray<PathNode> pathNodeArray, PathNode endNode)
            {
                path.Add(new int2(endNode.x, endNode.y));

                PathNode currentNode = endNode;
                while (currentNode.cameFromNodeIndex != -1)
                {
                    PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
                    path.Add(new int2(cameFromNode.x, cameFromNode.y));
                    currentNode = cameFromNode;
                }
            }
            private bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
            {
                return
                    gridPosition.x >= 0 &&
                    gridPosition.y >= 0 &&
                    gridPosition.x < gridSize.x &&
                    gridPosition.y < gridSize.y;
            }
            private int CalculateDistanceCost(int2 aPosition, int2 bPosition)
            {
                int xDistance = math.abs(aPosition.x - bPosition.x);
                int yDistance = math.abs(aPosition.y - bPosition.y);
                int remaining = math.abs(xDistance - yDistance);
                return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
            }
            private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
            {
                PathNode lowestCostPathNode = pathNodeArray[openList[0]];
                for (int i = 1; i < openList.Length; i++)
                {
                    PathNode testPathNode = pathNodeArray[openList[i]];
                    if (testPathNode.fCost < lowestCostPathNode.fCost)
                    {
                        lowestCostPathNode = testPathNode;
                    }
                }
                return lowestCostPathNode.index;
            }
            private int CalculateIndex(int x, int y, int gridWidth)
            {
                return x + y * gridWidth;
            }
        }
        private struct PathNode
        {
            public int x;
            public int y;

            public int index;

            public int gCost;
            public int hCost;
            public int fCost;

            public bool isWalkable;

            public int cameFromNodeIndex;

            public void CalculateFCost()
            {
                fCost = gCost + hCost;
            }
            public void SetIsWalkable(bool isWalkable)
            {
                this.isWalkable = isWalkable;
            }
        }
    }

}