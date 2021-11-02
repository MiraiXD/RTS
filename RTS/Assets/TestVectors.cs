using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVectors : MonoBehaviour
{
    Vector2 a, b;
    private void Update()
    {
        if (a == Vector2.zero && Input.GetKeyDown(KeyCode.Mouse0)) a = Input.mousePosition;
        else if (a != Vector2.zero && Input.GetKeyDown(KeyCode.Mouse0)) b = Input.mousePosition;
        if(a != Vector2.zero && b != Vector2.zero)
        {
            bool right = CheckVectorsOrder(b - a, new Vector2(Input.mousePosition.x, Input.mousePosition.y) - a);
            if (right) print("RIGHT");
            else print("LEFT");
        }
    }
    private static bool CheckVectorsOrder(Vector2 left, Vector2 right)
    {
        return right.x * left.y - right.y * left.x >= 0f;
    }
}
