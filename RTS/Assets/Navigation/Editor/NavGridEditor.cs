using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace KK.NavGrid
{
    [CustomEditor(typeof(NavGrid))]
    public class NavGridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Bake"))
            {
                (target as NavGrid).Bake();
            }
        }
    }
}