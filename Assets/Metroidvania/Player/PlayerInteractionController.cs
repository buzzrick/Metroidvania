using Cysharp.Threading.Tasks;
using Metroidvania.Interactables;
using Metroidvania.Interactables.ResourcePickups;
using Metroidvania.Interactables.WorldObjects;
using Metroidvania.Player.Animation;
using System.Threading;
using CandyCoded.HapticFeedback;
using Metroidvania.Player.Inventory;
using UnityEngine;
using Zenject;

namespace Metroidvania.Player
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerInteractionController : MonoBehaviour
    {
        public float DetectionRadius;
        public LayerMask LayerMask;    
        private Collider[] _colliders = new Collider[10];
        private PlayerAnimationActionsHandler _playerAnimationActionHandler;
        private PlayerInventoryManager _playerInventoryManager = default!;
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
        private ResourcePickupGenerator _resourceGenerator;
        private PlayerAnimationView _playerAnimationView;

        /// <summary>
        /// Used to calculate where spawned ResourcePickups are impulsed towards
        /// </summary>
        [SerializeField] private Transform _parentTransform;

        private Transform _resourcePickupParent;

        [Inject]
        private void Initialise(ResourcePickupGenerator resourceGenerator, 
            PlayerInventoryManager playerInventoryManager)
        {
            _resourceGenerator = resourceGenerator;
            _playerInventoryManager = playerInventoryManager;
            if (_parentTransform == null)
            {
                _parentTransform = transform.parent.transform;
            }

            _resourcePickupParent = new GameObject().transform;
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void RegisterPlayerAnimationHandler(
            PlayerAnimationActionsHandler playerAnimationActionHandler,
            PlayerAnimationView playerAnimationView)
        {
            _playerAnimationActionHandler = playerAnimationActionHandler;
            _playerAnimationView = playerAnimationView;
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

            //  can't do interactions if you're swimming
            if (!_playerAnimationView.IsSwimming)
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

                    if (interactable != null && interactable.IsInteractionEnabled)
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
                            
                            if (!IsInteractionUnlocked(_currentInteractionType))
                            {
                                //  nerf the interactions if we don't have the tool
                                _currentInteractionType = InteractionActionType.None;
                            }
                            
                            await _playerAnimationActionHandler.RunActionAnimationAsync(_currentInteractionType, this.GetCancellationTokenOnDestroy());
                            //  if this is a simple interact then remember the interactable so that we can trigger it later
                            return;
                        }
                    }
                }
                if (isForced || interactableFound)
                {
                    //  if interactableFound == true, then we don't know how to interact with it, so also play the "shrug" animation
                    //Debug.Log($"No interactable found");
                    await _playerAnimationActionHandler.RunActionAnimationAsync(InteractionActionType.None, this.GetCancellationTokenOnDestroy());
                }
            }
        }

        private bool IsToolUnlocked(InteractionActionType actionType)
        {
            PlayerAnimationTool tool = _playerAnimationView.GetToolForInteraction(actionType);
            return _playerInventoryManager.IsToolUnlocked(tool);  
        } 
        
        public bool IsInteractionUnlocked(InteractionActionType interactionActionType)
        {
            switch (interactionActionType)
            {
                case InteractionActionType.Pickaxe:
                case InteractionActionType.Axe:
                case InteractionActionType.Sickle:
                    return IsToolUnlocked(interactionActionType);
                    break;
            }
            return true;
        }
        
        /// <summary>
        /// triggered when the interaction animation hit's it's "Strike" callback. Here we generate the rewards
        /// </summary>
        private void Handle_OnAnimationStrike()
        {
            //Debug.Log($"OnAnimationStrike");
            DetectRewards();
        }

        private void Handle_OnAnimationComplete()
        {
            //Debug.Log($"OnAnimationComplete");
            DetectRewards();
        }


        private void DetectRewards()
        {
            if (_currentInteractionType == InteractionActionType.None)
            {
                //Debug.Log($"No Interactable");
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
                //Debug.Log($"Resource Interactable");
                //  re-calculate the objects that we're facing in case we have rotated or moved elsewhere
                int colliderCount = Physics.OverlapSphereNonAlloc(transform.position, DetectionRadius, _colliders, LayerMask);
                bool correctResourceFound = false;
                //Debug.Log($"Found {colliderCount} items");
                for (int i = 0; i < colliderCount; i++)
                {
                    Collider collider = _colliders[i];
                    IResourceNode resource = collider.GetComponent<IResourceNode>();
                    if (resource != null && resource.GetInteractionType() == _currentInteractionType)
                    {
                        var reward = resource.GetResource();
                        if (reward.resourceType.HarvestSound != null)
                        {
                            _audioSource.PlayOneShot(reward.resourceType.HarvestSound);
                        }
                        resource.Interact(_currentInteractionType); //  shrink's the resource
                        correctResourceFound = true;
                        //Debug.Log($"Generating {reward.amount} {reward.resourceType} from {collider.name}   (#{i})");
                        
                        _resourceGenerator.GeneratePickup(
                            reward.resourceType, 
                            reward.amount, 
                            _resourcePickupParent, //collider.transform,
                            transform.position,  // collider.ClosestPoint(transform.position),  // ClosestPoint requires Convex mesh collider, so just use the interaction position
                            _parentTransform.position);
                    }
                }

                //  todo: play "Swish" sound if missed, or connect (chop/clang) sound on connect etc
                if (!correctResourceFound)
                {
                    _audioSource.PlayOneShot(SwishSound);
                }
                else 
                {
                    HapticFeedback.MediumFeedback();
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