using Cysharp.Threading.Tasks;
using Metroidvania.Interactables;
using Metroidvania.Interactables.WorldObjects;
using Metroidvania.Player.Animation;
using Metroidvania.ResourceTypes;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Metroidvania.Player
{
    public class PlayerInteractionController : MonoBehaviour
    {
        public float DetectionRadius;
        public LayerMask LayerMask;    
        private Collider[] _colliders = new Collider[10];
        private PlayerAnimationActionsHandler _playerAnimationActionHandler;
        private bool _isAutomatic;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _onDestroyToken;
        private CancellationTokenSource _manualStopTokenSource;
        private CancellationToken _manualStopToken;
        /// <summary>
        /// The currently active interaction type
        /// </summary>
        private InteractionActionType _currentInteractionType = InteractionActionType.None;

        public void RegisterPlayerAnimationHandler(PlayerAnimationActionsHandler playerAnimationActionHandler)
        {
            _playerAnimationActionHandler = playerAnimationActionHandler;
            _playerAnimationActionHandler.OnAnimationStrike += Handle_OnAnimationStrike;
            _onDestroyToken = this.GetCancellationTokenOnDestroy();
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                AttemptInteraction(true).Forget();
            }
        }


        public void SetAutomatic(bool isAutomatic)
        {
            if (isAutomatic != _isAutomatic)
            {
                Debug.Log($"Setting Automatic interation : {isAutomatic}");
                _isAutomatic = isAutomatic;
                if (_isAutomatic)
                {
                    BuildCancellationToken();
                    DetectionLoop(_onDestroyToken).Forget();
                }
                else
                {
                    _manualStopTokenSource.Cancel();
                }
            }
        }

        private void BuildCancellationToken()
        {
            _manualStopTokenSource = new CancellationTokenSource();
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(_onDestroyToken, _manualStopTokenSource.Token);
        }

        private async UniTask DetectionLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {

                await AttemptInteraction(false);
                await UniTask.Delay(500);
                await UniTask.Yield();
            }

        }

        private async UniTask AttemptInteraction(bool isForced)
        {
            //Debug.Log($"AttemptInteraction");
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

                    //Debug.Log($"Attempting to interact with {interactable}");
                    _currentInteractionType = interactable.GetInteractionType();
                    if (_currentInteractionType != InteractionActionType.None)
                    {
                        await _playerAnimationActionHandler.RunActionAnimationAsync(_currentInteractionType);
                        //  if this is a simple interact then perform it directly 
                        //  otherwise the player animation will trigger each interact as the animation collides
                        if (_currentInteractionType == InteractionActionType.Interact)
                        {
                            bool interactionValid = interactable.Interact(_currentInteractionType);
                            if (interactionValid)
                            {
                            }
                        }
                        return;
                    }
                }
            }
            if (isForced)
            {
                Debug.Log($"No interactable found");
                await _playerAnimationActionHandler.RunActionAnimationAsync(InteractionActionType.None);
            }
        }

        /// <summary>
        /// Store the valid interactable rewards that have been observed in the interaction region
        /// </summary>
        /// <param name="interactionType"></param>
        /// <param name="colliders"></param>
        /// <param name="colliderCount"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void StoreInteractableRewards(InteractionActionType interactionType, Collider[] colliders, int colliderCount)
        {
            for (int i = 0; i < colliderCount; i++)
            {
            }
        }

        /// <summary>
        /// triggered when the interaction animation hit's it's "Strike" callback. Here we generate the rewards;
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void Handle_OnAnimationStrike()
        {
            //  re-calculate the objects that we're facing in case we have rotated or moved elsewhere
            int colliderCount = Physics.OverlapSphereNonAlloc(transform.position, DetectionRadius, _colliders, LayerMask);

            //Debug.Log($"Found {colliderCount} items");
            for (int i = 0; i < colliderCount; i++)
            {
                Collider collider = _colliders[i];
                IResourceNode resource = collider.GetComponent<IResourceNode>();
                if (resource.GetInteractionType() == _currentInteractionType)
                {
                    var reward = resource.GetResource();
                    Debug.Log($"Rewarding {reward.amount} {reward.resourceType} from {collider.name}   (#{i})");
                }
            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(transform.position, DetectionRadius);
        }
    }
}