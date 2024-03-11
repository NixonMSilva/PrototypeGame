using System;
using UnityEngine;

using Character;
using ScriptableObjects;
using UI;

namespace Base
{
    public class Shop : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer playerSuitA;
        [SerializeField] private SkinnedMeshRenderer playerSuitB;

        [SerializeField] private Stack stackReference;
        [SerializeField] private int stackUpgradeCost = 300;

        [SerializeField] private CanvasGroup shopPanel;

        [SerializeField] private Score scoreReference;
        
        private SuitTypeSO _currentSuit = null;

        public static Action<int> OnPurchase;

        public void PurchaseSuit(SuitTypeSO suit)
        {
            if (!scoreReference.HasEnoughScore(suit.price) || _currentSuit == suit)
            {
                return;
            }
            playerSuitA.sharedMaterial = suit.typeMaterial;
            playerSuitB.sharedMaterial = suit.typeMaterial;
            _currentSuit = suit;
            OnPurchase?.Invoke(-suit.price);
        }

        public void PurchaseStackUpgrade()
        {
            if (!scoreReference.HasEnoughScore(stackUpgradeCost))
            {
                return;
            }
            stackReference.UpdateStackLimit(1);
            OnPurchase?.Invoke(-stackUpgradeCost);
        }

        public void ShowShop()
        {
            shopPanel.alpha = 1f;
        }

        public void HideShop()
        {
            shopPanel.alpha = 0f;
        }
    }
}
