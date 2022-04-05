//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: The hands used by the player in the vr interaction system
//
//=============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Controller;
using Enums;
using UI;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using RenderModel = Render.RenderModel;

namespace Mechanics
{
    //-------------------------------------------------------------------------
    // Links with an appropriate SteamVR controller and facilitates
    // interactions with objects in the virtual world.
    //-------------------------------------------------------------------------
    public class Hand : MonoBehaviour
    {


        public Hand otherHand;
        public SteamVR_Input_Sources handType;

        public SteamVR_Behaviour_Pose trackedObject;

        public SteamVR_Action_Boolean grabPinchAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");

        public SteamVR_Action_Boolean grabGripAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");

        public SteamVR_Action_Vibration hapticAction = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic");

        public SteamVR_Action_Boolean uiInteractAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");



        public Camera noSteamVRFallbackCamera;
        public float noSteamVRFallbackMaxDistanceNoItem = 10.0f;
        public float noSteamVRFallbackMaxDistanceWithItem = 0.5f;
        private float noSteamVRFallbackInteractorDistance = -1.0f;

        public GameObject renderModelPrefab;
        [HideInInspector]
        public List<RenderModel> renderModels = new List<RenderModel>();
        [HideInInspector]
        public RenderModel mainRenderModel;

        public bool showDebugText = false;
        public bool spewDebugText = false;
        public bool showDebugInteractables = false;


        public bool hoverLocked { get; private set; }


        private TextMesh debugText;

        private const int ColliderArraySize = 32;

        private PlayerController playerInstance;

        private GameObject applicationLostFocusObject;

        // private SteamVR_Events.Action inputFocusAction;

        public bool isActive
        {
            get
            {
                if (trackedObject != null)
                    return trackedObject.isActive;

                return this.gameObject.activeInHierarchy;
            }
        }

        public bool isPoseValid
        {
            get
            {
                return trackedObject.isValid;
            }
        }


        //-------------------------------------------------
        // The Interactable object this Hand is currently hovering over
        //-------------------------------------------------

        public SteamVR_Behaviour_Skeleton skeleton
        {
            get
            {
                if (mainRenderModel != null)
                    return mainRenderModel.GetSkeleton();

                return null;
            }
        }

        public void ShowController(bool permanent = false)
        {
            if (mainRenderModel != null)
                mainRenderModel.SetControllerVisibility(true, permanent);

            // if (hoverhighlightRenderModel != null)
                // hoverhighlightRenderModel.SetControllerVisibility(true, permanent);
        }

        public void HideController(bool permanent = false)
        {
            if (mainRenderModel != null)
                mainRenderModel.SetControllerVisibility(false, permanent);

            // if (hoverhighlightRenderModel != null)
                // hoverhighlightRenderModel.SetControllerVisibility(false, permanent);
        }

        public void ShowSkeleton(bool permanent = false)
        {
            if (mainRenderModel != null)
                mainRenderModel.SetHandVisibility(true, permanent);

            // if (hoverhighlightRenderModel != null)
                // hoverhighlightRenderModel.SetHandVisibility(true, permanent);
        }

        public void HideSkeleton(bool permanent = false)
        {
            if (mainRenderModel != null)
                mainRenderModel.SetHandVisibility(false, permanent);

            // if (hoverhighlightRenderModel != null)
                // hoverhighlightRenderModel.SetHandVisibility(false, permanent);
        }

        public bool HasSkeleton()
        {
            return mainRenderModel != null && mainRenderModel.GetSkeleton() != null;
        }

        public void Show()
        {
            SetVisibility(true);
        }

        public void Hide()
        {
            SetVisibility(false);
        }

        public void SetVisibility(bool visible)
        {
            if (mainRenderModel != null)
                mainRenderModel.SetVisibility(visible);
        }

        public void SetSkeletonRangeOfMotion(EVRSkeletalMotionRange newRangeOfMotion, float blendOverSeconds = 0.1f)
        {
            for (int renderModelIndex = 0; renderModelIndex < renderModels.Count; renderModelIndex++)
            {
                renderModels[renderModelIndex].SetSkeletonRangeOfMotion(newRangeOfMotion, blendOverSeconds);
            }
        }

        public void SetTemporarySkeletonRangeOfMotion(SkeletalMotionRangeChange temporaryRangeOfMotionChange, float blendOverSeconds = 0.1f)
        {
            for (int renderModelIndex = 0; renderModelIndex < renderModels.Count; renderModelIndex++)
            {
                renderModels[renderModelIndex].SetTemporarySkeletonRangeOfMotion(temporaryRangeOfMotionChange, blendOverSeconds);
            }
        }

