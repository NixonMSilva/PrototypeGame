using System;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace UI
{
    public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        
        #region Properties
        
        [SerializeField] private GameObject _joystickOuter;
        [SerializeField] private GameObject _joystickInner;

        private float _backgroundImageSizeX, _backgroundImageSizeY;

        private float _offsetFactorWithBgSize = 0.5f;
        
        public static event Action<Vector2> OnJoystickMoved;

        private RectTransform _outerRectTransform;
        private RectTransform _innerRectTransform;

        public Vector2 InputDirection { set; get; }
        
        #endregion
        
        #region Methods

        private void Start()
        {
            _outerRectTransform = _joystickOuter.GetComponent<RectTransform>();
            _innerRectTransform = _joystickInner.GetComponent<RectTransform>();

            _backgroundImageSizeX = _outerRectTransform.sizeDelta.x;
            _backgroundImageSizeY = _outerRectTransform.sizeDelta.y;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 tappedOint;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _outerRectTransform, 
                    eventData.position, 
                    eventData.pressEventCamera, 
                    out tappedOint)) 
                return;
            
            tappedOint.x = (tappedOint.x / (_backgroundImageSizeX * _offsetFactorWithBgSize));
            tappedOint.y = (tappedOint.y / (_backgroundImageSizeY * _offsetFactorWithBgSize));

            InputDirection = new Vector3(tappedOint.x, tappedOint.y);

            InputDirection = InputDirection.magnitude > 1 ? InputDirection.normalized : InputDirection;

            var sizeDelta = _outerRectTransform.sizeDelta;
            _innerRectTransform.anchoredPosition =
                new Vector3(InputDirection.x * (sizeDelta.x * _offsetFactorWithBgSize),
                    InputDirection.y * (sizeDelta.y * _offsetFactorWithBgSize));
            
            OnJoystickMoved?.Invoke(InputDirection);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            InputDirection = Vector2.zero;
            _innerRectTransform.anchoredPosition = Vector3.zero;

            OnJoystickMoved?.Invoke(InputDirection);
        }
        
        #endregion
    }
}
