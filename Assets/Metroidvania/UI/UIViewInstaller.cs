using Metroidvania.UI;
using Metroidvania.UI.ToolPickupPopup;
using UnityEngine;
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