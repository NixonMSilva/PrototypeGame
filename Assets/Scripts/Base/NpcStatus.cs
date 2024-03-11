using System;
using UnityEngine;

namespace Base
{
    public class NpcStatus : MonoBehaviour
    {
        public bool HasBeenProcessed { get; set; } = false;

        [SerializeField] public int npcValue = 100;

        [SerializeField] public bool isDowned = false;
    }
}
