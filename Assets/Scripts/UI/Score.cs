using UnityEngine;

using Base;
using TMPro;

namespace UI
{
    public class Score : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreObject;

        [SerializeField] private int valuePerNpc = 100;
        
        private int _scoreValue;

        private void OnEnable()
        {
            NpcChest.OnNpcEnter += AddScore;
        }

        private void OnDisable()
        {
            NpcChest.OnNpcEnter -= AddScore;
        }

        private void AddScore (int delta)
        {
            _scoreValue += delta;
            scoreObject.text = _scoreValue.ToString();
        }
    }
}