using System.Collections.Generic;
using Base;
using UnityEngine;
using UnityEngine.UIElements;

namespace Character
{
    public class Stack : MonoBehaviour
    {
        [SerializeField] private Transform stackBaseAnchor;
        [SerializeField] private Transform punchAnchor;
        
        [SerializeField] private GameObject stackAnchorPrefab;
        [SerializeField] private GameObject stackNpcPrefab;
        [SerializeField] private GameObject throwNpcPrefab;
        
        [SerializeField] private int stackLimit = 5;

        [SerializeField] private float stackDistanceY = 1f;

        private int stackedNpcCount = 0;

        private CharacterMovement _characterMovement;

        private List<GameObject> _stackAnchors = new List<GameObject>();
        private List<GameObject> _stackNpcs = new List<GameObject>();

        private void Awake()
        {
            _characterMovement = GetComponent<CharacterMovement>();
        }
        
        private void Start()
        {
            InitializeStackAnchors();
        }

        private void InitializeStackAnchors()
        {
            for (var i = 0; i < stackLimit; ++i)
            {
                var newAnchorPosition = new Vector3(stackBaseAnchor.position.x ,
                    stackBaseAnchor.position.y + (stackDistanceY * i), stackBaseAnchor.position.z);
                var newAnchor = Instantiate(stackAnchorPrefab, newAnchorPosition, Quaternion.identity, stackBaseAnchor);
                _stackAnchors.Add(newAnchor);
            }
        }

        public void UpdateStackLimit(int delta)
        {
            stackLimit += delta;
            
            // Clear previous anchors
            foreach (var anchor in _stackAnchors)
            {
                Destroy(anchor);
            }
            _stackAnchors.Clear();
            
            // Reinitialize anchors
            InitializeStackAnchors();
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
            if (stackedNpcCount >= stackLimit)
            {
                return;
            }

            var stackNpc = Instantiate(stackNpcPrefab,
                _stackAnchors[stackedNpcCount].transform.position,
                Quaternion.identity,
                _stackAnchors[stackedNpcCount].transform);
            
            _stackNpcs.Add(stackNpc);
            stackNpc.transform.localPosition = Vector3.zero;
            stackNpc.transform.localRotation = Quaternion.Euler(-90f, 0f, -90f);
            stackedNpcCount++;

            CalculateSpeedChange();
        }

        private void CalculateSpeedChange()
        {
            var percentageChange = (stackedNpcCount * 1.0f / stackLimit * 1.0f) / 1.25f; // Can be reduced to up to 80%
            _characterMovement.ChangeSpeed(-percentageChange);
        }

        public void ReleaseNpc()
        {
            // Ignore if there's no NPCs on the stack
            if (stackedNpcCount <= 0)
            {
                return;
            }
            
            // Get the NPC at the top of the stack
            var topNpc = GetNpcAtIndex(stackedNpcCount - 1);
            
            // If there's no NPC return the function as a safety measure
            if (!topNpc)
            {
                return;
            }

            // Remove the object from the top
            RemoveNpcAtIndex(stackedNpcCount - 1);
            Destroy(topNpc);

            // Adds throw force to the NPC
            var newThrowNpc = Instantiate(throwNpcPrefab, punchAnchor.position + transform.forward, Quaternion.identity);
            var rigidBody = newThrowNpc.GetComponent<Rigidbody>();
            rigidBody.constraints = RigidbodyConstraints.None;
            rigidBody.velocity = Vector3.zero;
            rigidBody.AddForce((transform.forward + transform.up) * 5f, ForceMode.Impulse);

            // Decrease NPC count
            stackedNpcCount--;
        }

        public bool GetStackFull() => (stackedNpcCount >= stackLimit);
    }
}