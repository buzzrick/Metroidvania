using Cysharp.Threading.Tasks;
using Metroidvania.Interactables;
using Metroidvania.Interactables.WorldObjects;
using Metroidvania.Player.Animation;
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

        public void RegisterPlayerAnimationHandler(PlayerAnimationActionsHandler playerAnimationActionHandler)
        {
            _playerAnimationActionHandler = playerAnimationActionHandler;
            _onDestroyToken = this.GetCancellationTokenOnDestroy();
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                AttemptInteraction(true);
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
            Debug.Log($"AttemptInteraction");
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

                    Debug.Log($"Attempting to interact with {interactable}");
                    bool interactionValid = await interactable.InteractAsync();
                    if (interactionValid)
                    {
                        IResourceNode resourceNode = interactable as IResourceNode;
                        if (resourceNode != null)
                        {
                            await _playerAnimationActionHandler.RunActionAnimationAsync(resourceNode.GetResourceType());
                        }
                        else
                        {
                            await _playerAnimationActionHandler.RunActionAnimationAsync(InteractionActionType.Interact);
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

    }
}