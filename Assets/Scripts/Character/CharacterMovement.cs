using System;
using UnityEngine;
using UI;

namespace Character
{
    public class CharacterMovement : MonoBehaviour
    {
    
        #region Properties
    
        [SerializeField] private float speed = 10f;

        private float originalSpeed;

        private Animator _animator;
        private Rigidbody _rigidbody;

        private Vector3 _movement;
        private Vector3 _lastDirection;
        private Quaternion _targetRotation;
    
        private static readonly int IsMoving = Animator.StringToHash("isMoving");
    
        #endregion
    
        #region Methods

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();

            originalSpeed = speed;
        }

        private void OnEnable()
        {
            Joystick.OnJoystickMoved += CaptureInputMobile;
        }

        private void OnDisable()
        {
            Joystick.OnJoystickMoved -= CaptureInputMobile;
        }

        private void Update()
        {
            // CaptureInputKeyboard();
            //
        }

        private void FixedUpdate()
        {
            CalculateMovement();
            
            // Moves the rigid body to new position
            _rigidbody.Move(transform.position + _movement, Quaternion.identity);
        
            // Spherical linear interpolation to smoothly transition to new rotation angle
            transform.rotation = Quaternion.Slerp(transform.rotation,_targetRotation,10f * Time.fixedDeltaTime);
        }
    
        // OBSOLETE
        private void CaptureInputKeyboard ()
        {
            var inputX = Input.GetAxis("Horizontal");
            var inputZ = Input.GetAxis("Vertical");

            _movement = new Vector3(inputX, 0f, inputZ);
        }

        private void CaptureInputMobile(Vector2 mobileJoystickInput)
        {
            _movement = new Vector3(mobileJoystickInput.x, 0f, mobileJoystickInput.y);
        }

        private void CalculateMovement()
        {
            if (Mathf.Abs(_movement.magnitude) <= Mathf.Epsilon)
            {
                ForceStop();
                _animator.SetBool(IsMoving, false);
                return;
            }
        
            PerformMovement();
            CalculateTargetRotation();
        }

        private void PerformMovement()
        {
            _lastDirection = _movement.normalized;
            _movement = _movement.normalized * (speed * Time.fixedDeltaTime);
            _animator.SetBool(IsMoving, true);
        }

        private void ForceStop()
        {
            // Zeroes the values to avoid erratic behavior with the character
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _movement = Vector3.zero;
        }

        private void CalculateTargetRotation()
        {
            _targetRotation = Quaternion.LookRotation(_lastDirection);
        }

        public void ChangeSpeed(float percentageChange)
        {
            speed = originalSpeed * (1f + percentageChange);
        }
    
        #endregion
    
    }
}
