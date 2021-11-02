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


    private Terrain terrain;

    public void Generate()
    {
        terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrainData(terrain.terrainData);
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

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                SetHeights(i, j, 5f, ref heights);
            }
        }

        int w = 195 * 2;
        int h = 195 * 2;
        Vector2Int leftBottomCorner = new Vector2Int(width / 2 - 1 - w / 2, height / 2 - 1 - h / 2);
        Vector2Int rightBottomCorner = new Vector2Int(width / 2 + w / 2, height / 2 - 1 - h / 2);
        Vector2Int rightUpperCorner = new Vector2Int(width / 2 + w / 2, height / 2 + h / 2);
        Vector2Int leftUpperCorner = new Vector2Int(width / 2 - 1 - w / 2, height / 2 + h / 2);
        Vector2Int[] polygon = new Vector2Int[]
        {
            leftBottomCorner,rightBottomCorner,rightUpperCorner,leftUpperCorner
        };
        SetHeights(polygon, 3f, ref heights);

        int platformWidth = 30 * 2;
        int platformHeight = 30 * 2;
        int platformShortSide = 15 * 2;
        Vector2Int[] leftBottomPlatformPolygon = new Vector2Int[]
        {
            leftBottomCorner,
            leftBottomCorner + new Vector2Int(0,platformHeight),
            leftBottomCorner + new Vector2Int(platformShortSide,platformHeight),
            leftBottomCorner + new Vector2Int(platformWidth,platformShortSide),
            leftBottomCorner + new Vector2Int(platformWidth,0),
        };
        SetHeights(leftBottomPlatformPolygon, 3f, ref heights);

        Vector2Int[] rightBottomPlatformPolygon = new Vector2Int[]
        {
            rightBottomCorner,
            rightBottomCorner + new Vector2Int(0,platformHeight),
            rightBottomCorner + new Vector2Int(-platformShortSide,platformHeight),
            rightBottomCorner + new Vector2Int(-platformWidth,platformShortSide),
            rightBottomCorner + new Vector2Int(-platformWidth,0),
        };
        SetHeights(rightBottomPlatformPolygon, 3f, ref heights);

        Vector2Int[] rightUpperPlatformPolygon = new Vector2Int[]
        {
            rightUpperCorner,
            rightUpperCorner + new Vector2Int(0,-platformHeight),
            rightUpperCorner + new Vector2Int(-platformShortSide,-platformHeight),
            rightUpperCorner + new Vector2Int(-platformWidth,-platformShortSide),
            rightUpperCorner + new Vector2Int(-platformWidth,0),
        };
        SetHeights(rightUpperPlatformPolygon, 3f, ref heights);

        Vector2Int[] leftUpperPlatformPolygon = new Vector2Int[]
        {
            leftUpperCorner,
            leftUpperCorner + new Vector2Int(0,-platformHeight),
            leftUpperCorner + new Vector2Int(platformShortSide,-platformHeight),
            leftUpperCorner + new Vector2Int(platformWidth,-platformShortSide),
            leftUpperCorner + new Vector2Int(platformWidth,0),
        };
        SetHeights(leftUpperPlatformPolygon, 3f, ref heights);
        return heights;
    }

    private void SetHeights(int x, int y, float value, ref float[,] heights)
    {
        heights[y, x] = value / depth;
    }
    private float GetHeights(int x, int y, ref float[,] heights)
    {
        return heights[y, x] * depth;
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
    private void SetHeights(Vector2Int[] polygonPoints, float value, ref float[,] heights)
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
                    SetHeights(x, y, value, ref heights);
                }
                //if (IsOnPolygon(polygonPoints, new Vector2Int(x, y), out Vector2Int outsideDir))
                //{
                //    int smoothing = 3;
                //    if (outsideDir.x == 0 || outsideDir.y == 0) smoothing = 3;
                //    else if (outsideDir.x > 0 && outsideDir.y > 0) smoothing = 2;
                //    else smoothing = 3;

                //    float smoothingStep = 1f / smoothing;
                //    for (int i = 1; i <= smoothing; i++)
                //    {
                //        float value = UnityEngine.Random.Range(1f - (i - 1) * smoothingStep, 1f - i * smoothingStep);
                //        if (GetHeights(x + outsideDir.x * i, y + outsideDir.y * i, ref heights) < value)
                //        {
                //            SetHeights(x + outsideDir.x * i, y + outsideDir.y * i, value, ref heights);
                //        }
                //    }
                //}
            }
        }
    }
}
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using KK.NavGrid;
//public class TerrainGenerator : MonoBehaviour
//{
//    public int width = 512;
//    public int height = 512;
//    public int depth = 20;

