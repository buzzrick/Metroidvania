#nullable enable
using Cysharp.Threading.Tasks;
using Metroidvania.Characters.Player;
using Metroidvania.GameCore;
using Metroidvania.MessageBus;
using Metroidvania.MultiScene;
using System;
using Zenject;

namespace Metroidvania.UI.ToolPickupPopup
{
    public class ToolPickupPopupController : ICore, IDisposable
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly PlayerMovementInputLimiter _inputLimiter;
        private MessageBusBase<ToolLevel> _toolPickupBus;

        private bool _isPopupActive;
        
        public ToolPickupPopupController(ISceneLoader sceneLoader,
            PlayerMovementInputLimiter inputLimiter,
            [Inject(Id = "ToolPickedUp")] MessageBusBase<ToolLevel> toolPickupBus)
        {
            _sceneLoader = sceneLoader;
            _inputLimiter = inputLimiter;
            _toolPickupBus = toolPickupBus;
            _toolPickupBus.OnEvent += Handle_OnToolPickupEvent;
        }
        
        public void Dispose()
        {
            _toolPickupBus.OnEvent -= Handle_OnToolPickupEvent;
        }

        private async void Handle_OnToolPickupEvent(ToolLevel toolLevel)
        {
            await UniTask.WaitWhile(() => _isPopupActive);
            _inputLimiter.RegisterLimiter(this);
            _isPopupActive = true;
            ToolPickupPopupView view = await _sceneLoader.LoadUISceneAsync<ToolPickupPopupView>("ToolPickupPopup", true);
            await view.ShowToolPickup(toolLevel);
            await _sceneLoader.UnloadSceneAsync("ToolPickupPopup", view);
            _inputLimiter.UnregisterLimiter(this);
            _isPopupActive = false;
        }

        public UniTask StartCore()
        {
            return UniTask.CompletedTask;
        }

    }
}