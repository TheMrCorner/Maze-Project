using UnityEngine;

namespace FPS
{
    public class PlayerMovement : MonoBehaviour
    {
        #region  Variables
        #region Public
        [Header("Basic Information, Configuration")]
        public CharacterController ctr;
        public CameraLook cl;
        public float speed = 12f;
        // Sounds etc..

        [Header("Jumping and Gravity Configuration")]
        public Transform groundCheck;
        public LayerMask groundMask;
        public float gravity = -9.81f;
        public float groundDistance = 0.4f;
        public float jumpHeight = 3f;
        #endregion

        #region Private
        private float _x, _z;
        private bool _isGrounded;
        private bool _wasGrounded;
        private bool _isInInventory;
        private Vector3 _move;
        private Vector3 _vel;
        #endregion
        #endregion

        #region Logic
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if(_isGrounded && _vel.y < 0)
            {
                _vel.y = -2f;
            } // if


            if (!_isInInventory)
            {
                _x = Input.GetAxis("Horizontal");
                _z = Input.GetAxis("Vertical");

                _move = transform.right * _x + transform.forward * _z;
                ctr.Move(_move * speed * Time.deltaTime);

                if (Input.GetButtonDown("Jump") && _isGrounded)
                {
                    _vel.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                } // if
            }

            _vel.y += gravity * Time.deltaTime;

            ctr.Move(_vel * Time.deltaTime);

            _wasGrounded = _isGrounded;

            if (Input.GetKeyDown(KeyCode.E))
            {
            } // GetButtonDown
        } // Update

        public void SetInventoryUI(bool state)
        {
            Debug.Log("The Inventory has changed");
            _isInInventory = state;
            cl.ManageInventory(state);
        } // SetInventoryUI
        #endregion
    } // PlayerMovement
} // namespace
