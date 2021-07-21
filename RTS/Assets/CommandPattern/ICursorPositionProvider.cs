using UnityEngine; 
namespace KK.CommandPattern
{
    /// <summary>
    /// Indicates that an object should return the screen position of an abstract cursor independently of the platform. 
    /// On PC this could be Input.mousePosition, on consoles - position of the cursor steered with an analog stick, on mobiles - a finger position for example
    /// </summary>
    public interface ICursorPositionProvider
    {
        Vector3 GetCursorPosition();
    }
}