        public void ResetTemporarySkeletonRangeOfMotion(float blendOverSeconds = 0.1f)
        {
            for (int renderModelIndex = 0; renderModelIndex < renderModels.Count; renderModelIndex++)
            {
                renderModels[renderModelIndex].ResetTemporarySkeletonRangeOfMotion(blendOverSeconds);
            }
        }

        public void SetAnimationState(int stateValue)
        {
            for (int renderModelIndex = 0; renderModelIndex < renderModels.Count; renderModelIndex++)
            {
                renderModels[renderModelIndex].SetAnimationState(stateValue);
            }
        }

        public void StopAnimation()
        {
            for (int renderModelIndex = 0; renderModelIndex < renderModels.Count; renderModelIndex++)
            {
                renderModels[renderModelIndex].StopAnimation();
            }
        }


        //-------------------------------------------------
        // Attach a GameObject to this GameObject
        //
        // objectToAttach - The GameObject to attach
        // flags - The flags to use for attaching the object
        // attachmentPoint - Name of the GameObject in the hierarchy of this Hand which should act as the attachment point for this GameObject
        //-------------------------------------------------

        public void ForceHoverUnlock()
        {
            hoverLocked = false;
        }

        //-------------------------------------------------
        protected virtual void Awake()
        {
            // inputFocusAction = SteamVR_Events.InputFocusAction(OnInputFocus);

            applicationLostFocusObject = new GameObject("_application_lost_focus");
            applicationLostFocusObject.transform.parent = transform;
            applicationLostFocusObject.SetActive(false);

            if (trackedObject == null)
            {
                trackedObject = this.gameObject.GetComponent<SteamVR_Behaviour_Pose>();

                if (trackedObject != null)
                    trackedObject.onTransformUpdatedEvent += OnTransformUpdated;
            }
        }

        protected virtual void OnDestroy()
        {
            if (trackedObject != null)
            {
                trackedObject.onTransformUpdatedEvent -= OnTransformUpdated;
            }
        }

        protected virtual void OnTransformUpdated(SteamVR_Behaviour_Pose updatedPose, SteamVR_Input_Sources updatedSource)
        {
        }

        //-------------------------------------------------
        protected virtual IEnumerator Start()
        {
            // save off player instance
            playerInstance = PlayerController.instance;
            if (!playerInstance)
            {
                Debug.LogError("<b>[SteamVR Interaction]</b> No player instance found in Hand Start()", this);
            }

            // allocate array for colliders

            // We are a "no SteamVR fallback hand" if we have this camera set
            // we'll use the right mouse to look around and left mouse to interact
            // - don't need to find the device
            if (noSteamVRFallbackCamera)
            {
                yield break;
            }

            //Debug.Log( "<b>[SteamVR Interaction]</b> Hand - initializing connection routine" );

            while (true)
            {
                if (isPoseValid)
                {
                    InitController();
                    break;
                }

                yield return null;
            }
        }


        //-------------------------------------------------
        protected virtual void UpdateHovering()
        {
            if ((noSteamVRFallbackCamera == null) && (isActive == false))
            {
                return;
            }

            if (hoverLocked)
                return;

            if (applicationLostFocusObject.activeSelf)
                return;


            // Hover on this one
        }



        //-------------------------------------------------
        protected virtual void UpdateNoSteamVRFallback()
        {
            if (noSteamVRFallbackCamera)
            {
                Ray ray = noSteamVRFallbackCamera.ScreenPointToRay(Input.mousePosition);


                    // Not holding down the mouse:
                    // cast out a ray to see what we should mouse over

                    // Don't want to hit the hand and anything underneath it
                    // So move it back behind the camera when we do the raycast
                    Vector3 oldPosition = transform.position;
                    transform.position = noSteamVRFallbackCamera.transform.forward * (-1000.0f);

                    RaycastHit raycastHit;
                    if (Physics.Raycast(ray, out raycastHit, noSteamVRFallbackMaxDistanceNoItem))
                    {
                        transform.position = raycastHit.point;

                        // Remember this distance in case we click and drag the mouse
                        noSteamVRFallbackInteractorDistance = Mathf.Min(noSteamVRFallbackMaxDistanceNoItem, raycastHit.distance);
                    }
                    else if (noSteamVRFallbackInteractorDistance > 0.0f)
                    {
                        // Move it around at the distance we last had a hit
                        transform.position = ray.origin + Mathf.Min(noSteamVRFallbackMaxDistanceNoItem, noSteamVRFallbackInteractorDistance) * ray.direction;
                    }
                    else
                    {
                        // Didn't hit, just leave it where it was
                        transform.position = oldPosition;
                    }
            }
        }


