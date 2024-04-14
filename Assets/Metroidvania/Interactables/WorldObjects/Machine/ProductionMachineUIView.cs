#nullable enable
using AYellowpaper.SerializedCollections;
using Buzzrick.UnityLibs.Attributes;
using Cysharp.Threading.Tasks;
using Metroidvania.MultiScene;
using Metroidvania.ResourceTypes;
using Metroidvania.World;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Metroidvania.Interactables.WorldObjects.Machine
{
    public class ProductionMachineUIView : MonoBehaviour, IView
    {
        [SerializeField, RequiredField] private Canvas _canvas = default!;
        [SerializeField, RequiredField] private UnlockItemCostsDisplay[] _machineInputDisplays = default!;
        [SerializeField, RequiredField] private UnlockItemCostsDisplay[] _machineOutputDisplays = default!;
        [SerializeField, RequiredField] private Button Purchase_x1 = default!;
        [SerializeField, RequiredField] private Button Purchase_x10 = default!;
        [SerializeField, RequiredField] private Button Purchase_x100 = default!;
        
        private ProductionMachine? _displayedMachine;
        private Camera _mainCamera = default!;

        public event Action<int>? OnCreateRequested;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _canvas.worldCamera = _mainCamera;
        }

        public void CreateResources(int count)
        {
            OnCreateRequested?.Invoke(count);
        }


        public UniTask CleanupSelf()
        {
            return UniTask.CompletedTask;
        }
        
            
        private void Update()   //  Todo, make this a component of it's own (ie: BillboardCanvas)
        {
            // rotate the _canvas to face the camera
            Quaternion cameraRotation = _mainCamera.transform.rotation;
            _canvas.transform.LookAt(_canvas.transform.position + cameraRotation * Vector3.forward, cameraRotation * Vector3.up);
        }
        
        public UniTask ShowRequirements(ProductionMachine displayedMachine)
        {
            _displayedMachine = displayedMachine;
                        
            transform.position = _displayedMachine.transform.position;
            // rotate the _canvas to face the camera
            _canvas.transform.LookAt(_mainCamera.transform);
            
            ShowResources(displayedMachine.InputAmounts, _machineInputDisplays);
            ShowResources(displayedMachine.OutputAmounts, _machineOutputDisplays);
            return UniTask.CompletedTask;
        }

        private void ShowResources(SerializedDictionary<ResourceTypeSO, int> resourceAmounts, UnlockItemCostsDisplay[] itemDisplay)
        {
            //  Hide all of the item displays - we'll show the used ones
            foreach (var displayItem in itemDisplay)
            {
                displayItem.gameObject.SetActive(false);
            }

            
            int i = 0;
            int maxResources = itemDisplay.Length;
            foreach (var requiredResource in resourceAmounts)
            {
                if (resourceAmounts.Count > i)
                {
                    itemDisplay[i].gameObject.SetActive(true);
                    itemDisplay[i].SetResourceCosts(requiredResource.Key, requiredResource.Value, 0);
                }
                else
                {
                    itemDisplay[i].gameObject.SetActive(false);
                }
               
                i++;
                if (i > maxResources - 1)
                {
                    throw new NotImplementedException(
                        $"ProductionMachine UI only supports {maxResources} input resources at the moment");
                }
            }
        }

        public void UpdatePurchaseButtons(bool canAfford1, bool canAfford10, bool canAfford100)
        {
            Purchase_x1.gameObject.SetActive(canAfford1);
            Purchase_x10.gameObject.SetActive(canAfford10);
            Purchase_x100.gameObject.SetActive(canAfford100);
        }
    }
}