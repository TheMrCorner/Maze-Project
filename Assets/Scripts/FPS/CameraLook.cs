using UnityEngine;

namespace FPS
{
    public class CameraLook : MonoBehaviour
    {
        #region Variables
        #region Public
        [Header("Configuration")]
        public Transform playerBody;
        public Transform leaner;
        public float mouseSensitivity = 100f;
        #endregion

        #region Private
        float _xRotation = 0f;
        float _mouseX, _mouseY;
        bool _leanRight, _leanLeft;
        bool _isInventory = false;

        private enum Leaning { RIGHT, LEFT };
        #endregion
        #endregion

        #region Logic
        // Start is called before the first frame update
        void Start()
        {
            ManageInventory(false);
        } // Start

        // Update is called once per frame
        void Update()
        {
            ManageInput();

            if (!_isInventory)
            {
                _mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                _mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

                _xRotation -= _mouseY;
                _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

                transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
                playerBody.Rotate(Vector3.up * _mouseX);
            } // if
        } // Update

        private void ManageInput()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                
            } // if 
            else if (Input.GetKeyDown(KeyCode.Q))
            {

            } // else if
        } // ManageInput

        private void Lean(Leaning l)
        {
            if(l == Leaning.LEFT)
            {
                _leanLeft = true;
            } // if
            else
            {
                _leanRight = true;
            } // else
        } // Lean

        public void ManageInventory(bool status)
        {
            if (status)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                _isInventory = true;
            } // if
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                _isInventory = false;
            } // else
        } // ManageInventory
        #endregion
    } // CameraLook
} // namespace
