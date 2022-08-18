using System.Collections.Generic;
using UnityEngine;

namespace Gun_Scene.Assets.Mini_First_Person_Controller.Scripts
{
    public class FirstPersonMovement : MonoBehaviour
    {
        public float speed = 5;

        [Header("Running")]
        public bool canRun = true;
        public bool IsRunning { get; private set; }
        public float runSpeed = 9;
        public KeyCode runningKey = KeyCode.LeftShift;

        private Rigidbody m_Rigidbody;
        /// <summary> Functions to override movement speed. Will use the last added override. </summary>
        public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();


        private void Awake()
        {
            // Get the rigidbody on this.
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            // Update IsRunning from input.
            IsRunning = canRun && Input.GetKey(runningKey);

            // Get targetMovingSpeed.
            float targetMovingSpeed = IsRunning ? runSpeed : speed;
            if (speedOverrides.Count > 0)
            {
                targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
            }

            // Get targetVelocity from input.
            Vector2 targetVelocity =new Vector2( Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

            // Apply movement.
            m_Rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, m_Rigidbody.velocity.y, targetVelocity.y);
        }
    }
}