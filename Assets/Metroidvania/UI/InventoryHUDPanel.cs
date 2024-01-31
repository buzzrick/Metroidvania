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
            _playerInventoryManager.OnInventoryAmountChanged += Handle_OnInventoryAmountChanged; ;

        }
        private void OnDisable()
        {
            _playerInventoryManager.OnInventoryAmountChanged -= Handle_OnInventoryAmountChanged;
        }


        private void Handle_OnInventoryAmountChanged(PlayerInventoryManager.InventoryItemAmount amount)
        {
            //  TODO:  this reshuffling doesn't correctly put the newest item at the top of the list.
            //   And it's a bit janky and obtuse.

            // In the future I recomment just having an array,
            //  -   walking down the list until you find the same resource (or the end of the list)
            //  -   and then shuffling everything down every item above that point
            //  -   finally insert the new item at the top of the list
            //
            //  pull any existing item of this type out of the list.
            PlayerInventoryManager.InventoryItemAmount? removeItem = null;
            foreach (var item in _inventoryItemList)
            {
                if (item.ResourceType == amount.ResourceType)
                {
                    removeItem = item;
                    _inventoryItemList.Remove(item);
                    break;
                }
            }

            //_inventoryItemList.Insert(_inventoryItemList.Count, amount);
            _inventoryItemList.Insert(0, amount);

            for (int i = 0; i < _inventoryItemList.Count; i++)
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