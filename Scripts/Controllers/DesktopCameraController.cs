using System;
using UnityEngine;

namespace Controller
{
    public class DesktopCameraController : MonoBehaviour
    {
        
        public float rotationSpeed = 9.0f;
        public float maximumVerticalRotation = 45.0f;
        public float minimumVerticalRotation = -45.0f;
        private float verticalRotation = 0f;

        private void Start()
        {
            var body = GetComponent<Rigidbody>();
            if (body != null) {
                body.freezeRotation = true;
            }
        }

        private void Update()
        {
            verticalRotation -= Input.GetAxis("Mouse Y") * rotationSpeed;
            verticalRotation = Mathf.Clamp(verticalRotation, minimumVerticalRotation, maximumVerticalRotation);
            var horizontalRotation = transform.localEulerAngles.y;
            
            transform.localEulerAngles = new Vector3(verticalRotation, horizontalRotation, 0);
        }
    }
}