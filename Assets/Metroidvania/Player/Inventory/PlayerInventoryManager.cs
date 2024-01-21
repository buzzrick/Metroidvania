using Metroidvania.ResourceTypes;
using System;
using System.Collections.Generic;
using Zenject;

namespace Metroidvania.Player.Inventory
{

    public class PlayerInventoryManager 
    {
        public List<InventoryItem> InventoryList = new();
        private ResourceTypeDB _resourceTypeDB;

        [Inject]
        private void Initialise(ResourceTypeDB resourceTypeDB)
        {
            _resourceTypeDB = resourceTypeDB;
        }
        
        public class InventoryItem
        {
            public string ResourceTypeID;
            public ResourceTypeSO ResourceType;
            public int ItemCount;
        }


        public int GetInventoryCount(string resourceTypeID)
        {
            return GetOrCreateInventoryItem(resourceTypeID).ItemCount;
        }

        private InventoryItem GetOrCreateInventoryItem(string resourceTypeID)
        {
            foreach (InventoryItem item in InventoryList)
            {
                if (item.ResourceTypeID == resourceTypeID)
                {
                    return item;
                }
            }
            var newItem = new InventoryItem
            {
                ResourceTypeID = resourceTypeID,
                ResourceType = _resourceTypeDB.GetResourceType(resourceTypeID),
                ItemCount = 0
            };
            InventoryList.Add(newItem);
            return newItem;
        }


        private InventoryItem GetOrCreateInventoryItem(ResourceTypeSO resourceType)
        {
            foreach (InventoryItem item in InventoryList)
            {
                if (item.ResourceType == resourceType)
                {
                    return item;
                }
            }
            var newItem = new InventoryItem
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
            GetOrCreateInventoryItem(resourceType).ItemCount += amount;
        }

        public void IncrementInventory(string resourceTypeID, int amount)
        {
            GetOrCreateInventoryItem(resourceTypeID).ItemCount += amount;
        }
    }



}