//    public GameObject bridgeStonePrefab;
//    public NavGrid navGrid;
//    public GameObject[] decorations_Prefabs;
//    public Vector2Int decorationPlacementRadius;


//    private Terrain terrain;
//    public void Generate()
//    {
//        int childCount = transform.childCount;
//        for (int i = childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);

//        terrain = GetComponent<Terrain>();
//        terrain.terrainData = GenerateTerrainData(terrain.terrainData);        
//    }
//    public void SnapDecorationsToGrid()
//    {
//        GameObject[] decorations = GameObject.FindGameObjectsWithTag("Decoration");
//        foreach(var decoration in decorations)
//        {
//            decoration.transform.position = navGrid.GetCellCenterWorld(navGrid.WorldToCell(decoration.transform.position));
//        }
//    }
//    public void PlaceDecorationsRandomly()
//    {
//        for(int x=0; x<=navGrid.gridSize.x; x+=decorationPlacementRadius.x)
//        {
//            for (int y=0; y <=navGrid.gridSize.y; y+=decorationPlacementRadius.y) {
//                int attempts = 0;
//                Vector2Int randomPos;
//                do
//                {
//                    randomPos = new Vector2Int(x + UnityEngine.Random.Range(0, decorationPlacementRadius.x), y + UnityEngine.Random.Range(0, decorationPlacementRadius.y));
//                    attempts++;
//                } while (attempts < 10 && navGrid.IsNodeAnObstacle(randomPos));

//                if (!navGrid.IsNodeAnObstacle(randomPos))
//                {
//                    GameObject decorationGO = Instantiate(decorations_Prefabs[UnityEngine.Random.Range(0, decorations_Prefabs.Length)], transform);
//                    decorationGO.transform.position = navGrid.GetCellCenterWorld(randomPos);
//                    decorationGO.tag = "Decoration";
//                    decorationGO.isStatic = true;
//                }
//            }
//        }
//    }
//    private TerrainData GenerateTerrainData(TerrainData data)
//    {
//        data.heightmapResolution = 2 * width + 1;
//        data.size = new Vector3(width, depth, height);
//        data.SetHeights(0, 0, GenerateHeights(data.heightmapResolution, data.heightmapResolution));

//        return data;
//    }

//    private float[,] GenerateHeights(int width, int height)
//    {
//        float[,] heights = new float[width, height];

//        int cornerCutting = 10;
//        // Middle Segment        
//        int middleSegmentWidth = 120;
//        int middleSegmentHeight = 120;
//        int middle_X = width / 2;// - middleSegmentWidth / 2;
//        int middle_Y = height / 2;// - middleSegmentHeight / 2;

//        Vector2Int[] middle_Points = new Vector2Int[]
//        {
//            new Vector2Int(middle_X-middleSegmentWidth/2+cornerCutting, middle_Y-middleSegmentHeight/2),
//            new Vector2Int(middle_X-middleSegmentWidth/2, middle_Y-middleSegmentHeight/2+cornerCutting),
//            new Vector2Int(middle_X-middleSegmentWidth/2, middle_Y+middleSegmentHeight/2-cornerCutting),
//            new Vector2Int(middle_X-middleSegmentWidth/2+cornerCutting, middle_Y+middleSegmentHeight/2),
//            new Vector2Int(middle_X+middleSegmentWidth/2-cornerCutting, middle_Y+middleSegmentHeight/2),
//            new Vector2Int(middle_X+middleSegmentWidth/2, middle_Y+middleSegmentHeight/2-cornerCutting),
//            new Vector2Int(middle_X+middleSegmentWidth/2, middle_Y-middleSegmentHeight/2+cornerCutting),
//            new Vector2Int(middle_X+middleSegmentWidth/2-cornerCutting, middle_Y-middleSegmentHeight/2),
//        };

//        int segmentLength = 120;
//        int segmentBreadth = 30;
//        int leftBottom_X = middle_X - middleSegmentWidth - segmentBreadth;
//        int leftBottom_Y = middle_Y - middleSegmentHeight - segmentBreadth;

//        Vector2Int[] leftBottom_Points = new Vector2Int[]
//        {
//        new Vector2Int(leftBottom_X,leftBottom_Y),
//        new Vector2Int(leftBottom_X,leftBottom_Y+segmentLength-cornerCutting),
//        new Vector2Int(leftBottom_X+cornerCutting,leftBottom_Y+segmentLength),
//        new Vector2Int(leftBottom_X+segmentBreadth-cornerCutting,leftBottom_Y+segmentLength),
//        new Vector2Int(leftBottom_X+segmentBreadth,leftBottom_Y+segmentLength-cornerCutting),
//        new Vector2Int(leftBottom_X+segmentBreadth,leftBottom_Y+segmentLength*2/3),

