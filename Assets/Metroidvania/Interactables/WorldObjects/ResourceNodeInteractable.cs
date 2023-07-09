using Cysharp.Threading.Tasks;
using Metroidvania.ResourceTypes;
using UnityEngine;

namespace Metroidvania.Interactables.WorldObjects
{
    public class ResourceNodeInteractable : MonoBehaviour, IPlayerInteractable, IResourceNode
    {
        public ResourceTypeSO ResourceType;
        public int ResourceCount = 5;
        private int _currentResourceCount;

        private Vector3 _startingScale;

        private void Awake()
        {
            _startingScale = transform.localScale;
        }

        public async UniTask<bool> InteractAsync()
        {
            if (_currentResourceCount > 0)
            {
                transform.localScale = _startingScale * ((ResourceCount - _currentResourceCount) / ResourceCount);
                //  animate the shrink?
                return true;
            }
            return false;
        }

        public ResourceTypeSO GetResourceType() => ResourceType;
    }
}