using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using Metroidvania.Player;
using Metroidvania.ResourceTypes;
using UnityEngine;
using Zenject;

namespace Metroidvania.Interactables.WorldObjects.Machine
{
    [RequireComponent(typeof(Collider))]
    public class ProductionMachine : MonoBehaviour, IPlayerEnterTriggerZone, IPlayerExitTriggerZone
    {
        [SerializedDictionary("ResourceType", "Amount Required")] public SerializedDictionary<ResourceTypeSO, int> InputAmounts = new();
        [SerializedDictionary("ResourceType", "Amount Required")] public SerializedDictionary<ResourceTypeSO, int> OutputAmounts = new();
        [SerializeField] 
        private ProductionMachineUIController _uiController = default!;

        [Inject]
        private void Initialise(ProductionMachineUIController uiController)
        {
            _uiController = uiController;
        }
        
        public void OnPlayerEnteredZone(PlayerRoot player)
        {
            _uiController.ShowUI(this, player).Forget();
        }

        public void OnPlayerExitedZone(PlayerRoot player)
        {
            _uiController?.Hide();
        }
    }
}