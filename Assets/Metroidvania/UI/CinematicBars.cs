using System.Collections;
using Metroidvania.Characters.Player;
using UnityEngine;
using Zenject;

namespace Metroidvania.UI
{
    public class CinematicBars : MonoBehaviour
    {
        [SerializeField] private RectTransform _topBar;
        [SerializeField] private RectTransform _bottomBar;
        private PlayerMovementInputLimiter _inputLimiter;
        private bool _isInputAllowed = true;
        
        [Inject]
        private void Initialise(PlayerMovementInputLimiter inputLimiter)
        {
            _inputLimiter = inputLimiter;
            _inputLimiter.OnInputAllowedChanged += OnInputAllowedChanged;
            SetBarPercent(0f);
        }
        
        private void OnInputAllowedChanged(bool isAllowed)
        {
            if (_isInputAllowed != isAllowed)
            {
                _isInputAllowed = isAllowed;
                StartCoroutine(ShowBars(!isAllowed));
            }
        }
        
        private IEnumerator ShowBars(bool isShown)
        {
            float percent = 1f;
            while (percent > 0f)
            {
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
        }
    }
}