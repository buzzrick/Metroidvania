using Cysharp.Threading.Tasks;
using Metroidvania.Interactables;
using Metroidvania.Interactables.WorldObjects;
using Metroidvania.Player.Animation;
using System.Threading;
using UnityEngine;

namespace Metroidvania.Player
{
    [RequireComponent(typeof(AudioSource))]
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
        private IPlayerInteractable _currentInteractable = null;
        private Color _gizmoColor = new Color(1f, 1f, 1f, 0.2f);
        private AudioSource _audioSource;
        public AudioClip SwishSound;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void RegisterPlayerAnimationHandler(PlayerAnimationActionsHandler playerAnimationActionHandler)
        {
            _playerAnimationActionHandler = playerAnimationActionHandler;
            _playerAnimationActionHandler.OnAnimationStrike += Handle_OnAnimationStrike;
            _playerAnimationActionHandler.OnAnimationComplete += Handle_OnAnimationComplete;

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
            bool interactableFound = false;
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
                    interactableFound = true;
                    _currentInteractionType = interactable.GetInteractionType();
                    //Debug.Log($"Attempting to interact with {collider.name} (Type={_currentInteractionType})");
                    if (_currentInteractionType != InteractionActionType.None)
                    {
                        if (_currentInteractionType == InteractionActionType.Interact)
                        {
                            _currentInteractable = interactable;
                            //Debug.Log($"Found {interactable} to interact with");
                        }
                        await _playerAnimationActionHandler.RunActionAnimationAsync(_currentInteractionType);
                        //  if this is a simple interact then remember the interactable so that we can trigger it later
                        return;
                    }
                }
            }
            if (isForced || interactableFound)
            {
                //  if interactableFound == true, then we don't know how to interact with it, so also play the "shrug" animation
                //Debug.Log($"No interactable found");
                await _playerAnimationActionHandler.RunActionAnimationAsync(InteractionActionType.None);
            }
        }

        /// <summary>
        /// triggered when the interaction animation hit's it's "Strike" callback. Here we generate the rewards
        /// </summary>
        private void Handle_OnAnimationStrike()
        {
            Debug.Log($"OnAnimationStrike");
            DetectRewards();
        }

        private void Handle_OnAnimationComplete()
        {
            Debug.Log($"OnAnimationComplete");
            DetectRewards();
        }


        private void DetectRewards()
        {
            if (_currentInteractionType == InteractionActionType.None)
            {
                Debug.Log($"No Interactable");
                return;
            }
            else if(_currentInteractionType == InteractionActionType.Interact)
            {
                Debug.Log($"Simple Interact with {(_currentInteractable == null ? "NULL" : _currentInteractable.ToString())}");
                if (_currentInteractable != null)
                {
                    _currentInteractable.Interact(_currentInteractionType);
                }
            }
            else
            {
                Debug.Log($"Resource Interactable");
                //  re-calculate the objects that we're facing in case we have rotated or moved elsewhere
                int colliderCount = Physics.OverlapSphereNonAlloc(transform.position, DetectionRadius, _colliders, LayerMask);
                bool correctResourceFound = false;
                //Debug.Log($"Found {colliderCount} items");
                for (int i = 0; i < colliderCount; i++)
                {
                    Collider collider = _colliders[i];
                    IResourceNode resource = collider.GetComponent<IResourceNode>();
                    if (resource.GetInteractionType() == _currentInteractionType)
                    {
                        var reward = resource.GetResource();
                        if (reward.resourceType.HarvestSound != null)
                        {
                            _audioSource.PlayOneShot(reward.resourceType.HarvestSound);
                        }
                        correctResourceFound = true;
                        Debug.Log($"Rewarding {reward.amount} {reward.resourceType} from {collider.name}   (#{i})");
                    }
                }

                //  todo: play "Swish" sound if missed, or connect (chop/clang) sound on connect etc
                if (!correctResourceFound)
                {
                    _audioSource.PlayOneShot(SwishSound);
                }
            }

            _currentInteractionType = InteractionActionType.None;
            _currentInteractable = null;
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawSphere(transform.position, DetectionRadius);
        }
    }
}