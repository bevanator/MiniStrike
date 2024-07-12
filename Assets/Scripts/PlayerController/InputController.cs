using System;
using ProjectC;
using UnityEngine;

namespace PlayerController
{
    public class InputController : MonoBehaviour
    {

        private bool _isActive = true;
        [SerializeField] private Joystick m_AimJoystick;
        [SerializeField] private Joystick m_MoveJoystick;
        [SerializeField] private bool m_UseKeyBoardForMovement = false;
        
        private static InputController _instance;
        private Camera _camera;
        public static event Action<bool, float> AimStateChangeEvent;
        public static event Action<bool> MoveStateChange;
        public static bool IsAimDown { get; private set; }
        public static Vector2 MoveDirection { get; private set; }
        public static Vector2 AimDirection { get; private set; }

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Awake()
        {
            _instance = this;
        }

        private void OnEnable()
        {
            ((FixedJoystick)m_AimJoystick).PointerChange += PointerChange;
            ((FixedJoystick)m_MoveJoystick).PointerChange += MovePointerChange;
        }

        private void OnDisable()
        {
            ((FixedJoystick)m_AimJoystick).PointerChange -= PointerChange;
            ((FixedJoystick)m_MoveJoystick).PointerChange -= MovePointerChange;
        }

        private void PointerChange(bool state)
        {
            IsAimDown = state;
            if(!_isActive) return;
            AimStateChangeEvent?.Invoke(state, AimDirection.magnitude);
        }

        private void MovePointerChange(bool state)
        {
            if(!_isActive) return;
            MoveStateChange?.Invoke(state);
        }

        private void Update()
        {
            Vector2 playerPosOnScreen = _camera.WorldToScreenPoint(Player.Instance.transform.position);
            if(!_isActive) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_UseKeyBoardForMovement = !m_UseKeyBoardForMovement;
            }
            Vector2 keyboardMovement = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            // AimDirection = m_AimJoystick.Direction;
            AimDirection = (playerPosOnScreen - (Vector2)Input.mousePosition).normalized * -1f;
            print(AimDirection);
            MoveDirection = m_UseKeyBoardForMovement ? keyboardMovement : m_MoveJoystick.Direction; 
            
            
            // print((playerPosOnScreen - (Vector2)Input.mousePosition).normalized);
        }

        public static void DisableInput() => _instance._isActive = false;
    }
}