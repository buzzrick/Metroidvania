using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using UnityEngine;

namespace Metroidvania.Debugging
{
    public class DebuggingView : MonoBehaviour, IView, ICore
    {
        public PlayerMovementSelectionUI _playerMovementSelectionUI;

        public UniTask CleanupSelf()
        {
            return UniTask.CompletedTask;
        }

        public async UniTask StartCore()
        {
            await _playerMovementSelectionUI.StartCore();
        }
    }
}