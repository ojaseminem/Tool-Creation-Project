using UnityEngine;

namespace Controllers.CharacterController
{
    [RequireComponent(typeof(UnityEngine.CharacterController))]

    public class ScriptCharacter : MonoBehaviour
    {
        public float speed = 7.5f;
        public float jumpSpeed = 8.0f;
        public float gravity = 20.0f;
        public Camera fpsCamera;
        public float lookSpeed = 2.0f;
        public float lookXLimit = 45.0f;
        public Transform groundCheck;
        public float checkRadius;
        public float characterHp = 100f;

        UnityEngine.CharacterController characterController;
        Vector3 moveDirection = Vector3.zero;
        Vector2 rotation = Vector2.zero;

        [HideInInspector]
        public bool canMove = true;
        private bool isGrounded;

        private void Start()
        {
            characterController = GetComponent<UnityEngine.CharacterController>();
            rotation.y = transform.eulerAngles.y;
        }

        private void Update()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius);
            if (characterController.isGrounded)
            {
                Vector3 forward = transform.TransformDirection(Vector3.forward);
                Vector3 right = transform.TransformDirection(Vector3.right);
                float curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0;
                float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0;
                moveDirection = (forward * curSpeedX) + (right * curSpeedY);


                if (Input.GetButton("Jump") && canMove)
                {
                    moveDirection.y = jumpSpeed;
                }
            }

            moveDirection.y -= gravity * Time.deltaTime;

            characterController.Move(moveDirection * Time.deltaTime);

            if (canMove)
            {
                rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
                rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
                rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
                fpsCamera.transform.localRotation = Quaternion.Euler(rotation.x, 0, 0);
                transform.eulerAngles = new Vector2(0, rotation.y);
            }
        }
        
        public void LoseHealth(float losehp)
        {
            characterHp -= losehp;
            if(characterHp <= 0)
            {
                Debug.Log("Dead");
            }
        }
    }
}
