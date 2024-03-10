using UnityEngine;

using Base;
using TMPro;

namespace UI
{
    public class Score : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreObject;

        private int _scoreValue;

        private void OnEnable()
        {
            NpcChest.OnNpcEnter += AddScore;
            Shop.OnPurchase += AddScore;
        }

        private void OnDisable()
        {
            NpcChest.OnNpcEnter -= AddScore;
            Shop.OnPurchase -= AddScore;
        }

        private void AddScore (int delta)
        {
            _scoreValue += delta;
            scoreObject.text = _scoreValue.ToString();
        }
        
        public bool HasEnoughScore (int scoreToCheck) => scoreToCheck <= _scoreValue;

    }
}