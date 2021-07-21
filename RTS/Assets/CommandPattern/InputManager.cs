using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KK.CommandPattern
{
    /// <summary>
    /// Base input manager
    /// </summary>
    public abstract class InputManager : MonoBehaviour, ICursorPositionProvider
    {
        public abstract Vector3 GetCursorPosition();        
    }
}