using Metroidvania.Interactables;
using System;
using UnityEngine;

namespace Metroidvania.Player
{
    public class PlayerInteractionController : MonoBehaviour
    {
        public float DetectionRadius;
        public LayerMask LayerMask;
        private Collider[] _colliders = new Collider[10];

        public event Action OnInteractionFailed;
        public event Action OnInteraction;

        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                AttemptInteraction();
            }
        }

        private async void AttemptInteraction()
        {
            OnInteraction?.Invoke();
            int colliderCount = Physics.OverlapSphereNonAlloc(transform.position, DetectionRadius, _colliders, LayerMask);
            {
                for (int i = 0; i < colliderCount; i++)
                {
                    Collider collider = _colliders[i];
                    IPlayerInteractable interactable = collider.GetComponent<IPlayerInteractable>();
                    if (interactable != null)
                    {
                        OnInteraction?.Invoke();
                        bool interactionValid = await interactable.InteractAsync();
                        if (!interactionValid)
                        {
                            OnInteractionFailed?.Invoke();
                        }
                        break;
                    }
                }
            }

        }
    }
}