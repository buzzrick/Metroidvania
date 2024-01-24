﻿using Cysharp.Threading.Tasks;
using Metroidvania.Player.Installer;
using Metroidvania.ResourceTypes;
using System;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Metroidvania.Player.Animation
{
    public class PlayerAnimationActionsHandler
    {
        private PlayerAnimationView _playerAnimationView;
        private Animator _animator;
        private int _actionLayerID;
        private ActiveAnimatorDetector _actionAnimationDetector;
        private ToolPrefabs _toolPrefabs;
        public readonly int HashActionInteract = Animator.StringToHash("ActionInteract");
        public readonly int HashActionChopDiagonal = Animator.StringToHash("ActionChopDiagonal");
        public readonly int HashActionMining = Animator.StringToHash("ActionMining");
        public readonly int HashActionUnsure = Animator.StringToHash("ActionUnsure");

        private Transform _leftHandTransform;
        private GameObject _pickAxeTool;
        private GameObject _axeTool;
        private bool _isActive = false;

        public event Action OnAnimationComplete;
        public event Action OnAnimationStrike;

        public enum Tool
        {
            None,
            PickAxe,
            Axe,
        }

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
        }

        public async UniTask RunActionAnimationAsync(InteractionActionType interactionType, CancellationToken token)
        {
            switch (interactionType)
            {
                case InteractionActionType.None:
                    _animator.SetTrigger(HashActionUnsure);
                    break;
                case InteractionActionType.MineOre:
                    _animator.SetTrigger(HashActionMining);
                    break;
                case InteractionActionType.ChopWood:
                    _animator.SetTrigger(HashActionChopDiagonal);
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
            switch (interactionAction)
            {
                case InteractionActionType.MineOre:
                    SetTool(Tool.PickAxe);
                    await RunAnimationToComplete(token);
                    break;
                case InteractionActionType.ChopWood:
                    SetTool(Tool.Axe);
                    await RunAnimationToComplete(token);
                    break;
                default:
                    SetTool(Tool.None);
                    await RunAnimationToComplete(token);
                    break;
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