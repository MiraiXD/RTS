using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace KK.NavGrid
{
    [CustomEditor(typeof(CreateNavGrid))]
    public class CreateNavGridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Bake"))
            {
                (target as CreateNavGrid).Bake();
            }
        }
    }
}