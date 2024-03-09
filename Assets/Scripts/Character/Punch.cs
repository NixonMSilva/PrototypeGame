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

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _stack = GetComponent<Stack>();
        }
        
        public void PerformPunch()
        {
            var results = new RaycastHit[1];
            Debug.DrawRay(punchOrigin.position, transform.forward * 1.25f, Color.magenta, 1f);
            if (Physics.RaycastNonAlloc(punchOrigin.position, transform.forward, results, 1.25f, npcMask) == 0)
            {
                return;
            }
            else
            {
                if (_stack.GetStackFull())
                    return;
                
                Destroy(results[0].collider.gameObject);
                _stack.AddNpc();
                OnNpcPunched?.Invoke();
            }
            
            
        }
    }
}