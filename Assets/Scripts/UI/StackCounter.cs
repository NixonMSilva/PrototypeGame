using Character;
using TMPro;
using UnityEngine;

namespace UI
{
    public class StackCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI stackCurrentCount;
        [SerializeField] private TextMeshProUGUI stackLimitCount;

        private void OnEnable()
        {
            Stack.OnStackChanged += ChangeCurrentCount;
            Stack.OnStackLimitChanged += ChangeLimitCount;
        }

        private void OnDisable()
        {
            Stack.OnStackChanged -= ChangeCurrentCount;
            Stack.OnStackLimitChanged -= ChangeLimitCount;
        }

        private void ChangeCurrentCount(int value)
        {
            stackCurrentCount.text = value.ToString();
        }

        private void ChangeLimitCount(int value)
        {
            stackLimitCount.text = value.ToString();
        }
    }
}