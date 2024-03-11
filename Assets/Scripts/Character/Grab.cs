using System;
using Base;
using UnityEngine;

namespace Character
{
    public class Grab : MonoBehaviour
    {
        [SerializeField] private Transform grabOrigin;
        [SerializeField] private LayerMask npcMask;

        private Stack _stack;

        public static Action OnNpcGrabbed;

        private void Awake()
        {
            _stack = GetComponent<Stack>();
        }

        public void GrabNpc()
        {
            var downedNpcCandidate = SearchForDownedNpc();
            if (downedNpcCandidate == null)
            {
                return;
            }
            
            _stack.AddNpc();
            OnNpcGrabbed?.Invoke();
            Destroy(downedNpcCandidate);
        }

        private GameObject SearchForDownedNpc()
        {
            var results = Physics.OverlapSphere(grabOrigin.position, 1.5f, npcMask);
            
            GameObject returnCandidate = null;
            var returnCandidateDistance = Mathf.Infinity;

            foreach (var result in results)
            {
                var parent = result.gameObject.transform.parent;

                if (!parent)
                {
                    continue;
                }
                
                if (parent.gameObject.GetComponent<NpcStatus>().isDowned != true)
                {
                    continue;
                }

                var distance = Vector3.Distance(transform.position, parent.gameObject.transform.position);
                if (!(distance < returnCandidateDistance))
                {
                    continue;
                }
                returnCandidate = parent.gameObject;
                returnCandidateDistance = distance;
            }

            // Return the closest object to the player
            Debug.Log(returnCandidate);
            return returnCandidate;
        }
    }
}