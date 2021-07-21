using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KK.NavGrid;
public class TerrainGenerator : MonoBehaviour
{
    public int width = 512;
    public int height = 512;
    public int depth = 20;

    public GameObject bridgeStonePrefab;
    public NavGrid navGrid;
    public GameObject[] decorations_Prefabs;
    public Vector2Int decorationPlacementRadius;


    private Terrain terrain;
    public void Generate()
    {
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);

        terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrainData(terrain.terrainData);
    }
    public void SnapDecorationsToGrid()
    {
        GameObject[] decorations = GameObject.FindGameObjectsWithTag("Decoration");
        foreach(var decoration in decorations)
        {
            decoration.transform.position = navGrid.GetCellCenterWorld(navGrid.WorldToCell(decoration.transform.position));
        }
    }
    public void PlaceDecorationsRandomly()
    {
        for(int x=0; x<=navGrid.gridSize.x; x+=decorationPlacementRadius.x)
        {
            for (int y=0; y <=navGrid.gridSize.y; y+=decorationPlacementRadius.y) {
                int attempts = 0;
                Vector2Int randomPos;
                do
                {
                    randomPos = new Vector2Int(x + UnityEngine.Random.Range(0, decorationPlacementRadius.x), y + UnityEngine.Random.Range(0, decorationPlacementRadius.y));
                    attempts++;
                } while (attempts < 10 && navGrid.IsNodeAnObstacle(randomPos));

                if (!navGrid.IsNodeAnObstacle(randomPos))
                {
                    GameObject decorationGO = Instantiate(decorations_Prefabs[UnityEngine.Random.Range(0, decorations_Prefabs.Length)], transform);
                    decorationGO.transform.position = navGrid.GetCellCenterWorld(randomPos);
                    decorationGO.tag = "Decoration";
                    decorationGO.isStatic = true;
                }
            }
        }
    }
    private TerrainData GenerateTerrainData(TerrainData data)
    {
        data.heightmapResolution = 2 * width + 1;
        data.size = new Vector3(width, depth, height);
        data.SetHeights(0, 0, GenerateHeights(data.heightmapResolution, data.heightmapResolution));

        return data;
    }

    private float[,] GenerateHeights(int width, int height)
    {
        float[,] heights = new float[width, height];

        int cornerCutting = 10;
        // Middle Segment        
        int middleSegmentWidth = 120;
        int middleSegmentHeight = 120;
        int middle_X = width / 2;// - middleSegmentWidth / 2;
        int middle_Y = height / 2;// - middleSegmentHeight / 2;

        Vector2Int[] middle_Points = new Vector2Int[]
        {
            new Vector2Int(middle_X-middleSegmentWidth/2+cornerCutting, middle_Y-middleSegmentHeight/2),
            new Vector2Int(middle_X-middleSegmentWidth/2, middle_Y-middleSegmentHeight/2+cornerCutting),
            new Vector2Int(middle_X-middleSegmentWidth/2, middle_Y+middleSegmentHeight/2-cornerCutting),
            new Vector2Int(middle_X-middleSegmentWidth/2+cornerCutting, middle_Y+middleSegmentHeight/2),
            new Vector2Int(middle_X+middleSegmentWidth/2-cornerCutting, middle_Y+middleSegmentHeight/2),
            new Vector2Int(middle_X+middleSegmentWidth/2, middle_Y+middleSegmentHeight/2-cornerCutting),
            new Vector2Int(middle_X+middleSegmentWidth/2, middle_Y-middleSegmentHeight/2+cornerCutting),
            new Vector2Int(middle_X+middleSegmentWidth/2-cornerCutting, middle_Y-middleSegmentHeight/2),
        };

        int segmentLength = 120;
        int segmentBreadth = 30;
        int leftBottom_X = width / 2 - middleSegmentWidth - segmentBreadth;
        int leftBottom_Y = height / 2 - middleSegmentHeight - segmentBreadth;

        Vector2Int[] leftBottom_Points = new Vector2Int[]
        {
        new Vector2Int(leftBottom_X,leftBottom_Y),
        new Vector2Int(leftBottom_X,leftBottom_Y+segmentLength-cornerCutting),
        new Vector2Int(leftBottom_X+cornerCutting,leftBottom_Y+segmentLength),
        new Vector2Int(leftBottom_X+segmentBreadth-cornerCutting,leftBottom_Y+segmentLength),
        new Vector2Int(leftBottom_X+segmentBreadth,leftBottom_Y+segmentLength-cornerCutting),
        new Vector2Int(leftBottom_X+segmentBreadth,leftBottom_Y+segmentLength*2/3),

        new Vector2Int(leftBottom_X+segmentLength*2/3,leftBottom_Y+segmentBreadth),
        new Vector2Int(leftBottom_X+segmentLength-cornerCutting,leftBottom_Y+segmentBreadth),
        new Vector2Int(leftBottom_X+segmentLength,leftBottom_Y+segmentBreadth-cornerCutting),
        new Vector2Int(leftBottom_X+segmentLength,leftBottom_Y+cornerCutting),
        new Vector2Int(leftBottom_X+segmentLength-cornerCutting,leftBottom_Y),
        };

        int rightBottom_X = width / 2 + middleSegmentWidth + segmentBreadth;
        int rightBottom_Y = height / 2 - middleSegmentHeight - segmentBreadth;

        Vector2Int[] rightBottom_Points = new Vector2Int[]
        {
        new Vector2Int(rightBottom_X,rightBottom_Y),
        new Vector2Int(rightBottom_X,rightBottom_Y+segmentLength-cornerCutting),
        new Vector2Int(rightBottom_X-cornerCutting,rightBottom_Y+segmentLength),
        new Vector2Int(rightBottom_X-segmentBreadth+cornerCutting,rightBottom_Y+segmentLength),
        new Vector2Int(rightBottom_X-segmentBreadth,rightBottom_Y+segmentLength-cornerCutting),
        new Vector2Int(rightBottom_X-segmentBreadth,rightBottom_Y+segmentLength*2/3),

        new Vector2Int(rightBottom_X-segmentLength*2/3,rightBottom_Y+segmentBreadth),
        new Vector2Int(rightBottom_X-segmentLength+cornerCutting,rightBottom_Y+segmentBreadth),
        new Vector2Int(rightBottom_X-segmentLength,rightBottom_Y+segmentBreadth-cornerCutting),
        new Vector2Int(rightBottom_X-segmentLength,rightBottom_Y+cornerCutting),
        new Vector2Int(rightBottom_X-segmentLength+cornerCutting,rightBottom_Y),
        };

        int rightUpper_X = width / 2 + middleSegmentWidth + segmentBreadth;
        int rightUpper_Y = height / 2 + middleSegmentHeight + segmentBreadth;

        Vector2Int[] rightUpper_Points = new Vector2Int[]
        {
        new Vector2Int(rightUpper_X,rightUpper_Y),
        new Vector2Int(rightUpper_X,rightUpper_Y-segmentLength+cornerCutting),
        new Vector2Int(rightUpper_X-cornerCutting,rightUpper_Y-segmentLength),
        new Vector2Int(rightUpper_X-segmentBreadth+cornerCutting,rightUpper_Y-segmentLength),
        new Vector2Int(rightUpper_X-segmentBreadth,rightUpper_Y-segmentLength+cornerCutting),
        new Vector2Int(rightUpper_X-segmentBreadth,rightUpper_Y-segmentLength*2/3),

        new Vector2Int(rightUpper_X-segmentLength*2/3,rightUpper_Y-segmentBreadth),
        new Vector2Int(rightUpper_X-segmentLength+cornerCutting,rightUpper_Y-segmentBreadth),
        new Vector2Int(rightUpper_X-segmentLength,rightUpper_Y-segmentBreadth+cornerCutting),
        new Vector2Int(rightUpper_X-segmentLength,rightUpper_Y-cornerCutting),
        new Vector2Int(rightUpper_X-segmentLength+cornerCutting,rightUpper_Y),
        };

        int leftUpper_X = width / 2 - middleSegmentWidth - segmentBreadth;
        int leftUpper_Y = height / 2 + middleSegmentHeight + segmentBreadth;

        Vector2Int[] leftUpper_Points = new Vector2Int[]
        {
        new Vector2Int(leftUpper_X,leftUpper_Y),
        new Vector2Int(leftUpper_X,leftUpper_Y-segmentLength+cornerCutting),
        new Vector2Int(leftUpper_X+cornerCutting,leftUpper_Y-segmentLength),
        new Vector2Int(leftUpper_X+segmentBreadth-cornerCutting,leftUpper_Y-segmentLength),
        new Vector2Int(leftUpper_X+segmentBreadth,leftUpper_Y-segmentLength+cornerCutting),
        new Vector2Int(leftUpper_X+segmentBreadth,leftUpper_Y-segmentLength*2/3),

        new Vector2Int(leftUpper_X+segmentLength*2/3,leftUpper_Y-segmentBreadth),
        new Vector2Int(leftUpper_X+segmentLength-cornerCutting,leftUpper_Y-segmentBreadth),
        new Vector2Int(leftUpper_X+segmentLength,leftUpper_Y-segmentBreadth+cornerCutting),
        new Vector2Int(leftUpper_X+segmentLength,leftUpper_Y-cornerCutting),
        new Vector2Int(leftUpper_X+segmentLength-cornerCutting,leftUpper_Y),
        };

        CreateIsland(middle_Points, ref heights);
        CreateIsland(leftBottom_Points, ref heights);
        CreateIsland(rightBottom_Points, ref heights);
        CreateIsland(rightUpper_Points, ref heights);
        CreateIsland(leftUpper_Points, ref heights);

        float unitDiameter = (float)this.width / width;
        int x1, x2, y1, y2, x, y;
        // LeftBottom - Middle bridge
        x1 = leftBottom_X + segmentLength / 2 - 6;
        x2 = middle_X - middleSegmentWidth / 2 + 6;
        y1 = leftBottom_Y + segmentLength / 2 - 6;
        y2 = middle_Y - middleSegmentHeight / 2 + 6;

        x = x1;
        y = y1;
        while (x < x2 && y < y2)
        {
            CreateBridge(x * unitDiameter, y * unitDiameter);
            x += 3;
            y += 3;
        }

        // RightBottom - Middle bridge
        x1 = rightBottom_X - segmentLength / 2 + 6;
        x2 = middle_X + middleSegmentWidth / 2 - 6;
        y1 = rightBottom_Y + segmentLength / 2 - 6;
        y2 = middle_Y - middleSegmentHeight / 2 + 6;

        x = x1 - 3;
        y = y1;
        while (x >= x2 && y < y2)
        {
            CreateBridge(x * unitDiameter, y * unitDiameter);
            x -= 3;
            y += 3;
        }

        // RightUpper - Middle bridge
        x1 = rightUpper_X - segmentLength / 2 + 6;
        x2 = middle_X + middleSegmentWidth / 2 - 6;
        y1 = rightUpper_Y - segmentLength / 2 + 6;
        y2 = middle_Y + middleSegmentHeight / 2 - 6;

        x = x1 - 3;
        y = y1 - 3;
        print(y1);
        print(y2);
        while (x >= x2 && y >= y2)
        {
            CreateBridge(x * unitDiameter, y * unitDiameter);
            x -= 3;
            y -= 3;
        }

        // LeftUpper - Middle bridge
        x1 = leftUpper_X + segmentLength / 2 - 6;
        x2 = middle_X - middleSegmentWidth / 2 + 6;
        y1 = leftUpper_Y - segmentLength / 2 + 6;
        y2 = middle_Y + middleSegmentHeight / 2 - 6;

        x = x1;
        y = y1 - 3;
        while (x < x2 && y >= y2)
        {
            CreateBridge(x * unitDiameter, y * unitDiameter);
            x += 3;
            y -= 3;
        }
        print((leftUpper_X - leftBottom_X) % 3);
        return heights;
    }
    private void CreateBridge(float x, float y)
    {
        GameObject bridgeStoneGO = Instantiate(bridgeStonePrefab, transform);
        bridgeStoneGO.transform.position = new Vector3(x + 1f, bridgeStoneGO.transform.position.y + 0.001f, y + 1f);

    }
   
    private void SetHeights(int x, int y, float value, ref float[,] heights)
    {
        heights[y, x] = value;
    }
    private float GetHeights(int x, int y, ref float[,] heights)
    {
        return heights[y, x];
    }
    private void CreateIsland(Vector2Int[] polygonPoints, ref float[,] heights)
    {
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (var p in polygonPoints)
        {
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.y > maxY) maxY = p.y;
        }

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (IsInsindePolygon(polygonPoints, new Vector2Int(x, y)))
                {
                    SetHeights(x, y, 1f, ref heights);
                }
                if (IsOnPolygon(polygonPoints, new Vector2Int(x, y), out Vector2Int outsideDir))
                {
                    int smoothing = 3;
                    if (outsideDir.x == 0 || outsideDir.y == 0) smoothing = 3;
                    else if (outsideDir.x > 0 && outsideDir.y > 0) smoothing = 2;
                    else smoothing = 3;

                    float smoothingStep = 1f / smoothing;
                    for (int i = 1; i <= smoothing; i++)
                    {
                        float value = UnityEngine.Random.Range(1f - (i - 1) * smoothingStep, 1f - i * smoothingStep);
                        if (GetHeights(x + outsideDir.x * i, y + outsideDir.y * i, ref heights) < value)
                        {
                            SetHeights(x + outsideDir.x * i, y + outsideDir.y * i, value, ref heights);
                        }
                    }
                }
            }
        }
    }
    private bool IsInsindePolygon(Vector2Int[] polygonPoints, Vector2Int p)
    {
        var j = polygonPoints.Length - 1;
        var inside = false;
        for (int i = 0; i < polygonPoints.Length; j = i++)
        {
            var pi = polygonPoints[i];
            var pj = polygonPoints[j];
            if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
                (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                inside = !inside;
        }
        return inside;
    }
    private bool IsOnPolygon(Vector2Int[] polygonPoints, Vector2Int p, out Vector2Int outsideDir)
    {
        bool isOnPolygon = false;
        outsideDir = Vector2Int.zero;
        if (IsInsindePolygon(polygonPoints, p))
        {
            if (!IsInsindePolygon(polygonPoints, p + Vector2Int.right)) { isOnPolygon = true; outsideDir = Vector2Int.right; }
            if (!IsInsindePolygon(polygonPoints, p + Vector2Int.left)) { isOnPolygon = true; outsideDir = Vector2Int.left; }
            if (!IsInsindePolygon(polygonPoints, p + Vector2Int.up)) { isOnPolygon = true; outsideDir = Vector2Int.up; }
            if (!IsInsindePolygon(polygonPoints, p + Vector2Int.down)) { isOnPolygon = true; outsideDir = Vector2Int.down; }
            if (!IsInsindePolygon(polygonPoints, p + Vector2Int.right + Vector2Int.up)) { isOnPolygon = true; outsideDir = Vector2Int.right + Vector2Int.up; }
            if (!IsInsindePolygon(polygonPoints, p + Vector2Int.right + Vector2Int.down)) { isOnPolygon = true; outsideDir = Vector2Int.right + Vector2Int.down; }
            if (!IsInsindePolygon(polygonPoints, p + Vector2Int.left + Vector2Int.up)) { isOnPolygon = true; outsideDir = Vector2Int.left + Vector2Int.up; }
            if (!IsInsindePolygon(polygonPoints, p + Vector2Int.left + Vector2Int.down)) { isOnPolygon = true; outsideDir = Vector2Int.left + Vector2Int.down; }
        }

        return isOnPolygon;

    }
}
//private void CreateCoastLine(Vector2Int startPoint, Vector2Int endPoint, Vector2Int towardsWater_Dir, ref float[,] heights)
//{
//    int x1 = startPoint.x + towardsWater_Dir.x;
//    int x2 = endPoint.x + towardsWater_Dir.x;
//    int y1 = startPoint.y + towardsWater_Dir.y;
//    int y2 = endPoint.y + towardsWater_Dir.y;
//    int coastLength = 3;

