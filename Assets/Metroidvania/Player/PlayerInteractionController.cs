using Metroidvania.Interactables;
using Metroidvania.Interactables.WorldObjects;
using Metroidvania.Player.Animation;
using System;
using UnityEngine;

namespace Metroidvania.Player
{
    public class PlayerInteractionController : MonoBehaviour
    {
        public float DetectionRadius;
        public LayerMask LayerMask;
        private Collider[] _colliders = new Collider[10];
        private PlayerAnimationActionsHandler _playerAnimationActionHandler;

        public void RegisterPlayerAnimationHandler(PlayerAnimationActionsHandler playerAnimationActionHandler)
        {
            _playerAnimationActionHandler = playerAnimationActionHandler;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                AttemptInteraction();
            }
        }

        private async void AttemptInteraction()
        {
            int colliderCount = Physics.OverlapSphereNonAlloc(transform.position, DetectionRadius, _colliders, LayerMask);
            for (int i = 0; i < colliderCount; i++)
            {
                Collider collider = _colliders[i];
                IPlayerInteractable interactable = collider.GetComponent<IPlayerInteractable>();
                if (_playerAnimationActionHandler != null)
                {
                    //  don't allow multiple actions to play at once
                    if (_playerAnimationActionHandler.IsActionAnimationRunning())
                        return;
                }

                if (interactable != null)
                {
                    IResourceNode resourceNode = interactable as IResourceNode;
                    if (resourceNode != null)
                    {
                        _playerAnimationActionHandler.StartActionAnimation(resourceNode.GetResourceType());
                        return;
                    }

                    bool interactionValid = await interactable.InteractAsync();
                    if (interactionValid)
                    {
                        _playerAnimationActionHandler.StartActionAnimation(InteractionActionType.Interact);
                        return;
                    }
                }
            }
            _playerAnimationActionHandler.StartActionAnimation(InteractionActionType.None);
        }

    }
}