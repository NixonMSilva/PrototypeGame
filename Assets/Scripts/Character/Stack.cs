using System;
using System.Collections.Generic;

using UnityEngine;

namespace Character
{
    public class Stack : MonoBehaviour
    {
        
        #region Properties
        
        [SerializeField] private Transform stackBaseAnchor;
        [SerializeField] private Transform punchAnchor;
        
        [SerializeField] private GameObject stackAnchorPrefab;
        [SerializeField] private GameObject stackNpcPrefab;
        [SerializeField] private GameObject throwNpcPrefab;
        
        [SerializeField] private int stackLimit = 5;

        [SerializeField] private float stackDistanceY = 1f;

        private int _stackedNpcCount = 0;

        private CharacterMovement _characterMovement;

        private List<GameObject> _stackAnchors = new List<GameObject>();
        private List<GameObject> _stackNpcs = new List<GameObject>();

        private float[] _squareRootCache = new float[1000];

        private float _inertiaOffsetHorizontal = 1f;

        private Vector3 _currentPosition;
        private Vector3 _lastPosition;

        private Vector3 _currentVelocity;
        private Vector3 _secondaryVelocity;

        private Vector3 _currentForward;
        private Vector3 _lastForward;
        
        private float _initialRotation;
        private float _targetRotation;

        private float _secondarySpeed;
        
        #endregion
        
        #region Events

        public static Action<int> OnStackChanged;
        public static Action<int> OnStackLimitChanged;
        
        #endregion
        
        #region Methods

        private void Awake()
        {
            _characterMovement = GetComponent<CharacterMovement>();
        }
        
        private void Start()
        {
            InitializeStackAnchors();
            FillSquareRootCache();
            
            _lastPosition = transform.position;
            _currentPosition = transform.position;
            _currentVelocity = Vector3.zero;
            _currentForward = transform.forward;

            _initialRotation = transform.localRotation.y;
            
            OnStackLimitChanged?.Invoke(stackLimit);
        }

        private void FillSquareRootCache()
        {
            // Caches the square root to use it on the inertia calculation
            for (var i = 0; i < _squareRootCache.Length; ++i)
            {
                _squareRootCache[i] = Mathf.Sqrt(i);
            }
        }

        private void InitializeStackAnchors()
        {
            for (var i = 0; i < stackLimit; ++i)
            {
                AddAnchor(i);
            }
        }

        public void UpdateStackLimit(int delta)
        {
            AddAnchor(stackLimit);
            CalculateSpeedChange();
            stackLimit += delta;
            OnStackLimitChanged?.Invoke(stackLimit);
        }

        private void AddAnchor(int offset)
        {
            var newAnchorPosition = new Vector3(stackBaseAnchor.position.x,
                stackBaseAnchor.position.y + (stackDistanceY * offset), stackBaseAnchor.position.z);
            var newAnchor = Instantiate(stackAnchorPrefab, newAnchorPosition, Quaternion.identity, stackBaseAnchor);
            newAnchor.transform.localPosition = new Vector3(0f, newAnchor.transform.localPosition.y, 0f);
            newAnchor.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            _stackAnchors.Add(newAnchor);
        }

        private void FixedUpdate()
        {
            _currentPosition = transform.position;
            _currentForward = transform.forward;
            
            CalculateVelocity();

            _secondaryVelocity = Vector3.Lerp(_secondaryVelocity, _currentVelocity, 7.5f * Time.fixedDeltaTime);
            
            ApplyAnchorInertia();
            
            // Inertia of the stack items towards the character's movement, items on top have greater offset
            _secondarySpeed = Mathf.Lerp(_secondarySpeed, _currentVelocity.magnitude, 7.5f * Time.fixedDeltaTime);
            
            ApplyItemHorizontalInertia();
            ApplyItemRotationalInertia();
            
            _lastPosition = _currentPosition;
            _lastForward = _currentForward;
        }

        private void CalculateVelocity()
        {
            _currentVelocity = (_currentPosition - _lastPosition) / Time.fixedDeltaTime;
        }

        private void ApplyAnchorInertia()
        {
            // Inertia of the anchor towards the character's rotation
            
            var angle = Vector3.SignedAngle(_lastForward, _currentForward, transform.up);
            
            _targetRotation = _initialRotation + angle;

            var currentRotation = stackBaseAnchor.localRotation;
            var currentYRotation = currentRotation.eulerAngles.y;
            var newYRotation = Mathf.LerpAngle(currentYRotation, _targetRotation, Time.fixedDeltaTime * 5f);

            stackBaseAnchor.localRotation = Quaternion.Euler(currentRotation.eulerAngles.x, newYRotation,
                currentRotation.eulerAngles.z);
        }

