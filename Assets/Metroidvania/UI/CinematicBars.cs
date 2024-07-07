using System;
using System.Collections;
using System.Collections.Generic;
using Metroidvania.Characters.Player;
using Metroidvania.MessageBus;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Metroidvania.UI
{
    public class CinematicBars : MonoBehaviour
    {
        [SerializeField] private RectTransform _topBar;
        [SerializeField] private RectTransform _bottomBar;
        [SerializeField] private List<RectTransform> _uiElementsToHide = new();
        [SerializeField] private Button SkipButton;

        private PlayerMovementInputLimiter _inputLimiter;
        private bool _isInputAllowed = true;
        private bool _skipCinematic;
        private MessageBusVoid _skipBus = default!;

        [Inject]
        private void Initialise(PlayerMovementInputLimiter inputLimiter,
            [Inject(Id = "Skip")] MessageBusVoid skipBus)
        {
            _inputLimiter = inputLimiter;
            _skipBus = skipBus;
            _inputLimiter.OnInputAllowedChanged += OnInputAllowedChanged;
            SetBarPercent(0f);
            SkipButton.onClick.AddListener(SkipCinematic);
        }

        private void SkipCinematic()
        {
            _skipCinematic = true;
            SetBarPercent(0f);
        }

        private void OnInputAllowedChanged(bool isAllowed)
        {
            if (_isInputAllowed != isAllowed)
            {
                _isInputAllowed = isAllowed;
                StartCoroutine(ShowBars(!isAllowed));
                
                foreach (var uiElement in _uiElementsToHide)
                {
                    uiElement.gameObject.SetActive(isAllowed);
                }
            }
        }
        
        private IEnumerator ShowBars(bool isShown)
        {
            float percent = 1f;
            while (percent > 0f)
            {
                if (_skipCinematic)
                {
                    _skipCinematic = false;
                    _skipBus.RaiseEvent();
                    //SetBarPercent(0f);
                    yield break;    
                }

                percent -= Time.deltaTime;
                if (!isShown)
                {
                    SetBarPercent(percent);
                }
                else
                {
                    SetBarPercent(1f - percent);
                }
                yield return null;
            }
        }

        private void SetBarPercent(float percent)
        {
            float amount = percent * 0.1f; 
            
            _topBar.anchorMin = new Vector2(0, 1f - amount);
            //_topBar.anchorMax = new Vector2(1, 1f);
            
            _bottomBar.anchorMax = new Vector2(1, amount);

            SkipButton.gameObject.SetActive(percent >= 0.1f);
        }
    }
}