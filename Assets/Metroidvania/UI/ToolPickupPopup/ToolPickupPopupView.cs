#nullable enable
using AYellowpaper.SerializedCollections;
using Buzzrick.UnityLibs.Attributes;
using Cysharp.Threading.Tasks;
using Metroidvania.Characters.Player.Animation;
using Metroidvania.MessageBus;
using Metroidvania.MultiScene;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace Metroidvania.UI.ToolPickupPopup
{
    public class ToolPickupPopupView : MonoBehaviour, IView
    {
        [SerializedDictionary("ToolType", "Sprite")] public SerializedDictionary<PlayerAnimationTool, Sprite> ToolSprites = new();

        [SerializeField, RequiredField] private Image _toolSprite = default!;
        [SerializeField, RequiredField] private TMP_Text _toolName = default!;
        [SerializeField, RequiredField] private TMP_Text _toolLevel = default!;

        [Inject(Id = "Skip")] private MessageBusVoid _skipBus = default!;

        public async UniTask ShowToolPickup(ToolLevel toolLevel)
        {
            CancellationTokenSource skipToken = new CancellationTokenSource();

            void HandleSkip()
            {
                skipToken.Cancel();
            }

            _skipBus.OnEvent += HandleSkip;

            if (ToolSprites.TryGetValue(toolLevel.Tool, out Sprite sprite))
            {
                _toolSprite.sprite = sprite;
            }
            _toolName.text = toolLevel.Tool.ToString();
            _toolLevel.text = toolLevel.Level.ToString();

            try
            {
                await UniTask.Delay(2000, cancellationToken: skipToken.Token);
            }
            catch (OperationCanceledException )
            {
                // Skipped
            }
            _skipBus.OnEvent -= HandleSkip;
        }


        public UniTask CleanupSelf()
        {
            return UniTask.CompletedTask;
        }
    }
}