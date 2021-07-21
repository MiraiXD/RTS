using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Generate"))
        {
            (target as TerrainGenerator).Generate();
        }
        if (GUILayout.Button("Place Decorations Randomly"))
        {
            (target as TerrainGenerator).PlaceDecorationsRandomly();
        }
        if (GUILayout.Button("Snap Decorations To Grid"))
        {
            (target as TerrainGenerator).SnapDecorationsToGrid();
        }
    }
}
