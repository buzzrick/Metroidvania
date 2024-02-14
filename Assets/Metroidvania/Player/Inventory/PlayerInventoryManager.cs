#nullable enable
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using Metroidvania.ResourceTypes;
using System;
using System.Collections.Generic;
using System.IO;
using Metroidvania.Player.Animation;
using UnityEngine;
using Zenject;

namespace Metroidvania.Player.Inventory
{
    [Serializable]
    public class PlayerInventoryManager
    {
        [SerializeField] private List<InventoryItemAmount> InventoryList = new();
        
        //  Todo: convert to dictionary with unlock level (requires JSON.NET serialization)
        [SerializeField] private List<PlayerAnimationTool> OwnedTools = new();
        private ResourceTypeDB _resourceTypeDB = default!;

        public event Action<InventoryItemAmount>? OnInventoryAmountChanged;
        public event Action? OnInventoryReset;

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

        public bool TryConsumeResources(SerializedDictionary<ResourceTypeSO, int> amounts, int multiplier = 1)
        {
            if (!CanAffordResources(amounts, multiplier))
            {
                return false;
            }

            //  If we get here, then we've confirmed we can afford the costs.
            foreach (var item in amounts)
            {
                ConsumeResource(item.Key, item.Value * multiplier);
            }
            return true;
        }
        
        public bool CanAffordResources(SerializedDictionary<ResourceTypeSO, int> amounts, int multiplier = 1)
        {
            foreach (var item in amounts)
            {
                if (GetOrCreateInventoryItemAmount(item.Key).ItemCount < (item.Value * multiplier))
                {
                    return false;
                }
            }
            return true;
        }

        public void AddResources(SerializedDictionary<ResourceTypeSO, int> amounts, int multiplier = 1)
        {
            //  If we get here, then we've confirmed we can afford the costs.
            foreach (var item in amounts)
            {
                IncrementInventory(item.Key, item.Value * multiplier);
            }
        }

        public async UniTask ResetInventory()
        {
            InventoryList.Clear();
            OwnedTools.Clear();
            OnInventoryReset?.Invoke();
            await SaveData();
        }

        [Serializable]
        public class InventoryItemAmount
        {
            public string ResourceTypeID;
            [NonSerialized] public ResourceTypeSO ResourceType;
            public int ItemCount;
        }

        public bool IsToolUnlocked(PlayerAnimationTool toolType)
        {
            return OwnedTools.Contains(toolType);
        }
        
        public void SetToolUnlocked(PlayerAnimationTool toolType, bool isOwned)
        {
            if (IsToolUnlocked(toolType) != isOwned)
            {
                if (isOwned)
                {
                    OwnedTools.Add(toolType);
                }
                else
                {
                    OwnedTools.Remove(toolType);
                }
            }
        }
    }



}