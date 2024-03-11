using System;
using Base;
using UnityEngine;

namespace Character
{
    public class Punch : MonoBehaviour
    {
        
        #region Properties
        
        [SerializeField] private Transform punchOrigin;

        [SerializeField] private LayerMask npcMask;

        private Animator _animator;
        private Stack _stack;

        public static Action OnNpcPunched;
        
        private static readonly int PunchTrigger = Animator.StringToHash("punch");
        
        #endregion
        
        #region Method

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _stack = GetComponent<Stack>();
        }

        public void StartPunch()
        {
            _animator.SetTrigger(PunchTrigger);
        }
        
        public void PerformPunch()
        {
            var results = new Collider[1];
            var size = Physics.OverlapSphereNonAlloc(punchOrigin.position, 1f, results, npcMask);

            if (size <= 0)
            {
                return;
            }

            if (_stack.GetStackFull())
                return;

            // Enable ragdolling and update status
            results[0].transform.parent.GetComponent<Animator>().enabled = false;
            results[0].transform.parent.GetComponent<NpcStatus>().isDowned = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(punchOrigin.position, 1f);
        }
        
        #endregion
    }
}