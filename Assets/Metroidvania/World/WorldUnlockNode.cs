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
    [RequireComponent(typeof(UnlockAnimator))]
    public class WorldUnlockNode : WorldUnlockNodeBase, IPlayerEnterTriggerZone, IPlayerExitTriggerZone
    {
        [SerializeField, RequiredField] private MeshRenderer _meshRenderer = default!;
        [SerializedDictionary("ResourceType", "Amount Required")] public SerializedDictionary<ResourceTypeSO, int> ResourceAmounts = new();
        [Tooltip("How much of each resource should be paid per frame")]
        public int PaymentChunkSize = 10;

        /// <summary>
        /// Don't Animate on first load
        /// </summary>
        private bool _firstLoad;
        private int _updateTicker;
        private PlayerRoot? _player;

        private void Reset()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        public override void LoadData(WorldUnlockData worldUnlockData, string zoneID)
        {
            base.LoadData(worldUnlockData, zoneID);
            

            _meshRenderer.enabled = !IsUnlocked;
            
            _firstLoad = false;
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
            _player = null;
            ShowUI(false);
        }


        public void OnPlayerEnteredZone(PlayerRoot player)
        {
            if (!IsUnlocked)
            {
                _player = player;

                ShowUI(true);
                //UnlockNode();     //  todo : move this into a global debugging ScriptableObject

            }
        }

        private void Update()
        {
        }

        private void FixedUpdate()
        {
            if (_player != null)
            {
                _updateTicker++;
                if (_updateTicker > 10)
                {
                    PayResources(_player);
                    _updateTicker = 0;
                }
            }
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


        private void PayResources(PlayerRoot player)
        {
            if (!_thisNode.IsUnlocked)
            {
                Player.Inventory.PlayerInventoryManager playerInventory = player.PlayerInventoryManager;
                //  recalculate based on paid amounts
                var amounts = GetUnlockAmounts();

                bool isPaid = true;
                foreach (var requiredResource in amounts.RequiredAmounts)
                {
                    if (requiredResource.Key == null)
                    {
                        Debug.LogError($"Null Resource type on WorldUnlockNode", this);
                        continue;
                    }

                    int currentlyPaid = amounts.PaidAmounts.GetPaidAmount(requiredResource.Key.name);
                    int paymentRequired = requiredResource.Value;
                    int paymentRemaining = paymentRequired - currentlyPaid;
                    if (paymentRemaining > 0)
                    {
                        int payAmount = paymentRemaining;
                        if (payAmount > PaymentChunkSize)
                        {
                            payAmount = PaymentChunkSize % paymentRemaining;
                        }
                        int amountPaid = player.PlayerInventoryManager.ConsumeResource(requiredResource.Key, payAmount);
                        _thisNode.AddPaidAmount(requiredResource.Key.name, amountPaid);
                        if (amountPaid < paymentRemaining)
                        {
                            isPaid = false;
                        }
                    }
                }
                //  Only recalculate if we think we've paid the full amount
                if (isPaid)
                {
                    CalculateIsUnlocked();
                    //  LoadData will correctly unlock the node if required
                    LoadData(_worldUnlockData, _zoneID);
                }
            }
        }


        private void UnlockNode()
        {
            if (!IsUnlocked)
            {
                _thisNode.IsUnlocked = true;
                LoadData(_worldUnlockData, _zoneID);
            }
        }
    }
}
