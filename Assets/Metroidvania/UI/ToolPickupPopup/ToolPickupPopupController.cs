#nullable enable
using Cysharp.Threading.Tasks;
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
        private MessageBusBase<ToolLevel> _toolPickupBus;

        private bool _isPopupActive;
        
        public ToolPickupPopupController(ISceneLoader sceneLoader,
            [Inject(Id = "ToolPickedUp")] MessageBusBase<ToolLevel> toolPickupBus)
        {
            _sceneLoader = sceneLoader;
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
            _isPopupActive = true;
            ToolPickupPopupView view = await _sceneLoader.LoadUISceneAsync<ToolPickupPopupView>("ToolPickupPopup", true);
            await view.ShowToolPickup(toolLevel);
            await _sceneLoader.UnloadSceneAsync("ToolPickupPopup", view);
            _isPopupActive = false;
        }

        public UniTask StartCore()
        {
            return UniTask.CompletedTask;
        }

    }
}