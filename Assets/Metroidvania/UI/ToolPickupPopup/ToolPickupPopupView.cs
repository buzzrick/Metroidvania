#nullable enable
using AYellowpaper.SerializedCollections;
using Buzzrick.UnityLibs.Attributes;
using Cysharp.Threading.Tasks;
using Metroidvania.MessageBus;
using Metroidvania.MultiScene;
using Metroidvania.Player.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Metroidvania.UI.ToolPickupPopup
{
    public class ToolPickupPopupView : MonoBehaviour, IView
    {
        [SerializedDictionary("ToolType", "Sprite")] public SerializedDictionary<PlayerAnimationTool, Sprite> ToolSprites = new();

        [SerializeField, RequiredField] private Image _toolSprite = default!;
        [SerializeField, RequiredField] private TMP_Text _toolName = default!;
        [SerializeField, RequiredField] private TMP_Text _toolLevel = default!;

        public async UniTask ShowToolPickup(ToolLevel toolLevel)
        {
            if (ToolSprites.TryGetValue(toolLevel.Tool, out Sprite sprite))
            {
                _toolSprite.sprite = sprite;
            }
            _toolName.text = toolLevel.Tool.ToString();
            _toolLevel.text = toolLevel.Level.ToString();

            await UniTask.Delay(2000);
        }

        public UniTask CleanupSelf()
        {
            return UniTask.CompletedTask;
        }
    }
}