        //-------------------------------------------------
        private void UpdateDebugText()
        {
            if (showDebugText)
            {
                if (debugText == null)
                {
                    debugText = new GameObject("_debug_text").AddComponent<TextMesh>();
                    debugText.fontSize = 120;
                    debugText.characterSize = 0.001f;
                    debugText.transform.parent = transform;

                    debugText.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                }

                if (handType == SteamVR_Input_Sources.RightHand)
                {
                    debugText.transform.localPosition = new Vector3(-0.05f, 0.0f, 0.0f);
                    debugText.alignment = TextAlignment.Right;
                    debugText.anchor = TextAnchor.UpperRight;
                }
                else
                {
                    debugText.transform.localPosition = new Vector3(0.05f, 0.0f, 0.0f);
                    debugText.alignment = TextAlignment.Left;
                    debugText.anchor = TextAnchor.UpperLeft;
                }

                debugText.text = string.Format(
                    "Hovering: {0}\n" +
                    "Hover Lock: {1}\n" +
                    "Attached: {2}\n" +
                    "Total Attached: {3}\n" +
                    "Type: {4}\n",
                    hoverLocked,
                    handType.ToString());
            }
            else
            {
                if (debugText != null)
                {
                    Destroy(debugText.gameObject);
                }
            }
        }


        //-------------------------------------------------
        protected virtual void OnEnable()
        {
            // inputFocusAction.enabled = true;

        }


        //-------------------------------------------------
        protected virtual void OnDisable()
        {
            // inputFocusAction.enabled = false;

            CancelInvoke();
        }


        //-------------------------------------------------
        protected virtual void Update()
        {
            UpdateNoSteamVRFallback();

        }

        /// <summary>
        /// Returns true when the hand is currently hovering over the interactable passed in
        /// </summary>



        protected const float MaxVelocityChange = 10f;
        protected const float VelocityMagic = 6000f;
        protected const float AngularVelocityMagic = 50f;
        protected const float MaxAngularVelocityChange = 20f;




        

        

       

        //-------------------------------------------------


        //-------------------------------------------------
        private void HandDebugLog(string msg)
        {
            if (spewDebugText)
            {
                Debug.Log("<b>[SteamVR Interaction]</b> Hand (" + this.name + "): " + msg);
            }
        }


        //-------------------------------------------------
        // Continue to hover over this object indefinitely, whether or not the Hand moves out of its interaction trigger volume.
        //
        // interactable - The Interactable to hover over indefinitely.
        //-------------------------------------------------



        //-------------------------------------------------
        // Stop hovering over this object indefinitely.
        //
        // interactable - The hover-locked Interactable to stop hovering over indefinitely.
        //-------------------------------------------------

        public void TriggerHapticPulse(ushort microSecondsDuration)
        {
            float seconds = (float)microSecondsDuration / 1000000f;
            hapticAction.Execute(0, seconds, 1f / seconds, 1, handType);
        }

        public void TriggerHapticPulse(float duration, float frequency, float amplitude)
        {
            hapticAction.Execute(0, duration, frequency, amplitude, handType);
        }

        public void ShowGrabHint()
        {
            ControllerButtonHints.ShowButtonHint(this, grabGripAction); //todo: assess
        }

        public void HideGrabHint()
        {
            ControllerButtonHints.HideButtonHint(this, grabGripAction); //todo: assess
        }

        public void ShowGrabHint(string text)
        {
            ControllerButtonHints.ShowTextHint(this, grabGripAction, text);
        }

        public GrabTypes GetGrabStarting(GrabTypes explicitType = GrabTypes.None)
        {
            if (explicitType != GrabTypes.None)
            {
                if (noSteamVRFallbackCamera)
                {
                    if (Input.GetMouseButtonDown(0))
                        return explicitType;
                    else
                        return GrabTypes.None;
                }

                if (explicitType == GrabTypes.Pinch && grabPinchAction.GetStateDown(handType))
                    return GrabTypes.Pinch;
                if (explicitType == GrabTypes.Grip && grabGripAction.GetStateDown(handType))
                    return GrabTypes.Grip;
            }
            else
            {
                if (noSteamVRFallbackCamera)
                {
                    if (Input.GetMouseButtonDown(0))
                        return GrabTypes.Grip;
                    else
                        return GrabTypes.None;
                }

                if (grabPinchAction != null && grabPinchAction.GetStateDown(handType))
                    return GrabTypes.Pinch;
                if (grabGripAction != null && grabGripAction.GetStateDown(handType))
                    return GrabTypes.Grip;
            }

            return GrabTypes.None;
        }

