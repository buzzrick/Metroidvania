using Cysharp.Threading.Tasks;
using Metroidvania.Player.Installer;
using Metroidvania.ResourceTypes;
using System;
using System.Threading;
using Metroidvania.Player.Inventory;
using UnityEngine;
using Zenject;

namespace Metroidvania.Player.Animation
{
    public class PlayerAnimationActionsHandler
    {
        private readonly PlayerCore _playerCore;
        private PlayerInventoryManager _playerInventoryManager;
        private readonly PlayerAnimationView _playerAnimationView;
        private readonly ActiveAnimatorDetector _actionAnimationDetector;
        private readonly ToolPrefabs _toolPrefabs;
        
        private Animator _animator;
        private int _actionLayerID;
        public readonly int HashActionInteract = Animator.StringToHash("ActionInteract");
        public readonly int HashActionChopDiagonal = Animator.StringToHash("ActionChopDiagonal");
        public readonly int HashActionSickle = Animator.StringToHash("ActionSickle");
        public readonly int HashActionMining = Animator.StringToHash("ActionMining");
        public readonly int HashActionUnsure = Animator.StringToHash("ActionUnsure");

        private Transform _leftHandTransform;
        private GameObject _pickAxeTool;
        private GameObject _axeTool;
        private GameObject _sickleTool;
        private bool _isActive = false;

        public event Action OnAnimationComplete;
        public event Action OnAnimationStrike;

        public enum Tool
        {
            None,
            PickAxe,
            Axe,
            Sickle
        }

        public PlayerAnimationActionsHandler(PlayerCore playerCore, PlayerAnimationView playerAnimationView, ToolPrefabs toolPrefabs)
        {
            _playerCore = playerCore;
            _playerAnimationView = playerAnimationView;
            _playerAnimationView.OnAnimationStriked += HandleOnAnimationStriked;
            _animator = _playerAnimationView.GetAnimator();
            _actionLayerID = _animator.GetLayerIndex("ActionLayer");
            _actionAnimationDetector = new ActiveAnimatorDetector(_animator, _actionLayerID);
            _toolPrefabs = toolPrefabs;
            //Time.timeScale = 0.3f;
            BuildTools();
            Reset();
        }

        private void HandleOnAnimationStriked() => OnAnimationStrike?.Invoke();

        private void BuildTools()
        {
            _leftHandTransform = _animator.GetBoneTransform(HumanBodyBones.RightHand);
            _pickAxeTool = GameObject.Instantiate(_toolPrefabs.PickAxePrefab, _leftHandTransform);
            _axeTool = GameObject.Instantiate(_toolPrefabs.AxePrefab, _leftHandTransform);
            _sickleTool = GameObject.Instantiate(_toolPrefabs.SicklePrefab, _leftHandTransform);

            SetTool(Tool.None);

            //foreach (var bone in _animator.avatar.humanDescription.human)
            //{
            //    if (bone.humanName == "RightHand")
            //    {
            //    }
            //}
        }

        private void SetTool(Tool tool)
        {
            _pickAxeTool.SetActive(tool == Tool.PickAxe);
            _axeTool.SetActive(tool == Tool.Axe);
            _sickleTool.SetActive(tool == Tool.Sickle);
        }

        public async UniTask RunActionAnimationAsync(InteractionActionType interactionType, CancellationToken token)
        {
            if (!IsInteractionUnlocked(interactionType))
            {
                //  nerf the interactions if we don't have the tool
                interactionType = InteractionActionType.None;
            }
            
            switch (interactionType)
            {
                case InteractionActionType.None:
                    _animator.SetTrigger(HashActionUnsure);
                    break;
                case InteractionActionType.Pickaxe:
                    _animator.SetTrigger(HashActionMining);
                    break;
                case InteractionActionType.Axe:
                    _animator.SetTrigger(HashActionChopDiagonal);
                    break;
                case InteractionActionType.Sickle:
                    _animator.SetTrigger(HashActionSickle);
                    break;
                case InteractionActionType.Interact:
                    _animator.SetTrigger(HashActionInteract);
                    break;
            }


            await SetToolForAnimation(interactionType, token);
            if (token.IsCancellationRequested)
                return;
            
            //  this is redundant now?
            //await UniTask.WaitUntil(() => !IsActionAnimationRunning());
            OnAnimationComplete?.Invoke();
        }



        public async UniTask RunActionAnimationAsync(ResourceTypeSO resourceTypeSO, CancellationToken token)
        {
            if (resourceTypeSO != null)
            {
                await RunActionAnimationAsync(resourceTypeSO.InteractionAction, token);
            }
            else
            {
                await RunActionAnimationAsync(InteractionActionType.None, token);
            }
        }

        private async UniTask SetToolForAnimation(InteractionActionType interactionAction, CancellationToken token)
        {
            Tool tool = GetToolForInteraction(interactionAction);
            SetTool(tool);
            await RunAnimationToComplete(token);
        }
        
        private Tool GetToolForInteraction(InteractionActionType interactionAction)
        {
            switch (interactionAction)
            {
                case InteractionActionType.Pickaxe:
                    return Tool.PickAxe;
                case InteractionActionType.Axe:
                    return Tool.Axe;
                case InteractionActionType.Sickle:
                    return Tool.Sickle;
                default:
                    return Tool.None;
            }
        }

        private bool IsInteractionUnlocked(InteractionActionType interactionActionType)
        {
            switch (interactionActionType)
            {
                case InteractionActionType.Pickaxe:
                case InteractionActionType.Axe:
                case InteractionActionType.Sickle:
                    return IsToolUnlocked(GetToolForInteraction(interactionActionType));
                    break;
            }
            return true;
        }
        private bool IsToolUnlocked(Tool tool)
        {
            //  this is only queried when required (lazy loaded) otherwise the PlayerView may not have been loaded yet
            _playerInventoryManager ??= _playerCore.GetInventoryManager();  
            return _playerInventoryManager.IsToolUnlocked(tool);  
        } 

        private async UniTask RunAnimationToComplete(CancellationToken token)
        {
            _animator.SetLayerWeight(_actionLayerID, 1f);
            // wait for the animation to start
            await UniTask.WaitUntil(() => IsActionAnimationRunning());
            if (token.IsCancellationRequested)
                return;
            //  then wait again for it to stop
            await UniTask.WaitUntil(() => !IsActionAnimationRunning());
            if (token.IsCancellationRequested)
                return;
            _animator.SetLayerWeight(_actionLayerID, 0);
            SetTool(Tool.None);
        }

        public bool IsActionAnimationRunning() => _actionAnimationDetector.IsActionAnimationRunning();

        public void Reset()
        {
            _animator.SetLayerWeight(_actionLayerID, 0);
        }

        public class Factory : PlaceholderFactory<PlayerAnimationView, PlayerAnimationActionsHandler> { }
    }
}