//        new Vector2Int(leftBottom_X+segmentLength*2/3,leftBottom_Y+segmentBreadth),
//        new Vector2Int(leftBottom_X+segmentLength-cornerCutting,leftBottom_Y+segmentBreadth),
//        new Vector2Int(leftBottom_X+segmentLength,leftBottom_Y+segmentBreadth-cornerCutting),
//        new Vector2Int(leftBottom_X+segmentLength,leftBottom_Y+cornerCutting),
//        new Vector2Int(leftBottom_X+segmentLength-cornerCutting,leftBottom_Y),
//        };

//        int rightBottom_X = middle_X + middleSegmentWidth + segmentBreadth;
//        int rightBottom_Y = middle_Y - middleSegmentHeight - segmentBreadth;

//        Vector2Int[] rightBottom_Points = new Vector2Int[]
//        {
//        new Vector2Int(rightBottom_X,rightBottom_Y),
//        new Vector2Int(rightBottom_X,rightBottom_Y+segmentLength-cornerCutting),
//        new Vector2Int(rightBottom_X-cornerCutting,rightBottom_Y+segmentLength),
//        new Vector2Int(rightBottom_X-segmentBreadth+cornerCutting,rightBottom_Y+segmentLength),
//        new Vector2Int(rightBottom_X-segmentBreadth,rightBottom_Y+segmentLength-cornerCutting),
//        new Vector2Int(rightBottom_X-segmentBreadth,rightBottom_Y+segmentLength*2/3),

//        new Vector2Int(rightBottom_X-segmentLength*2/3,rightBottom_Y+segmentBreadth),
//        new Vector2Int(rightBottom_X-segmentLength+cornerCutting,rightBottom_Y+segmentBreadth),
//        new Vector2Int(rightBottom_X-segmentLength,rightBottom_Y+segmentBreadth-cornerCutting),
//        new Vector2Int(rightBottom_X-segmentLength,rightBottom_Y+cornerCutting),
//        new Vector2Int(rightBottom_X-segmentLength+cornerCutting,rightBottom_Y),
//        };

//        int rightUpper_X = middle_X + middleSegmentWidth + segmentBreadth;
//        int rightUpper_Y = middle_Y + middleSegmentHeight + segmentBreadth;

//        Vector2Int[] rightUpper_Points = new Vector2Int[]
//        {
//        new Vector2Int(rightUpper_X,rightUpper_Y),
//        new Vector2Int(rightUpper_X,rightUpper_Y-segmentLength+cornerCutting),
//        new Vector2Int(rightUpper_X-cornerCutting,rightUpper_Y-segmentLength),
//        new Vector2Int(rightUpper_X-segmentBreadth+cornerCutting,rightUpper_Y-segmentLength),
//        new Vector2Int(rightUpper_X-segmentBreadth,rightUpper_Y-segmentLength+cornerCutting),
//        new Vector2Int(rightUpper_X-segmentBreadth,rightUpper_Y-segmentLength*2/3),

//        new Vector2Int(rightUpper_X-segmentLength*2/3,rightUpper_Y-segmentBreadth),
//        new Vector2Int(rightUpper_X-segmentLength+cornerCutting,rightUpper_Y-segmentBreadth),
//        new Vector2Int(rightUpper_X-segmentLength,rightUpper_Y-segmentBreadth+cornerCutting),
//        new Vector2Int(rightUpper_X-segmentLength,rightUpper_Y-cornerCutting),
//        new Vector2Int(rightUpper_X-segmentLength+cornerCutting,rightUpper_Y),
//        };

//        int leftUpper_X = middle_X - middleSegmentWidth - segmentBreadth;
//        int leftUpper_Y = middle_Y + middleSegmentHeight + segmentBreadth;

//        Vector2Int[] leftUpper_Points = new Vector2Int[]
//        {
//        new Vector2Int(leftUpper_X,leftUpper_Y),
//        new Vector2Int(leftUpper_X,leftUpper_Y-segmentLength+cornerCutting),
//        new Vector2Int(leftUpper_X+cornerCutting,leftUpper_Y-segmentLength),
//        new Vector2Int(leftUpper_X+segmentBreadth-cornerCutting,leftUpper_Y-segmentLength),
//        new Vector2Int(leftUpper_X+segmentBreadth,leftUpper_Y-segmentLength+cornerCutting),
//        new Vector2Int(leftUpper_X+segmentBreadth,leftUpper_Y-segmentLength*2/3),

