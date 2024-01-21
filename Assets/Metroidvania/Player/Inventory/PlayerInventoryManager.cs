using Metroidvania.ResourceTypes;
using System;
using System.Collections.Generic;
using Zenject;

namespace Metroidvania.Player.Inventory
{

    public class PlayerInventoryManager 
    {
        public List<InventoryItemAmount> InventoryList = new();
        private ResourceTypeDB _resourceTypeDB;

        public event Action<InventoryItemAmount> OnInventoryAmountChanged;

        [Inject]
        private void Initialise(ResourceTypeDB resourceTypeDB)
        {
            _resourceTypeDB = resourceTypeDB;
        }



        public int GetInventoryCount(string resourceTypeID)
        {
            return GetOrCreateInventoryItemAmount(resourceTypeID).ItemCount;
        }

        private InventoryItemAmount GetOrCreateInventoryItemAmount(string resourceTypeID)
        {
            foreach (InventoryItemAmount item in InventoryList)
            {
                if (item.ResourceTypeID == resourceTypeID)
                {
                    return item;
                }
            }
            var newItem = new InventoryItemAmount
            {
                ResourceTypeID = resourceTypeID,
                ResourceType = _resourceTypeDB.GetResourceType(resourceTypeID),
                ItemCount = 0
            };
            InventoryList.Add(newItem);
            return newItem;
        }


        private InventoryItemAmount GetOrCreateInventoryItemAmount(ResourceTypeSO resourceType)
        {
            foreach (InventoryItemAmount item in InventoryList)
            {
                if (item.ResourceType == resourceType)
                {
                    return item;
                }
            }
            var newItem = new InventoryItemAmount
            {
                ResourceTypeID = resourceType.name,
                ResourceType = resourceType,
                ItemCount = 0
            };
            InventoryList.Add(newItem);
            return newItem;
        }

        public void IncrementInventory(ResourceTypeSO resourceType, int amount)
        {
            InventoryItemAmount inventoryItem = GetOrCreateInventoryItemAmount(resourceType);
            inventoryItem.ItemCount += amount;
            OnInventoryAmountChanged?.Invoke(inventoryItem);
        }

        public void IncrementInventory(string resourceTypeID, int amount)
        {
            InventoryItemAmount inventoryItem = GetOrCreateInventoryItemAmount(resourceTypeID);
            inventoryItem.ItemCount += amount;
            OnInventoryAmountChanged?.Invoke(inventoryItem);
        }


        public class InventoryItemAmount
        {
            public string ResourceTypeID;
            public ResourceTypeSO ResourceType;
            public int ItemCount;
        }
    }



}