using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Suit Type", menuName = "Objects/Suit", order = 0)]
    public class SuitTypeSO : ScriptableObject
    {
        public string typeName;
        public Material typeMaterial;
        public int price;
    }
}