//        new Vector2Int(leftUpper_X+segmentLength*2/3,leftUpper_Y-segmentBreadth),
//        new Vector2Int(leftUpper_X+segmentLength-cornerCutting,leftUpper_Y-segmentBreadth),
//        new Vector2Int(leftUpper_X+segmentLength,leftUpper_Y-segmentBreadth+cornerCutting),
//        new Vector2Int(leftUpper_X+segmentLength,leftUpper_Y-cornerCutting),
//        new Vector2Int(leftUpper_X+segmentLength-cornerCutting,leftUpper_Y),
//        };

//        int islandWidth = 30;
//        int islandHeight = 30;

//        int leftIsland_X = (leftBottom_X + (middle_X - middleSegmentWidth / 2)) / 2;
//        int leftIsland_Y = middle_Y;

//        Vector2Int[] leftisland_Points = new Vector2Int[]
//        {
//            new Vector2Int(leftIsland_X-islandWidth/2+cornerCutting, leftIsland_Y-islandHeight/2),
//            new Vector2Int(leftIsland_X-islandWidth/2, leftIsland_Y-islandHeight/2+cornerCutting),
//            new Vector2Int(leftIsland_X-islandWidth/2, leftIsland_Y+islandHeight/2-cornerCutting),
//            new Vector2Int(leftIsland_X-islandWidth/2+cornerCutting, leftIsland_Y+islandHeight/2),
//            new Vector2Int(leftIsland_X+islandWidth/2-cornerCutting, leftIsland_Y+islandHeight/2),
//            new Vector2Int(leftIsland_X+islandWidth/2, leftIsland_Y+islandHeight/2-cornerCutting),
//            new Vector2Int(leftIsland_X+islandWidth/2, leftIsland_Y-islandHeight/2+cornerCutting),
//            new Vector2Int(leftIsland_X+islandWidth/2-cornerCutting, leftIsland_Y-islandHeight/2),
//        };

//        CreateIsland(middle_Points, ref heights);
//        CreateIsland(leftBottom_Points, ref heights);
//        CreateIsland(rightBottom_Points, ref heights);
//        CreateIsland(rightUpper_Points, ref heights);
//        CreateIsland(leftUpper_Points, ref heights);
//        CreateIsland(leftisland_Points, ref heights);

//        float unitDiameter = (float)this.width / width;
//        int x1, x2, y1, y2, x, y;
//        // LeftBottom - Middle bridge
//        x1 = leftBottom_X + segmentLength / 2 - 6;
//        x2 = middle_X - middleSegmentWidth / 2 + 6;
//        y1 = leftBottom_Y + segmentLength / 2 - 6;
//        y2 = middle_Y - middleSegmentHeight / 2 + 6;

//        x = x1;
//        y = y1;
//        while (x < x2 && y < y2)
//        {
//            CreateBridge(x * unitDiameter, y * unitDiameter);
//            x += 3;
//            y += 3;
//        }

//        // RightBottom - Middle bridge
//        x1 = rightBottom_X - segmentLength / 2 + 6;
//        x2 = middle_X + middleSegmentWidth / 2 - 6;
//        y1 = rightBottom_Y + segmentLength / 2 - 6;
//        y2 = middle_Y - middleSegmentHeight / 2 + 6;

//        x = x1 - 3;
//        y = y1;
//        while (x >= x2 && y < y2)
//        {
//            CreateBridge(x * unitDiameter, y * unitDiameter);
//            x -= 3;
//            y += 3;
//        }

//        // RightUpper - Middle bridge
//        x1 = rightUpper_X - segmentLength / 2 + 6;
//        x2 = middle_X + middleSegmentWidth / 2 - 6;
//        y1 = rightUpper_Y - segmentLength / 2 + 6;
//        y2 = middle_Y + middleSegmentHeight / 2 - 6;

//        x = x1 - 3;
//        y = y1 - 3;
//        print(y1);
//        print(y2);
//        while (x >= x2 && y >= y2)
//        {
//            CreateBridge(x * unitDiameter, y * unitDiameter);
//            x -= 3;
//            y -= 3;
//        }

//        // LeftUpper - Middle bridge
//        x1 = leftUpper_X + segmentLength / 2 - 6;
//        x2 = middle_X - middleSegmentWidth / 2 + 6;
//        y1 = leftUpper_Y - segmentLength / 2 + 6;
//        y2 = middle_Y + middleSegmentHeight / 2 - 6;

