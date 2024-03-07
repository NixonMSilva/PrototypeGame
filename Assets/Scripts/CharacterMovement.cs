using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework.Internal;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    
    #region Properties
    
    [SerializeField] private float speed = 10f;

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
    }

    private void Update()
    {
        CaptureInput();
        CalculateMovement();
    }

    private void FixedUpdate()
    {
        // Moves the rigid body to new position
        _rigidbody.Move(transform.position + _movement, Quaternion.identity);
        
        // Spherical linear interpolation to smoothly transition to new rotation angle
        transform.rotation = Quaternion.Slerp(transform.rotation,_targetRotation,10f * Time.fixedDeltaTime);
    }
    
    private void CaptureInput ()
    {
        var inputX = Input.GetAxis("Horizontal");
        var inputZ = Input.GetAxis("Vertical");

        _movement = new Vector3(inputX, 0f, inputZ);
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
        _movement = _movement.normalized * (speed * Time.deltaTime);
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
    
    #endregion
    
}
