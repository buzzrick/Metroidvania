using Metroidvania.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Metroidvania.UI
{
    public class UIViewButton : MonoBehaviour, IPointerClickHandler
    {
        public UIView _uiView;
        public string MessageID;

        public void OnPointerClick(PointerEventData eventData)
        {
            _uiView.TriggerMessage(MessageID);
        }
    }
}