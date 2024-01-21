#nullable enable
using Buzzrick.UnityLibs.Attributes;
using Metroidvania.Player.Inventory;
using Metroidvania.ResourceTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania.UI
{
    public class InventoryItemAmountPanel : MonoBehaviour
    {
        public ResourceTypeSO? ResourceType => Resource?.ResourceType;
        public int Amount => Resource?.ItemCount ?? 0;
        public PlayerInventoryManager.InventoryItemAmount? Resource { get; private set; }

        [SerializeField, RequiredField] private Image _resourceSprite;
        [SerializeField, RequiredField] private TMP_Text _amountText;
        [SerializeField, RequiredField] private RectTransform _rect ;

        private void Awake()
        {
            RenderItem();
        }

        public void SetResource(PlayerInventoryManager.InventoryItemAmount? resource)
        {
            Resource = resource;
            RenderItem();
        }

        public void SetPositionInGrid(float yAnchorSize, int position)
        {
            if (_rect == null)
            {
                _rect = GetComponent<RectTransform>();
            }

            float yMax = (position + 1) * yAnchorSize;
            float yMin = (position) * yAnchorSize;

            _rect.anchorMax  = new Vector2(1f, yMax);
            _rect.anchorMin = new Vector2(0f, yMin);
        }


        private void RenderItem()
        {
            if (Resource == null || Amount == 0)
            {
                _resourceSprite.gameObject.SetActive(false);
                _amountText.gameObject.SetActive(false);
            }
            else
            {
                _resourceSprite.gameObject.SetActive(true);
                _amountText.gameObject.SetActive(true);
                _resourceSprite.sprite = Resource.ResourceType.ResourceSprite;
                _amountText.text = Resource.ItemCount.ToString();
            }
        }

        private void Reset()
        {
            _resourceSprite = transform.GetComponentInChildren<Image>();
            _amountText = transform.GetComponentInChildren<TMP_Text>();
            _rect = GetComponent<RectTransform>();
        }
    }
}