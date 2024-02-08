using System;
using AYellowpaper.SerializedCollections;
using Buzzrick.UnityLibs.Attributes;
using Cysharp.Threading.Tasks;
using Metroidvania.MultiScene;
using Metroidvania.ResourceTypes;
using Metroidvania.World;
using UnityEngine;

namespace Metroidvania.Interactables.WorldObjects.Machine
{
    public class ProductionMachineUIView : MonoBehaviour, IView
    {
        [SerializeField, RequiredField] private Canvas _canvas = default!;
        [SerializeField, RequiredField] private UnlockItemCostsDisplay[] _machineInputDisplays = default!;
        [SerializeField, RequiredField] private UnlockItemCostsDisplay[] _machineOutputDisplays = default!;
        private ProductionMachine _displayedMachine;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
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
        
        public async UniTask ShowRequirements(ProductionMachine displayedMachine)
        {
            _displayedMachine = displayedMachine;
                        
            transform.position = _displayedMachine.transform.position;
            // rotate the _canvas to face the camera
            _canvas.transform.LookAt(_mainCamera.transform);
            
            ShowResources(displayedMachine.InputAmounts, _machineInputDisplays);
            ShowResources(displayedMachine.OutputAmounts, _machineOutputDisplays);
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
    }
}