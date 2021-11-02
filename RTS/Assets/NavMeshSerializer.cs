using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
public class NavMeshSerializer : MonoBehaviour
{
    //public void Bake()
    //{
    //    var triangulation = NavMesh.CalculateTriangulation();
    //    //CleanTriangulation(ref triangulation);
    //    string content = "";
    //    content += "MapOriginX:" + 30 + Environment.NewLine;
    //    content += "MapOriginY:" + 3 + Environment.NewLine;
    //    content += "MapOriginZ:" + 30 + Environment.NewLine;
    //    content += "MapWidth:" + 195 + Environment.NewLine;
    //    content += "MapHeight:" + 195 + Environment.NewLine;
    //    content += "VertexCount:" + triangulation.vertices.Length + Environment.NewLine;
    //    foreach (var vertex in triangulation.vertices)
    //    {
    //        content += vertex.x + ":" + 3f + ":" + vertex.z + Environment.NewLine;
    //    }
    //    content += "IndexCount:" + triangulation.indices.Length + Environment.NewLine;
    //    for (int i = 0; i < triangulation.indices.Length; i += 3)
    //    {
    //        content += triangulation.indices[i] + ":" + triangulation.indices[i + 1] + ":" + triangulation.indices[i + 2] + Environment.NewLine;
    //    }

    //    File.WriteAllText("Default_4_players.txt", content);

    //    Debug.Log("Baking finished");
    //}
    //private void CleanTriangulation(ref NavMeshTriangulation t)
    //{
    //    List<Vector3> verts = new List<Vector3>();
    //    for (int i = 0; i < t.vertices.Length; i++)
    //    {
    //        if (!verts.Contains(t.vertices[i]))
    //        {
    //            verts.Add(t.vertices[i]);
    //        }
    //        else
    //        {
    //            int index = verts.IndexOf(t.vertices[i]);
    //            for (int j = 0; j < t.indices.Length; j++)
    //            {
    //                if (t.indices[j] == i) t.indices[j] = index;
    //            }
    //        }
    //    }
    //    t.vertices = verts.ToArray();

