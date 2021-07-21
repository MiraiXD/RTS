using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int width = 512;
    public int height = 512;
    public int depth = 20;

    public GameObject bridgeStonePrefab;

    private Terrain terrain;
    public void Generate()
    {
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);

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


        // Middle Segment
        int middleSegmentWidth = 120;
        int middleSegmentHeight = 120;
        int segmentMiddle_X = width / 2 - middleSegmentWidth / 2;
        int segmentMiddle_Y = height / 2 - middleSegmentHeight / 2;

        //CreateIsland(segmentMiddle_X, segmentMiddle_Y, middleSegmentWidth, middleSegmentHeight, ref heights);

        //CreateCoastLine(segmentMiddle_X, segmentMiddle_Y, middleSegmentWidth, 0, Vector2Int.down, ref heights);
        //CreateCoastLine(segmentMiddle_X, segmentMiddle_Y, 0, middleSegmentHeight, Vector2Int.left, ref heights);
        //CreateCoastLine(segmentMiddle_X + middleSegmentWidth, segmentMiddle_Y, 0, middleSegmentHeight, Vector2Int.right, ref heights);
        //CreateCoastLine(segmentMiddle_X, segmentMiddle_Y + middleSegmentHeight, middleSegmentWidth, 0, Vector2Int.up, ref heights);

        int segmentWidth = 30;
        int segmentHeight = 30;

        // Left Upper Segment
        int segmentLeftUpper_X = segmentMiddle_X - middleSegmentWidth / 2;
        int segmentLeftUpper_Y = segmentMiddle_Y + middleSegmentHeight + middleSegmentHeight / 2;

        //CreateIsland(segmentLeftUpper_X, segmentLeftUpper_Y - segmentHeight, segmentWidth, segmentHeight, ref heights);
        //CreateIsland(segmentLeftUpper_X + segmentWidth, segmentLeftUpper_Y - segmentHeight, segmentWidth, segmentHeight, ref heights);
        //CreateIsland(segmentLeftUpper_X, segmentLeftUpper_Y - segmentHeight - segmentHeight, segmentWidth, segmentHeight, ref heights);

        //CreateCoastLine(segmentLeftUpper_X, segmentLeftUpper_Y - 1 - 2 * segmentHeight, 0, segmentHeight * 2, Vector2Int.left, ref heights);
        //CreateCoastLine(segmentLeftUpper_X + segmentWidth, segmentLeftUpper_Y - 2 * segmentHeight - 1, 0, segmentHeight, Vector2Int.right, ref heights);
        //CreateCoastLine(segmentLeftUpper_X + segmentWidth + 1, segmentLeftUpper_Y - segmentHeight, segmentWidth + 1, 0, Vector2Int.down, ref heights);
        //CreateCoastLine(segmentLeftUpper_X + 2 * segmentWidth, segmentLeftUpper_Y - segmentHeight, 0, segmentHeight, Vector2Int.right, ref heights);
        //CreateCoastLine(segmentLeftUpper_X, segmentLeftUpper_Y, 2 * segmentWidth, 0, Vector2Int.up, ref heights);
        //CreateCoastLine(segmentLeftUpper_X, segmentLeftUpper_Y - 2 * segmentHeight, segmentWidth, 0, Vector2Int.down, ref heights);

        // Left Bottom Segment
        

        //CreateIsland(segmentLeftBottom_X, segmentLeftBottom_Y, segmentWidth, segmentHeight, ref heights);
        //CreateIsland(segmentLeftBottom_X + segmentWidth, segmentLeftBottom_Y, segmentWidth + segmentWidth / 2, segmentHeight, ref heights);
        //CreateIsland(segmentLeftBottom_X, segmentLeftBottom_Y + segmentHeight, segmentWidth, segmentHeight + segmentHeight / 2, ref heights);
        //CreateIsland(segmentLeftBottom_X - segmentWidth, segmentLeftBottom_Y - segmentHeight, segmentWidth, segmentHeight, ref heights);
        //CreateIsland(segmentLeftBottom_X, segmentLeftBottom_Y - segmentHeight, segmentWidth, segmentHeight, ref heights);
        //CreateIsland(segmentLeftBottom_X - segmentWidth, segmentLeftBottom_Y, segmentWidth, segmentHeight, ref heights);      

        int segmentLength = 120;
        int segmentBreadth = 30;
        int cornerCutting = 10;
        int segmentLeftBottom_X = segmentMiddle_X - middleSegmentWidth / 2 - segmentBreadth;
        int segmentLeftBottom_Y = segmentMiddle_Y - middleSegmentHeight / 2 - segmentBreadth;
        Vector2Int[] polygonPoints = new Vector2Int[]
        {
        new Vector2Int(segmentLeftBottom_X,segmentLeftBottom_Y),
        new Vector2Int(segmentLeftBottom_X,segmentLeftBottom_Y+segmentLength-cornerCutting),
        new Vector2Int(segmentLeftBottom_X+cornerCutting,segmentLeftBottom_Y+segmentLength),
        new Vector2Int(segmentLeftBottom_X+segmentBreadth-cornerCutting,segmentLeftBottom_Y+segmentLength),
        new Vector2Int(segmentLeftBottom_X+segmentBreadth,segmentLeftBottom_Y+segmentLength-cornerCutting),
        new Vector2Int(segmentLeftBottom_X+segmentBreadth,segmentLeftBottom_Y+segmentLength*2/3),

        new Vector2Int(segmentLeftBottom_X+segmentLength*2/3,segmentLeftBottom_Y+segmentBreadth),
        new Vector2Int(segmentLeftBottom_X+segmentLength-cornerCutting,segmentLeftBottom_Y+segmentBreadth),
        new Vector2Int(segmentLeftBottom_X+segmentLength,segmentLeftBottom_Y+segmentBreadth-cornerCutting),
        new Vector2Int(segmentLeftBottom_X+segmentLength,segmentLeftBottom_Y+cornerCutting),
        new Vector2Int(segmentLeftBottom_X+segmentLength-cornerCutting,segmentLeftBottom_Y),
        };
        CreateIsland(polygonPoints, ref heights);

        float unitDiameter = (float)this.width / width;
        int x1 = segmentLeftBottom_X + segmentLength/2-6;
        int x2 = segmentMiddle_X - 1;
        int y1 = segmentLeftBottom_Y + segmentLength/2-6;
        int y2 = segmentMiddle_Y - 1;

        int x = x1, y = y1;
        while (x < x2 && y < y2)
        {
            CreateBridge(x * unitDiameter, y * unitDiameter);
            x += 3;
            y += 3;
        }


        return heights;
    }
    private void CreateBridge(float x, float y)
    {
        GameObject bridgeStoneGO = Instantiate(bridgeStonePrefab, transform);
        bridgeStoneGO.transform.position = new Vector3(x + 1f, bridgeStoneGO.transform.position.y, y + 1f);

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
                    for(int i=1; i<= smoothing; i++)
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
