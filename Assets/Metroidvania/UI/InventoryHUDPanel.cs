#nullable enable
using Buzzrick.UnityLibs.Attributes;
using Metroidvania.Player.Inventory;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;

namespace Metroidvania.UI
{
    public class InventoryHUDPanel : MonoBehaviour
    {
        private PlayerInventoryManager _playerInventoryManager;
        /// <summary>
        /// How many of the most recent inventory items should we display.
        /// </summary>
        public int InventoryDisplayCount = 3;

        [SerializeField, RequiredField] InventoryItemAmountPanel _inventoryItemAmountPanelTemplate = default!;
        [SerializeField, RequiredField] InventoryItemAmountPanel[] _inventoryItemAmountPanels = default;

        private PlayerInventoryManager.InventoryItemAmount[] _inventoryItemAmounts = default!;
        private List<PlayerInventoryManager.InventoryItemAmount> _inventoryItemList = new();


        [Inject]
        private void Initialise(PlayerInventoryManager playerInventoryManager)
        {
            _playerInventoryManager = playerInventoryManager;
        }

        private void Awake()
        {
            _inventoryItemAmounts = new PlayerInventoryManager.InventoryItemAmount[InventoryDisplayCount];
            _inventoryItemAmountPanels = new InventoryItemAmountPanel[InventoryDisplayCount];

            float yAnchorSize = 1f / InventoryDisplayCount;
            _inventoryItemAmountPanelTemplate.SetPositionInGrid(yAnchorSize, InventoryDisplayCount - 1);
            _inventoryItemAmountPanels[0] = _inventoryItemAmountPanelTemplate;

            for (int i = 1; i < InventoryDisplayCount; i++)
            {
                var nextPanel = GameObject.Instantiate(_inventoryItemAmountPanelTemplate, Vector3.zero, Quaternion.identity);
                nextPanel.transform.SetParent(transform, false);
                nextPanel.SetPositionInGrid(yAnchorSize, InventoryDisplayCount - i - 1);
                _inventoryItemAmountPanels[i] = nextPanel;
            }
        }

        private void OnEnable()
        {
            _playerInventoryManager.OnInventoryAmountChanged += Handle_OnInventoryAmountChanged; 
            _playerInventoryManager.OnInventoryReset += Handle_OnInventoryReset;
        }


        private void OnDisable()
        {
            _playerInventoryManager.OnInventoryAmountChanged -= Handle_OnInventoryAmountChanged;
            _playerInventoryManager.OnInventoryReset -= Handle_OnInventoryReset;
        }

        private void Handle_OnInventoryReset()
        {
            _inventoryItemList.Clear();
            RedrawItems();
        }

        private void Handle_OnInventoryAmountChanged(PlayerInventoryManager.InventoryItemAmount amount)
        {
            //  pull any existing item of this type out of the list.
            PlayerInventoryManager.InventoryItemAmount? removeItem = null;
            foreach (var item in _inventoryItemList)
            {
                if (item.ResourceType == amount.ResourceType)
                {
                    _inventoryItemList.Remove(item);
                    break;
                }
            }
            //  add the updated item at the top of the list
            _inventoryItemList.Insert(0, amount);

            RedrawItems();
        }

        private void RedrawItems()
        {
            for (int i = 0; i < _inventoryItemAmountPanels.Length; i++)
            {
                if (_inventoryItemList.Count > i)
                {
                    _inventoryItemAmountPanels[i].SetResource(_inventoryItemList[i]);
                }
                else
                {
                    _inventoryItemAmountPanels[i].SetResource(null);
                }
            }
        }
    }
}