        private void ApplyItemHorizontalInertia()
        {
            // Ignore first element
            for (var i = 1; i < _stackNpcs.Count; ++i)
            {
                var a = _stackNpcs[i].transform.position;
                var b = _stackNpcs[i - 1].transform.position + (_stackNpcs[i - 1].transform.right * 0.1f * _secondarySpeed);

                Vector2 rateRange = new Vector2(0.8f,0.8f);
                float rate = Mathf.Lerp(rateRange.x, rateRange.y, (float)i / (float)_stackNpcs.Count);
                
                var newPosition = Vector3.Lerp(a, b, rate);
                newPosition.y = stackBaseAnchor.transform.position.y + (i * stackDistanceY);

                var currentTransform = _stackAnchors[i].transform;
                currentTransform.position = newPosition;
            }
        }

        private void ApplyItemRotationalInertia()
        {
            for (var i = 1; i < _stackNpcs.Count; ++i)
            {
                var currentItem = _stackAnchors[i].transform;
                var currentItemRotation = currentItem.localRotation.eulerAngles;
                var t = i * _secondarySpeed * Time.fixedDeltaTime;
                var targetAngle = Mathf.Lerp(0f, 45f, t);
                currentItem.localRotation = Quaternion.Euler(targetAngle, currentItemRotation.y, currentItemRotation.z);
            }
        }

        private GameObject GetNpcAtIndex(int index)
        {
            if (index < 0 || index >= _stackNpcs.Count)
            {
                Debug.LogWarning("Stack anchor index out of range, ignoring.");
                return null;
            }
            
            return _stackNpcs[index];
        }

        private void RemoveNpcAtIndex(int index)
        {
            if (index < 0 || index >= _stackNpcs.Count)
            {
                Debug.LogWarning("Stack anchor index out of range, ignoring.");
                return;
            }
            
            _stackNpcs.RemoveAt(index);
        }

        public void AddNpc()
        {
            if (_stackedNpcCount >= stackLimit)
            {
                return;
            }

            var stackNpc = Instantiate(stackNpcPrefab,
                _stackAnchors[_stackedNpcCount].transform.position,
                Quaternion.identity,
                _stackAnchors[_stackedNpcCount].transform);
            
            _stackNpcs.Add(stackNpc);
            stackNpc.transform.localPosition = Vector3.zero;
            stackNpc.transform.localRotation = Quaternion.Euler(-90f, 0f, -90f);
            _stackedNpcCount++;

            CalculateSpeedChange();
            
            OnStackChanged?.Invoke(_stackedNpcCount);
        }

        private void CalculateSpeedChange()
        {
            var percentageChange = (_stackedNpcCount * 1.0f / stackLimit * 1.0f) / 1.25f; // Can be reduced to up to 80%
            _characterMovement.ChangeSpeed(-percentageChange);
        }

        public void ReleaseNpc()
        {
            // Ignore if there's no NPCs on the stack
            if (_stackedNpcCount <= 0)
            {
                return;
            }
            
            // Get the NPC at the top of the stack
            var topNpc = GetNpcAtIndex(_stackedNpcCount - 1);
            
            // If there's no NPC return the function as a safety measure
            if (!topNpc)
            {
                return;
            }

            // Remove the object from the top
            RemoveNpcAtIndex(_stackedNpcCount - 1);
            Destroy(topNpc);

            // Adds throw force to the NPC
            var newThrowNpc = Instantiate(throwNpcPrefab, punchAnchor.position + transform.forward, Quaternion.identity);
            var rigidBody = newThrowNpc.GetComponent<Rigidbody>();
            rigidBody.constraints = RigidbodyConstraints.None;
            rigidBody.velocity = Vector3.zero;
            rigidBody.AddForce((transform.forward + transform.up) * 5f, ForceMode.Impulse);

            // Decrease NPC count
            _stackedNpcCount--;
            
            // Update speed accordingly
            CalculateSpeedChange();
            
            // Set thrown NPC de-spawn timeout
            Destroy(newThrowNpc.gameObject, 5f);
            
            OnStackChanged?.Invoke(_stackedNpcCount);
        }

        public bool GetStackFull() => (_stackedNpcCount >= stackLimit);
    }
    
    #endregion
}