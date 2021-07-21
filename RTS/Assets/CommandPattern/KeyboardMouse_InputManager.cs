using UnityEngine;
using ZombieApocalypseTest;
namespace KK.CommandPattern
{
    /// <summary>
    /// InputManager for a PC when playing with a mouse and keyboard. Here you can bind or rebind commands and keys. 
    /// </summary>
    public sealed class KeyboardMouse_InputManager : InputManager
    {
        public ICommand LMB, RMB, alpha1, alpha2, mouseMovement;        
        //[SerializeField] PlayerController playerController;        
        private bool initialized = false;

        public void Init()
        {
            //mouseMovement = new PlayerRotateCommand(playerController, this);
            //LMB = new PlayerShootCommand(playerController);
            //alpha1 = new TriggerAbilityCommand(playerController, 0);
            //alpha2 = new TriggerAbilityCommand(playerController, 1);

            initialized = true;
        }
        public override Vector3 GetCursorPosition()
        {
            return Input.mousePosition;
        }

        private void Update()
        {
            if (!initialized) return;
            
            mouseMovement.Execute();

            if (Input.GetKeyDown(KeyCode.Mouse0))  LMB.Execute();
            if (Input.GetKeyDown(KeyCode.Alpha1)) alpha1.Execute();
            if (Input.GetKeyDown(KeyCode.Alpha2)) alpha2.Execute();

            //
            // so on for the rest of the keys... 
            // 
        }
    }
}