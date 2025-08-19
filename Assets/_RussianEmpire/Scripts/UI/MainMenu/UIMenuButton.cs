using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

namespace Railway.UI.Menu
{
    public class UIMenuButton : MonoBehaviour
    {
        [SerializeField] private GameObject _imageGO;
        [SerializeField] private Color _textColor;

        private Color defaultColor;
        private Image _image;
        
        private PointerObserver _pointerObserver;

        private void OnEnable()
        {
            _pointerObserver = GetComponent<PointerObserver>();
            
            _pointerObserver.PointerEntered += OnPointerEnter;
            _pointerObserver.PointerExited += OnPointerExit;
            
            defaultColor = GetComponentInChildren<TMP_Text>().color;
            
            _image = _imageGO.GetComponent<Image>();
        }

        private void OnPointerEnter(PointerEventData eventData)
        {
            _imageGO.SetActive(true);
            GetComponentInChildren<TMP_Text>().DOColor(_textColor, .5f);
        }

        private void OnPointerExit(PointerEventData eventData)
        {
            _imageGO.SetActive(false);
            GetComponentInChildren<TMP_Text>().DOColor(defaultColor, .5f);
        }
    }
}