#nullable enable

using AYellowpaper.SerializedCollections;
using Buzzrick.UnityLibs.Attributes;
using Metroidvania.Interactables;
using Metroidvania.Player;
using Metroidvania.ResourceTypes;
using UnityEngine;

namespace Metroidvania.World
{

    [RequireComponent(typeof(Collider))]
    public class WorldUnlockNode : WorldUnlockNodeBase, IPlayerEnterTriggerZone, IPlayerExitTriggerZone
    {
        [SerializeField, RequiredField] private Collider _collider;
        [SerializeField, RequiredField] private MeshRenderer _meshRenderer;

        [SerializedDictionary("ResourceType", "Amount Required")] public SerializedDictionary<ResourceTypeSO, int> ResourceAmounts = new();


        private void Reset()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
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

        public override void LoadData(WorldUnlockData worldUnlockData, string zoneID)
        {
            base.LoadData(worldUnlockData, zoneID);
            _meshRenderer.enabled = !IsUnlocked;
        }

        public WorldUnlockData.WorldUnlockNodeAmounts GetUnlockAmounts()
        {
            return new WorldUnlockData.WorldUnlockNodeAmounts
            {
                RequiredAmounts = ResourceAmounts,
                PaidAmounts =  _thisNode,
            };
        }

        private void ShowUI(bool isEnabled)
        {
            Debug.Log($"{(isEnabled ? "Show" : "Hide")} WorldNode({NodeID})");
        }

        public void OnPlayerExitedZone(PlayerRoot player)
        {
            ShowUI(false);
        }


        public void OnPlayerEnteredZone(PlayerRoot player)
        {
            if (!IsUnlocked)
            {
                this._thisNode.IsUnlocked = true;
                LoadData(_worldUnlockData, _zoneID);

                ShowUI(true);
            }
        }
    }
}
