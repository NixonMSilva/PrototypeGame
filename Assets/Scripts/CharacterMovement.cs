using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float movementThreshold = 0.05f;

    private Animator _animator;
    private CharacterController _characterController;
    private Rigidbody _rigidbody;

    private Vector3 _movement;

    private Vector3 _lastPosition;
    private static readonly int Speed = Animator.StringToHash("speed");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    private void Start()
    {
        _lastPosition = transform.position;
    }

    private void Update()
    {
        CalculateMovement();
    }

    private void FixedUpdate()
    {
        _rigidbody.Move(transform.position + _movement, Quaternion.identity);
    }

    private void CalculateMovement()
    {
        var inputX = Input.GetAxis("Horizontal");
        var inputZ = Input.GetAxis("Vertical");

        if (Mathf.Abs(inputX) <= Mathf.Epsilon && Mathf.Abs(inputZ) <= Mathf.Epsilon)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _movement = Vector3.zero;
            _animator.SetBool(IsMoving, false);
            return;
        }
        
        _movement = new Vector3(inputX, 0f, inputZ).normalized * (speed * Time.deltaTime);
        _animator.SetBool(IsMoving, true);
    }

    private float CalculateSpeed (Vector3 currentPosition)
    {
        var currentSpeed = (currentPosition - _lastPosition).magnitude / Time.deltaTime;
        _lastPosition = currentPosition;
        return currentSpeed;
    }
}
