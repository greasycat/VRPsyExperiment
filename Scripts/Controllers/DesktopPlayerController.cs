//rongfei@ucsb.edu

using Core;
using Event;
using UnityEngine;
using Valve.VR;

namespace Controller
{
    public class DesktopPlayerController : MonoBehaviour
    {
        //Basic Character controls
        [Range(1.0f, 10f)]
        public float speed = 4.0f;
        private CharacterController characterController;
        
        //Event

        private void OnEnable()
        {
            characterController = PlayerController.instance.characterController;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleMouseRotation()
        {
            var deltaX = Input.GetAxis("Horizontal") * speed;
            var deltaZ = Input.GetAxis("Vertical") * speed;
            var movement = new Vector3(deltaX, 0, deltaZ);
            movement = Vector3.ClampMagnitude(movement, speed);
            movement.y = -10f;
            movement *= Time.deltaTime;
            movement = transform.TransformDirection(movement);
            characterController.Move(movement);
            transform.Rotate(0, Input.GetAxis("Mouse X") * speed, 0);
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.F)) KeyF();
            if (Input.GetKeyDown(KeyCode.E)) KeyE();
            if (Input.GetKeyDown(KeyCode.Space)) KeySpace();
            
            HandleMouseRotation();
        }

        private void KeyF()
        {
            
        }

        private void KeyE()
        {
            Locator.instance.GetService<MazeSystem>().ClearMaze();
        }

        private void KeySpace()
        {
            
        }
    }
}