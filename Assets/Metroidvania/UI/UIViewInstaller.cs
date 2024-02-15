using Metroidvania.UI.ToolPickupPopup;
using Zenject;

namespace Assets.Metroidvania.UI
{
    public class UIViewInstaller : MonoInstaller<UIViewInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<ToolPickupPopupController>().FromNew().AsSingle().NonLazy();
        }
    }
}