//        x = x1;
//        y = y1 - 3;
//        while (x < x2 && y >= y2)
//        {
//            CreateBridge(x * unitDiameter, y * unitDiameter);
//            x += 3;
//            y -= 3;
//        }
//        print((leftUpper_X - leftBottom_X) % 3);
//        return heights;
//    }
//    private void CreateBridge(float x, float y)
//    {
//        GameObject bridgeStoneGO = Instantiate(bridgeStonePrefab, transform);
//        bridgeStoneGO.transform.position = new Vector3(x + 1f, bridgeStoneGO.transform.position.y + 0.001f, y + 1f);

//    }

//    private void SetHeights(int x, int y, float value, ref float[,] heights)
//    {
//        heights[y, x] = value;
//    }
//    private float GetHeights(int x, int y, ref float[,] heights)
//    {
//        return heights[y, x];
//    }
//    private void CreateIsland(Vector2Int[] polygonPoints, ref float[,] heights)
//    {
//        int minX = int.MaxValue;
//        int minY = int.MaxValue;
//        int maxX = int.MinValue;
//        int maxY = int.MinValue;

//        foreach (var p in polygonPoints)
//        {
//            if (p.x < minX) minX = p.x;
//            if (p.x > maxX) maxX = p.x;
//            if (p.y < minY) minY = p.y;
//            if (p.y > maxY) maxY = p.y;
//        }

//        for (int x = minX; x <= maxX; x++)
//        {
//            for (int y = minY; y <= maxY; y++)
//            {
//                if (IsInsindePolygon(polygonPoints, new Vector2Int(x, y)))
//                {
//                    SetHeights(x, y, 1f, ref heights);
//                }
//                if (IsOnPolygon(polygonPoints, new Vector2Int(x, y), out Vector2Int outsideDir))
//                {
//                    int smoothing = 3;
//                    if (outsideDir.x == 0 || outsideDir.y == 0) smoothing = 3;
//                    else if (outsideDir.x > 0 && outsideDir.y > 0) smoothing = 2;
//                    else smoothing = 3;

//                    float smoothingStep = 1f / smoothing;
//                    for (int i = 1; i <= smoothing; i++)
//                    {
//                        float value = UnityEngine.Random.Range(1f - (i - 1) * smoothingStep, 1f - i * smoothingStep);
//                        if (GetHeights(x + outsideDir.x * i, y + outsideDir.y * i, ref heights) < value)
//                        {
//                            SetHeights(x + outsideDir.x * i, y + outsideDir.y * i, value, ref heights);
//                        }
//                    }
//                }
//            }
//        }
//    }
//    private bool IsInsindePolygon(Vector2Int[] polygonPoints, Vector2Int p)
//    {
//        var j = polygonPoints.Length - 1;
//        var inside = false;
//        for (int i = 0; i < polygonPoints.Length; j = i++)
//        {
//            var pi = polygonPoints[i];
//            var pj = polygonPoints[j];
//            if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
//                (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
//                inside = !inside;
//        }
//        return inside;
//    }
//    private bool IsOnPolygon(Vector2Int[] polygonPoints, Vector2Int p, out Vector2Int outsideDir)
//    {
//        bool isOnPolygon = false;
//        outsideDir = Vector2Int.zero;
//        if (IsInsindePolygon(polygonPoints, p))
//        {
//            if (!IsInsindePolygon(polygonPoints, p + Vector2Int.right)) { isOnPolygon = true; outsideDir = Vector2Int.right; }
//            if (!IsInsindePolygon(polygonPoints, p + Vector2Int.left)) { isOnPolygon = true; outsideDir = Vector2Int.left; }
//            if (!IsInsindePolygon(polygonPoints, p + Vector2Int.up)) { isOnPolygon = true; outsideDir = Vector2Int.up; }
//            if (!IsInsindePolygon(polygonPoints, p + Vector2Int.down)) { isOnPolygon = true; outsideDir = Vector2Int.down; }
//            if (!IsInsindePolygon(polygonPoints, p + Vector2Int.right + Vector2Int.up)) { isOnPolygon = true; outsideDir = Vector2Int.right + Vector2Int.up; }
//            if (!IsInsindePolygon(polygonPoints, p + Vector2Int.right + Vector2Int.down)) { isOnPolygon = true; outsideDir = Vector2Int.right + Vector2Int.down; }
//            if (!IsInsindePolygon(polygonPoints, p + Vector2Int.left + Vector2Int.up)) { isOnPolygon = true; outsideDir = Vector2Int.left + Vector2Int.up; }
//            if (!IsInsindePolygon(polygonPoints, p + Vector2Int.left + Vector2Int.down)) { isOnPolygon = true; outsideDir = Vector2Int.left + Vector2Int.down; }
//        }

//        return isOnPolygon;

//    }
//}