        public GrabTypes GetGrabEnding(GrabTypes explicitType = GrabTypes.None)
        {
            if (explicitType != GrabTypes.None)
            {
                if (noSteamVRFallbackCamera)
                {
                    if (Input.GetMouseButtonUp(0))
                        return explicitType;
                    else
                        return GrabTypes.None;
                }

                if (explicitType == GrabTypes.Pinch && grabPinchAction.GetStateUp(handType))
                    return GrabTypes.Pinch;
                if (explicitType == GrabTypes.Grip && grabGripAction.GetStateUp(handType))
                    return GrabTypes.Grip;
            }
            else
            {
                if (noSteamVRFallbackCamera)
                {
                    if (Input.GetMouseButtonUp(0))
                        return GrabTypes.Grip;
                    else
                        return GrabTypes.None;
                }

                if (grabPinchAction.GetStateUp(handType))
                    return GrabTypes.Pinch;
                if (grabGripAction.GetStateUp(handType))
                    return GrabTypes.Grip;
            }

            return GrabTypes.None;
        }

        public bool IsGrabbingWithType(GrabTypes type)
        {
            if (noSteamVRFallbackCamera)
            {
                if (Input.GetMouseButton(0))
                    return true;
                else
                    return false;
            }

            switch (type)
            {
                case GrabTypes.Pinch:
                    return grabPinchAction.GetState(handType);

                case GrabTypes.Grip:
                    return grabGripAction.GetState(handType);

                default:
                    return false;
            }
        }

        public bool IsGrabbingWithOppositeType(GrabTypes type)
        {
            if (noSteamVRFallbackCamera)
            {
                if (Input.GetMouseButton(0))
                    return true;
                else
                    return false;
            }

            switch (type)
            {
                case GrabTypes.Pinch:
                    return grabGripAction.GetState(handType);

                case GrabTypes.Grip:
                    return grabPinchAction.GetState(handType);

                default:
                    return false;
            }
        }

        public GrabTypes GetBestGrabbingType()
        {
            return GetBestGrabbingType(GrabTypes.None);
        }

        public GrabTypes GetBestGrabbingType(GrabTypes preferred, bool forcePreference = false)
        {
            if (noSteamVRFallbackCamera)
            {
                if (Input.GetMouseButton(0))
                    return preferred;
                else
                    return GrabTypes.None;
            }

            if (preferred == GrabTypes.Pinch)
            {
                if (grabPinchAction.GetState(handType))
                    return GrabTypes.Pinch;
                else if (forcePreference)
                    return GrabTypes.None;
            }
            if (preferred == GrabTypes.Grip)
            {
                if (grabGripAction.GetState(handType))
                    return GrabTypes.Grip;
                else if (forcePreference)
                    return GrabTypes.None;
            }

            if (grabPinchAction.GetState(handType))
                return GrabTypes.Pinch;
            if (grabGripAction.GetState(handType))
                return GrabTypes.Grip;

            return GrabTypes.None;
        }


        //-------------------------------------------------
        private void InitController()
        {
            if (spewDebugText)
                HandDebugLog("Hand " + name + " connected with type " + handType.ToString());

            bool hadOldRendermodel = mainRenderModel != null;
            EVRSkeletalMotionRange oldRM_rom = EVRSkeletalMotionRange.WithController;
            if (hadOldRendermodel)
                oldRM_rom = mainRenderModel.GetSkeletonRangeOfMotion;


            foreach (RenderModel r in renderModels)
            {
                if (r != null)
                    Destroy(r.gameObject);
            }

            renderModels.Clear();

            GameObject renderModelInstance = GameObject.Instantiate(renderModelPrefab);
            renderModelInstance.layer = gameObject.layer;
            renderModelInstance.tag = gameObject.tag;
            renderModelInstance.transform.parent = this.transform;
            renderModelInstance.transform.localPosition = Vector3.zero;
            renderModelInstance.transform.localRotation = Quaternion.identity;
            renderModelInstance.transform.localScale = renderModelPrefab.transform.localScale;

            //TriggerHapticPulse(800);  //pulse on controller init

            int deviceIndex = trackedObject.GetDeviceIndex();

            mainRenderModel = renderModelInstance.GetComponent<RenderModel>();
            renderModels.Add(mainRenderModel);

            if (hadOldRendermodel)
                mainRenderModel.SetSkeletonRangeOfMotion(oldRM_rom);

            this.BroadcastMessage("SetInputSource", handType, SendMessageOptions.DontRequireReceiver); // let child objects know we've initialized
            this.BroadcastMessage("OnHandInitialized", deviceIndex, SendMessageOptions.DontRequireReceiver); // let child objects know we've initialized
        }

        public void SetRenderModel(GameObject prefab)
        {
            renderModelPrefab = prefab;

            if (mainRenderModel != null && isPoseValid)
                InitController();
        }

        // public void SetHoverRenderModel(RenderModel hoverRenderModel)
        // {
        //     // hoverhighlightRenderModel = hoverRenderModel;
        //     renderModels.Add(hoverRenderModel);
        // }

        public int GetDeviceIndex()
        {
            return trackedObject.GetDeviceIndex();
        }
    }


    [System.Serializable]
    public class HandEvent : UnityEvent<Hand> { }
}