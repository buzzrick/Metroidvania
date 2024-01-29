#nullable enable

using AYellowpaper.SerializedCollections;
using Buzzrick.UnityLibs.Attributes;
using Metroidvania.ResourceTypes;
using UnityEngine;

namespace Metroidvania.World
{

    [RequireComponent(typeof(Collider))]
    public class WorldUnlockNode : WorldUnlockNodeBase
    {
        [SerializeField, RequiredField] private Collider _collider;

        [SerializedDictionary("ResourceType", "Amount Required")] public SerializedDictionary<ResourceTypeSO, int> ResourceAmounts = new();


        private void Reset()
        {
            _collider = GetComponent<Collider>();
        }

        protected override void CalculateIsUnlocked()
        {
            if (!_thisNode.IsUnlocked)
            {
                bool isPaid = true;
                //  recalculate based on paid amounts
                var amounts = GetUnlockAmounts();
                {
                    foreach (var requiredResource in amounts.RequiredAmounts)
                    {
                        if (requiredResource.Key == null)
                        {
                            Debug.LogError($"Null Resource type on WorldUnlockNode", this);
                            continue;
                        }

                        if (amounts.PaidAmounts.GetPaidAmount(requiredResource.Key.name) < requiredResource.Value)
                        {
                            isPaid = false;
                            break;
                        }
                    }
                }
                _thisNode.IsUnlocked = isPaid;
            }
            IsUnlocked = _thisNode.IsUnlocked;
        }

        public WorldUnlockData.WorldUnlockNodeAmounts GetUnlockAmounts()
        {
            return new WorldUnlockData.WorldUnlockNodeAmounts
            {
                RequiredAmounts = ResourceAmounts,
                PaidAmounts =  _thisNode,
            };
        }
    }
}
