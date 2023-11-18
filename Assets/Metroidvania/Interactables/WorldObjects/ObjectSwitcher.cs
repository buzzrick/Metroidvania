using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Metroidvania.Interactables.WorldObjects
{

    [RequireComponent(typeof(Collider))]
    public class ObjectSwitcher : MonoBehaviour, IPlayerInteractable
    {
        public ObjectSwitchable[] ObjectsToSwitch;

        public async UniTask<bool> InteractAsync()
        {
            Debug.Log($"Toggling Switcher {name}");
            if (ObjectsToSwitch.Length == 0)
                return false;   

            foreach (var switcher in ObjectsToSwitch)
            {
                switcher.ToggleSwitchState();
            }

            return true;
        }
    }
}