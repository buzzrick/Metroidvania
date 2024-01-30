using System;
using System.Collections.Generic;
using System.IO;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using Metroidvania.ResourceTypes;
using Newtonsoft.Json;
using UnityEngine;

namespace Metroidvania.World
{
    /// <summary>
    /// Encapsulates ALL saved world data
    /// </summary>
    [JsonObject]
    public class WorldUnlockData 
    {
        public Dictionary<string, WorldDataZone> Zones = new ();
        [JsonIgnore] public string SavePath = Path.Combine(Application.persistentDataPath, "WorldData.json");
        
        public async UniTask SaveData()
        {
            Debug.Log($"Writing WorldData to {SavePath}");
            await File.WriteAllTextAsync(SavePath, ToJson());
        }

        public async UniTask LoadData()
        {
            Debug.Log($"Loading WorldData from {SavePath}");
            if (File.Exists(SavePath))
            {
                string json = await File.ReadAllTextAsync(SavePath);
                FromJson(json);
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void FromJson(string json)
        {
            JsonConvert.PopulateObject(json, this);
        }
        
        public WorldDataZone GetOrCreateZone(string zoneID)
        {
            if (Zones.TryGetValue(zoneID, out var zone))
            {
                return zone;
            }
            else
            {
                var newZone = new WorldDataZone()
                {
                    ZoneID = zoneID
                };
                Zones[zoneID] = newZone;
                return newZone;
            }
        }

        public void ResetWorldData()
        {
            Zones.Clear();
        }

        /// <summary>
        /// Lists all unlocked nodes in a single Zone
        /// </summary>
        [JsonObject]
        public class WorldDataZone
        {
            public string ZoneID;
            public bool IsUnlocked;
            
            public Dictionary<string, WorldUnlockNodeData> UnlockedNodes = new ();

            public WorldUnlockNodeData GetOrCreateNode(string nodeID)
            {
                if (UnlockedNodes.TryGetValue(nodeID, out var node))
                {
                    return node;
                }
                else
                {
                    var newNode = new WorldUnlockNodeData()
                    {
                        NodeID = nodeID
                    };
                    UnlockedNodes[nodeID] = newNode;
                    return newNode;
                }
            }
        }

        [JsonObject]
        public class WorldUnlockNodeData
        {
            public string NodeID;
            public bool IsUnlocked;
            
            public Dictionary<string, int> PaidAmounts = new ();

            public int GetPaidAmount(string resourceID)
            {
                if (PaidAmounts.TryGetValue(resourceID, out int amount))
                {
                    return amount;
                }

                return 0;
            }

            public void AddPaidAmount(string resourceID, int paidAmount)
            {
                PaidAmounts[resourceID] = GetPaidAmount(resourceID) + paidAmount;
            }
        }
        
        /// <summary>
        /// Used to show both Required unlock amounts, and the currently paid amounts
        /// </summary>
        public struct WorldUnlockNodeAmounts
        {
            public Dictionary<ResourceTypeSO, int> RequiredAmounts2;
            public SerializedDictionary<ResourceTypeSO, int> RequiredAmounts;
            public WorldUnlockNodeData PaidAmounts;
        }
    }
}