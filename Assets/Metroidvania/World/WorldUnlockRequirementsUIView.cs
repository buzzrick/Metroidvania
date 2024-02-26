using System;
using AYellowpaper.SerializedCollections;
using Buzzrick.UnityLibs.Attributes;
using Cysharp.Threading.Tasks;
using Metroidvania.MultiScene;
using Metroidvania.Characters.Player;
using Metroidvania.Characters.Player.Inventory;
using Metroidvania.ResourceTypes;
using UnityEngine;
using Zenject;

namespace Metroidvania.World
{
    public class WorldUnlockRequirementsUIView : MonoBehaviour, IView
    {
        [SerializeField, RequiredField] private Canvas _canvas = default!;
        [SerializeField, RequiredField] private UnlockItemCostsDisplay[] _unlockItemCostsDisplays = default!;
        
        private WorldUnlockNode _displayedNode = default!;
        private WorldUnlockData.WorldUnlockNodeData _displayedNodeData = default!;
        private SerializedDictionary<ResourceTypeSO, int> _requiredResourceTotals = default!;
        private Camera _mainCamera;
        private PlayerCore _playerCore;


        [Inject]
        private void Initialise(PlayerCore playerCore)
        {
            Debug.Log($"WorldUnlockRequirements Init");
            _playerCore = playerCore;
            _mainCamera = Camera.main;
            _canvas.worldCamera = _mainCamera;
        }

        private void OnEnable()
        {
            _playerCore.GetInventoryManager().OnInventoryAmountChanged += RecalculateResourceCosts;
        }

        private void OnDisable()
        {
            _playerCore.GetInventoryManager().OnInventoryAmountChanged -= RecalculateResourceCosts;
        }

        private void RecalculateResourceCosts(PlayerInventoryManager.InventoryItemAmount obj)
        {
            if (_displayedNodeData == null)
            {
                return;
            }
            ShowRequirements(_displayedNode);
        }

        public UniTask CleanupSelf()
        {
            return UniTask.CompletedTask;
        }

        public UniTask ShowRequirements(WorldUnlockNode unlockNode)
        {
            _displayedNode = unlockNode;
            _displayedNodeData = unlockNode.NodeData;
            _requiredResourceTotals = unlockNode.ResourceAmounts;
            
            transform.position = unlockNode.transform.position;
            RotateBillboard();
            
            //  Hide all of the unlock item costs displays
            foreach (var displayItem in _unlockItemCostsDisplays)
            {
                displayItem.gameObject.SetActive(false);
            }
            
            int i = 0;
            foreach (var requiredResource in _requiredResourceTotals)
            {
                int paidAmount = _displayedNodeData.GetPaidAmount(requiredResource.Key.name);
                _unlockItemCostsDisplays[i].SetResourceCosts(requiredResource.Key, requiredResource.Value, paidAmount);
                _unlockItemCostsDisplays[i].gameObject.SetActive(true);
                
                i++;
                if (i > 3)
                {
                    throw new NotImplementedException(
                        "World Unlock Requirements UI only supports 3 resources at the moment");
                }
            }
            
            return UniTask.CompletedTask;
        }

        private void Update()
        {
            RotateBillboard();
        }

        private void RotateBillboard()
        {
            // rotate the _canvas to face the camera
            Quaternion cameraRotation = _mainCamera.transform.rotation;
            _canvas.transform.LookAt(_canvas.transform.position + cameraRotation * Vector3.forward, cameraRotation * Vector3.up);
        }
    }
}