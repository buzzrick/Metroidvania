using Cysharp.Threading.Tasks;
using Metroidvania.Player.Installer;
using Metroidvania.ResourceTypes;
using System;
using UnityEngine;
using Zenject;

namespace Metroidvania.Player.Animation
{
    public class PlayerAnimationActionsHandler
    {
        private PlayerAnimationView _playerAnimationView;
        private Animator _animator;
        private int _actionLayerID;
        private ToolPrefabs _toolPrefabs;
        public readonly int HashActionInteract = Animator.StringToHash("ActionInteract");
        public readonly int HashActionChopDiagonal = Animator.StringToHash("ActionChopDiagonal");
        public readonly int HashActionMining = Animator.StringToHash("ActionMining");
        public readonly int HashActionUnsure = Animator.StringToHash("ActionUnsure");

        private float _timer = 0f;
        private Transform _leftHandTransform;
        private GameObject _pickAxeTool;
        private GameObject _axeTool;

        public enum Tool
        {
            None,
            PickAxe,
            Axe,
        }

        public PlayerAnimationActionsHandler(PlayerAnimationView playerAnimationView, ToolPrefabs toolPrefabs)
        {
            _playerAnimationView = playerAnimationView;
            _animator = _playerAnimationView.GetAnimator();
            _actionLayerID = _animator.GetLayerIndex("ActionLayer");
            _toolPrefabs = toolPrefabs;
            //Time.timeScale = 0.3f;
            BuildTools();
            Reset();
        }

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

        public void StartActionAnimation(InteractionActionType interactionType)
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

            SetToolForAnimation(interactionType);
        }

        public void StartActionAnimation(ResourceTypeSO resourceTypeSO)
        {
            if (resourceTypeSO != null)
            {
                StartActionAnimation(resourceTypeSO.InteractionAction);
            }
            else
            {
                StartActionAnimation(InteractionActionType.None);
            }
        }

        private void SetToolForAnimation(InteractionActionType interactionAction)
        {
            switch (interactionAction)
            {
                case InteractionActionType.MineOre:
                    SetTool(Tool.PickAxe);
                    RunAnimationForSeconds(1.2f);
                    break;
                case InteractionActionType.ChopWood:
                    SetTool(Tool.Axe);
                    RunAnimationForSeconds(1.2f);
                    break;
                default:
                    SetTool(Tool.None);
                    RunAnimationForSeconds(1.2f);
                    break;
            }
        }

        private async void RunAnimationForSeconds(float seconds)
        {
            _timer = seconds;
            _animator.SetLayerWeight(_actionLayerID, 1f);
            await UniTask.WaitUntil(()=>!IsActionAnimationRunning());
            _animator.SetLayerWeight(_actionLayerID, 0);
            SetTool(Tool.None);
        }

        public void Tick()
        {
            if (_timer > 0f)
            {
                _timer -= Time.deltaTime;
            }
        }

        public bool IsActionAnimationRunning()
        {
            return _timer > 0f;
        }

        public void Reset()
        {
            _animator.SetLayerWeight(_actionLayerID, 0);
        }

        public class Factory : PlaceholderFactory<PlayerAnimationView, PlayerAnimationActionsHandler> { }
    }
}