    //    List<Vector3Int> indicesVectors = new List<Vector3Int>();
    //    for (int i = 0; i < t.indices.Length; i += 3)
    //    {
    //        Vector3Int v = new Vector3Int(t.indices[i], t.indices[i + 1], t.indices[i + 2]);
    //        if (!indicesVectors.Contains(v)) indicesVectors.Add(v);
    //    }
    //    List<int> indices = new List<int>();
    //    foreach (var v in indicesVectors)
    //    {
    //        indices.Add(v.x);
    //        indices.Add(v.y);
    //        indices.Add(v.z);
    //    }
    //    t.indices = indices.ToArray();
    //}
    public void Bake()
    {
        Vector2 a = new Vector2(62f, 35.5f);
        Vector2 b = new Vector2(62.5f,39.5f);
        Vector2 c = new Vector2(63.5f, 39.5f);
        
        Vector2 bottomLeft = new Vector2(49.5f, 30f);
        Vector2 bottomRight = bottomLeft + 19.5f * Vector2.right;
        Vector2 topLeft = bottomLeft + 19.5f * Vector2.up;
        Vector2 topRight = bottomLeft + new Vector2(19.5f, 19.5f);

        Debug.Log("INTEERSECT: ");
        //Debug.Log(LineLineIntersection(bottomLeft, bottomRight, a, b));
        //Debug.Log(LineLineIntersection(bottomLeft, bottomRight, a, c));
        //Debug.Log(LineLineIntersection(bottomLeft, bottomRight, b, c));

        //Debug.Log(LineLineIntersection(bottomLeft, topLeft, a, b));
        //Debug.Log(LineLineIntersection(bottomLeft, topLeft, a, c));
        //Debug.Log(LineLineIntersection(bottomLeft, topLeft, b, c));

        //Debug.Log(LineLineIntersection(bottomRight, topRight, a, b));
        //Debug.Log(LineLineIntersection(bottomRight, topRight, a, c));
        //Debug.Log(LineLineIntersection(bottomRight, topRight, b, c));

        //Debug.Log(LineLineIntersection(topLeft, topRight, a, b));
        //Debug.Log(LineLineIntersection(topLeft, topRight, a, c));
        //Debug.Log(LineLineIntersection(topLeft, topRight, b, c));

        Debug.Log(PointInRectangle(a, bottomRight, bottomLeft, topLeft, topRight));
        Debug.Log(PointInRectangle(b, bottomRight, bottomLeft, topLeft, topRight));
        Debug.Log(PointInRectangle(c, bottomRight, bottomLeft, topLeft, topRight));
        //var t = NavMesh.CalculateTriangulation();
        //for (int i = 0; i < t.indices.Length; i+=3)
        //{
        //    Vector3 a = t.vertices[t.indices[i]];
        //    Vector3 b = t.vertices[t.indices[i+1]];
        //    Vector3 c = t.vertices[t.indices[i+2]];
        //    Debug.LogError("asd");
        //    Debug.Log(a.ToString() + b.ToString() + c.ToString());
        //    Debug.Log(t.indices[i] +":" + t.indices[i + 1] + ":" + t.indices[i + 2]);
        //    Debug.LogError("asd");


        //}
    }
    public static bool PointInRectangle(Vector2 p, Vector2 r1, Vector2 r2, Vector2 r3, Vector2 r4)
    {
        var AB = r2 - r1;
        var AM = p - r1;
        var BC = r3 - r2;
        var BM = p - r2;
        var dotABAM = Vector2.Dot(AB, AM);
        var dotABAB = Vector2.Dot(AB, AB);
        var dotBCBM = Vector2.Dot(BC, BM);
        var dotBCBC = Vector2.Dot(BC, BC);
        return 0 <= dotABAM && dotABAM <= dotABAB && 0 <= dotBCBM && dotBCBM <= dotBCBC;
    }
    private static float Cross(Vector2 lhs, Vector2 rhs)
    {
        return lhs.x* rhs.y - lhs.y * rhs.x;
    }
    private static bool LineLineIntersection(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
    {
        Vector2 r = q1 - p1;
        Vector2 s = q2 - p2;
        float r_x_s = Cross(r, s);
        Vector2 qmp = p2 - p1;
        float qmp_x_r = Cross(qmp, r);
        float qmp_x_s = Cross(qmp, s);
        float t = qmp_x_s / r_x_s;
        float u = qmp_x_r / r_x_s;
        if (r_x_s == 0f && qmp_x_r == 0f)
        {
            //colinear
            float t0 = Vector2.Dot(qmp, r) / Vector2.Dot(r, r);
            float t1 = t0 + Vector2.Dot(s, r) / Vector2.Dot(r, r);
            //Console.WriteLine(p1.x + " , " + p1.y);
            //Console.WriteLine(q1.x + " , " + q1.y);
            //Console.WriteLine(p2.x + " , " + p2.y);
            //Console.WriteLine(q2.x + " , " + q2.y);
            //Console.WriteLine("1");

            return false; // :/
        }
        else if (r_x_s == 0f && qmp_x_r != 0f)
        {
            // parallel and non intersecting
            return false;
        }
        else if (r_x_s != 0f && t >= 0f && t <= 1f && u >= 0f && u <= 1f)
        {
            Vector2 intersectionPoint = p1 + t * r;
            return true;
        }
        else
        {
            // not parallel but do not intersect
            return false;
        }
    }
    //public Material mat;
    //public void Bake()
    //{
    //    var t = NavMesh.CalculateTriangulation();
    //    for (int i = 0; i < t.indices.Length; i+=3)
    //    {
    //        GameObject go = new GameObject();
    //        var filter = go.AddComponent<MeshFilter>();
    //        Mesh mesh = new Mesh();
    //        List<Vector3> vertices = new List<Vector3>();
    //        vertices.Add(t.vertices[t.indices[i]]);
    //        vertices.Add(t.vertices[t.indices[i+1]]);
    //        vertices.Add(t.vertices[t.indices[i+2]]);
    //        mesh.SetVertices(vertices);
    //        mesh.SetIndices(new int[] { 0,1,2 }, MeshTopology.Triangles, 0);
    //        mesh.colors = new Color[] { Color.red, Color.red, Color.red };
    //        filter.mesh = mesh;

    //        var renderer = go.AddComponent<MeshRenderer>();
    //        renderer.material = mat;
    //        renderer.material.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);
    //    }
    //}
}
