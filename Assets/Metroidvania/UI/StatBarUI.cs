using Buzzrick.UnityLibs.Attributes;
using UnityEngine;


namespace Metroidvania.UI
{
    public class StatBarUI : MonoBehaviour
    {
        [SerializeField, RequiredField] private RectTransform _bar;

        public void SetValue(float percent)
        {
            _bar.anchorMax = new Vector2(percent, 1f);
        }

        public void SetValue(float currentValue, float maxValue)
        {
            SetValue(currentValue / maxValue);
        }
    }
}