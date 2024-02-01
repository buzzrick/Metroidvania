using Cysharp.Threading.Tasks;
using Metroidvania.ResourceTypes;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;
using Zenject.ReflectionBaking.Mono.Cecil;

namespace Metroidvania.Player.Inventory
{
    [Serializable]
    public class PlayerInventoryManager
    {
        [SerializeField] private List<InventoryItemAmount> InventoryList = new();
        private ResourceTypeDB _resourceTypeDB;

        public event Action<InventoryItemAmount> OnInventoryAmountChanged;

        private static string SavePath;

        [Inject]
        private void Initialise(ResourceTypeDB resourceTypeDB)
        {
            _resourceTypeDB = resourceTypeDB;
            SavePath = Path.Combine(Application.persistentDataPath, "PlayerInventory.json");
        }

        public async UniTask LoadData()
        {
            if (File.Exists(SavePath))
            {
                Debug.Log($"Loading PlayerInventory from {SavePath}");
                JsonUtility.FromJsonOverwrite(await File.ReadAllTextAsync(SavePath), this);

                if (InventoryList == null)
                {
                    InventoryList = new();
                }

                /// wire back up the ResourceTypeSOs
                foreach (var item in InventoryList)
                {
                    item.ResourceType = _resourceTypeDB.GetResourceType(item.ResourceTypeID);
                }
            }
        }

        public async UniTask SaveData()
        {
            Debug.Log($"Writing PlayerInventory to {SavePath}");
            await File.WriteAllTextAsync(SavePath, JsonUtility.ToJson(this));
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

        /// <summary>
        /// consumes (pays) the given resource amount
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="payAmount"></param>
        /// <returns>Returns the amount paid (should be payAmount, unless the currently inventory is less)</returns>
        public int ConsumeResource(ResourceTypeSO resource, int payAmount)
        {
            //Debug.Log($"Paying {payAmount} {resource.name}");
            int availableAmount = GetInventoryCount(resource.name);
            int finalAmount = Math.Clamp(payAmount, 0, availableAmount);
            IncrementInventory(resource.name, -finalAmount);
            return finalAmount;
        }

        [Serializable]
        public class InventoryItemAmount
        {
            public string ResourceTypeID;
            [NonSerialized] public ResourceTypeSO ResourceType;
            public int ItemCount;
        }
    }



}