//    float heightLeft = 1f;
//    float heightLeft_Interval = 1f / coastLength;

//    int x = x1;
//    int y = y1;
//    for (int l = 0; l < coastLength; l++)
//    {
//        heightLeft = 1f - l * heightLeft_Interval;

//        if (x1 == x2)
//        {
//            if (y1 == y2)
//            {
//                SetHeights(x + l * towardsWater_Dir.x, y + l * towardsWater_Dir.y, UnityEngine.Random.Range(Mathf.Clamp01(heightLeft), Mathf.Clamp01(heightLeft - heightLeft_Interval)), ref heights);
//            }
//            else
//            {
//                for (y = y1; y < y2; y++)
//                {
//                    SetHeights(x + l * towardsWater_Dir.x, y + l * towardsWater_Dir.y, UnityEngine.Random.Range(Mathf.Clamp01(heightLeft), Mathf.Clamp01(heightLeft - heightLeft_Interval)), ref heights);
//                }
//            }
//        }
//        for (x = x1; x < x2; x++)
//        {
//            if (y1 == y2)
//            {
//                SetHeights(x + l * towardsWater_Dir.x, y + l * towardsWater_Dir.y, UnityEngine.Random.Range(Mathf.Clamp01(heightLeft), Mathf.Clamp01(heightLeft - heightLeft_Interval)), ref heights);
//            }
//            else
//            {
//                for (y = y1; y < y2; y++)
//                {
//                    SetHeights(x + l * towardsWater_Dir.x, y + l * towardsWater_Dir.y, UnityEngine.Random.Range(Mathf.Clamp01(heightLeft), Mathf.Clamp01(heightLeft - heightLeft_Interval)), ref heights);
//                }
//            }
//        }
//    }
//}
//private void CreateIsland(int xCoord, int yCoord, int width, int height, ref float[,] heights)
//{
//    int x1 = xCoord, x2 = x1 + width;
//    int y1 = yCoord, y2 = y1 + height;
//    //int smoothWidth = 3;
//    for (int x = x1; x <= x2; x++)
//    {
//        for (int y = y1; y <= y2; y++)
//        {
//            SetHeights(x, y, 1f, ref heights);
//        }
//    }
//}