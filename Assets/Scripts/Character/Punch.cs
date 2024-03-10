using System;
using UnityEngine;

namespace Character
{
    public class Punch : MonoBehaviour
    {
        [SerializeField] private Transform punchOrigin;

        [SerializeField] private LayerMask npcMask;

        private Animator _animator;
        private Stack _stack;

        public static Action OnNpcPunched;

        private Vector3 _punchBox = new Vector3(1.5f, 1.5f, 1.5f);

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _stack = GetComponent<Stack>();
        }
        
        public void PerformPunch()
        {
            var results = new Collider[1];
            var size = Physics.OverlapSphereNonAlloc(punchOrigin.position, 1f, results, npcMask);

            if (size <= 0)
            {
                return;
            }
            else
            {
                if (_stack.GetStackFull())
                    return;
                
                Destroy(results[0].gameObject);
                _stack.AddNpc();
                OnNpcPunched?.Invoke();
            }
            
            
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(punchOrigin.position, 1f);
        }
    }
}