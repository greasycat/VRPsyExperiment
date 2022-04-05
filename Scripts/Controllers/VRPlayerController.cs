using System;
using Core;
using UnityEngine;
using UnityEngine.Serialization;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Controller
{
    public class VRPlayerController : MonoBehaviour
    {

        [Range(1f, 3f)]
        public float movingSpeed = 3.0f;

        [SerializeField] private Camera vrCamera;
        public SteamVR_Input_Sources leftHand;
        public SteamVR_Input_Sources rightHand;
        public SteamVR_Action_Vector2 actionMove = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("Debugger", "Move");
        public SteamVR_Action_Vector2 actionTurn = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("Debugger", "Turn");
        public SteamVR_Action_Boolean actionTrigger =
            SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Debugger", "Trigger");

        private void OnEnable()
        {
            actionTrigger.onState += ControllerTrigger;
            actionMove.onAxis += ControllerMove;
        }

        private void ControllerTrigger(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSources)
        {
            Locator.instance.GetService<MazeSystem>().ClearMaze();
        }

        private void ControllerMove(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSources, Vector2 axis, Vector2 delta)
        {
            var movement = vrCamera.transform.TransformDirection(new Vector3(axis.x, 0, axis.y))*Time.deltaTime*movingSpeed;
            movement = Vector3.ProjectOnPlane(movement, Vector3.up);
            PlayerController.instance.characterController.Move(movement);
        }

        private void Update()
        {
        }
    }
}