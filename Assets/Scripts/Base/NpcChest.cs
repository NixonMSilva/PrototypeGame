using System;
using UnityEngine;

namespace Base
{
    public class NpcChest : MonoBehaviour
    {
        public static Action<int> OnNpcEnter;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("ThrownNPC"))
            {
                var npcStatus = other.gameObject.transform.parent.GetComponent<NpcStatus>();
                if (npcStatus.HasBeenProcessed)
                {
                    // Prevents double registration of score for an NPC
                    return;
                }

                npcStatus.HasBeenProcessed = true;
                OnNpcEnter?.Invoke(npcStatus.npcValue);
            }
        }
    }
}
