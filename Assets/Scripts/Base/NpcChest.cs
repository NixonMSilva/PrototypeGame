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
                var npcStatus = other.gameObject.GetComponent<NpcStatus>();
                if (npcStatus.HasBeenProcessed)
                {
                    // Prevents double registration of score for an NPC
                    return;
                }

                npcStatus.HasBeenProcessed = true;
                OnNpcEnter?.Invoke(npcStatus.npcValue);
                
                // Set NPC de-spawn timeout
                Destroy(other.gameObject, 5f);
            }
        }
    }
}
