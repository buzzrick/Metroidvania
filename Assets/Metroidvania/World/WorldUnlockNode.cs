#nullable enable

using System;
using AYellowpaper.SerializedCollections;
using CandyCoded.HapticFeedback;
using Cysharp.Threading.Tasks;
using Metroidvania.Configuration;
using Metroidvania.Interactables;
using Metroidvania.Characters.Player;
using Metroidvania.ResourceTypes;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace Metroidvania.World
{

    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(UnlockAnimator))]
    public class WorldUnlockNode : WorldUnlockNodeBase, IPlayerEnterTriggerZone, IPlayerExitTriggerZone
    {
        [SerializedDictionary("ResourceType", "Amount Required")] public SerializedDictionary<ResourceTypeSO, int> ResourceAmounts = new();
        [Tooltip("How much of each resource should be paid per frame")]
        public int PaymentChunkSize = 10;

        private int _updateTicker;
        private PlayerRoot? _player;
        private WorldUnlockRequirementsUIController _requirementsUIController = default!;
        private GameConfiguration _gameConfiguration = default!;

        [Inject]
        private void Initialise(WorldUnlockRequirementsUIController requirementsUIController, 
            GameConfiguration gameConfiguration)
        {
            _requirementsUIController = requirementsUIController;
            _gameConfiguration = gameConfiguration;
        }

        private void OnDisable()
        {
            ShowUI(false);
        }

        public WorldUnlockData.WorldUnlockNodeAmounts GetUnlockAmounts()
        {
            return new WorldUnlockData.WorldUnlockNodeAmounts
            {
                RequiredAmounts = ResourceAmounts,
                PaidAmounts =  _nodeData,
            };
        }

        private void ShowUI(bool isEnabled)
        {
            //Debug.Log($"{(isEnabled ? "Show" : "Hide")} WorldNode({NodeID})");
            if (isEnabled)
            {
                _requirementsUIController.ShowRequirements(this).Forget();
            }
            else
            {
                _requirementsUIController.Hide(this).Forget();
            }
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
            }
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
            if (!_nodeData.IsUnlocked)
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
                _nodeData.IsUnlocked = isPaid;
            }

            IsUnlocked = _nodeData.IsUnlocked;
        }


        private void PayResources(PlayerRoot player)
        {
            if (!_nodeData.IsUnlocked)
            {
                bool isPaid = true;
                Characters.Player.Inventory.PlayerInventoryManager playerInventory = player.PlayerInventoryManager;
                //  recalculate based on paid amounts
                var amounts = GetUnlockAmounts();

                bool paymentMade = false;
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
                        if (_gameConfiguration.FreeWorldUnlocks)
                        {
                            _nodeData.AddPaidAmount(requiredResource.Key.name, paymentRemaining);
                            paymentMade = true;
                        }
                        else
                        {
                            int payAmount = paymentRemaining;
                            if (payAmount > PaymentChunkSize)
                            {
                                payAmount = PaymentChunkSize % paymentRemaining;
                            }
                            int amountPaid = player.PlayerInventoryManager.ConsumeResource(requiredResource.Key, payAmount);
                            _nodeData.AddPaidAmount(requiredResource.Key.name, amountPaid);
                            if (amountPaid < paymentRemaining)
                            {
                                isPaid = false;
                            }
                            if (amountPaid > 0)
                            {
                                paymentMade = true;
                            }
                        }
                    }
                }
                
                //  Only recalculate if we think we've paid the full amount
                if (isPaid)
                {
                    CalculateIsUnlocked();
                    //  LoadData will correctly unlock the node if required
                    LoadData(_zoneID, _worldUnlockData, _parentNodeData, false);
                    ShowUI(false);
                    HapticFeedback.HeavyFeedback();
                }
                //  if we made a payment, then vibrate
                else if (paymentMade)
                {
                    HapticFeedback.LightFeedback();
                    //Handheld.Vibrate();
                }
            }
        }




#if UNITY_EDITOR

        //[HideIf("IsPrefabObject")]
        [Button(enabledMode: EButtonEnableMode.Editor)]
        private void LinkToPrefab()
        {
            WorldUnlockNode nodePrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<WorldUnlockNode>(PrefabPath);
            ConvertToPrefab(this, nodePrefab);
        }
#endif
    }
}
