using Cysharp.Threading.Tasks;
using Metroidvania.Characters.Player.Installer;
using Metroidvania.ResourceTypes;
using System;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Metroidvania.Characters.Player.Animation
{
    public class PlayerAnimationActionsHandler
    {
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

        public event Action OnAnimationComplete;
        public event Action OnAnimationStrike;

        public PlayerAnimationActionsHandler(PlayerAnimationView playerAnimationView, ToolPrefabs toolPrefabs)
        {
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

            SetTool(PlayerAnimationTool.None);

            //foreach (var bone in _animator.avatar.humanDescription.human)
            //{
            //    if (bone.humanName == "RightHand")
            //    {
            //    }
            //}
        }

        private void SetTool(PlayerAnimationTool tool)
        {
            _pickAxeTool.SetActive(tool == PlayerAnimationTool.PickAxe);
            _axeTool.SetActive(tool == PlayerAnimationTool.Axe);
            _sickleTool.SetActive(tool == PlayerAnimationTool.Sickle);
        }

        public async UniTask RunActionAnimationAsync(InteractionActionType interactionType, CancellationToken token)
        {
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
            PlayerAnimationTool tool = GetToolForInteraction(interactionAction);
            SetTool(tool);
            await RunAnimationToComplete(token);
        }
        
        public PlayerAnimationTool GetToolForInteraction(InteractionActionType interactionAction)
        {
            switch (interactionAction)
            {
                case InteractionActionType.Pickaxe:
                    return PlayerAnimationTool.PickAxe;
                case InteractionActionType.Axe:
                    return PlayerAnimationTool.Axe;
                case InteractionActionType.Sickle:
                    return PlayerAnimationTool.Sickle;
                default:
                    return PlayerAnimationTool.None;
            }
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
            SetTool(PlayerAnimationTool.None);
        }

        public bool IsActionAnimationRunning() => _actionAnimationDetector.IsActionAnimationRunning();

        public void Reset()
        {
            _animator.SetLayerWeight(_actionLayerID, 0);
        }

        public class Factory : PlaceholderFactory<PlayerAnimationView, PlayerAnimationActionsHandler> { }
    }
}