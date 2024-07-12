using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PlayerController
{
    public class PlayerMovement : MonoBehaviour
    {
        private float _speed;

        private AnimationHandler _animationHandler;
        private CharacterController _characterController;
        private float _verticalVelocity;
        private float _relativeAngle;
        private Vector3 _inputDirection;
        private Camera _camera;

        private Vector3 _velocity;

        private bool _isMoving;

        private float _aimAngle;
        private bool _isAiming;

        [SerializeField] private float m_MaxMovementSpeed = 2.0f;

        private Rigidbody _rigidbody;
        private Vector3 _direction;
        [SerializeField] private bool m_UseCharacterController;
        private bool _isMovePointing;


        protected void Awake()
        {
            _camera = Camera.main;
            _animationHandler = GetComponent<AnimationHandler>();
            _characterController = GetComponent<CharacterController>();
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        private void OnEnable()
        {
            InputController.AimStateChangeEvent += OnAimStateChangeEvent;
            InputController.MoveStateChange += OnMoveStateChangeEvent;
        }

        private void OnDisable()
        {
            InputController.AimStateChangeEvent -= OnAimStateChangeEvent;
            InputController.MoveStateChange -= OnMoveStateChangeEvent;
        }

        private void OnMoveStateChangeEvent(bool state) => _isMovePointing = state;


        private void OnAimStateChangeEvent(bool state, float mag = 0) => _isAiming = state;
        

        private void FixedUpdate()
        {
            ProcessInputDirection();
            Move();
            Rotate();
        }

  

        private void HandleAnimation(Vector2 lhs, Vector2 rhs)
        {
            float blendX = Vector2.Dot(lhs, rhs);
            float blendY = Vector3.Cross(lhs, rhs).z;
            _animationHandler.UpdateLocomotionBlends(blendX, blendY);
        }

        private void Rotate()
        {
            if (_isMovePointing)
            {
                transform.rotation = Quaternion.Euler(0f, _relativeAngle, 0f);
                HandleAnimation(InputController.MoveDirection, InputController.MoveDirection);
            }
            // if (!_isAiming) return;
            transform.rotation = Quaternion.Euler(0f, _aimAngle, 0f);
            HandleAnimation(InputController.MoveDirection, InputController.AimDirection);
        }


        private void ProcessInputDirection()
        {
            _relativeAngle = Mathf.Atan2(InputController.MoveDirection.x,
                InputController.MoveDirection.y) * Mathf.Rad2Deg + _camera.transform.eulerAngles.y;
            _aimAngle = Mathf.Atan2(InputController.AimDirection.x,
                InputController.AimDirection.y) * Mathf.Rad2Deg + _camera.transform.eulerAngles.y;
            _inputDirection = Quaternion.Euler(0f, _relativeAngle, 0f) * Vector3.forward;
            float x = InputController.MoveDirection.x;
            float y = InputController.MoveDirection.y;
            _direction = new Vector3(x, 0, y).normalized;
        }
        private void Move()
        {
            if (InputController.MoveDirection.magnitude > 0.1f)
            {
                if (!_isMoving)
                {
                    _isMoving = true;
                    _animationHandler.SetRunningState(true);
                }
            }
            
            if (!(InputController.MoveDirection.magnitude > 0.1f))
            {
                if (_isMoving)
                {
                    _isMoving = false; 
                    _animationHandler.SetRunningState(false);
                }
                return;
            }
            if (_isMoving)
            {
                if (m_UseCharacterController)
                {
                    _speed = m_MaxMovementSpeed * Time.deltaTime;
                    _characterController.Move(_speed * _inputDirection.normalized);
                    Vector3 newPosition = transform.position;
                    newPosition.y = Mathf.Clamp(newPosition.y, 0f, 0.01f); 
                    _characterController.Move(newPosition - transform.position);
                }
                else _rigidbody.MovePosition(transform.position + _direction.normalized * (m_MaxMovementSpeed * Time.deltaTime));
            }
        }

    }
    
}