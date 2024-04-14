using UnityEngine;
using Zenject;

namespace Metroidvania.UI
{
    public class UIFactoryInstaller : MonoInstaller<UIFactoryInstaller>
    {

        [SerializeField] private CharacterHealthBar CharacterHealthBarPrefab;

        public override void InstallBindings()
        {
            Container.BindFactory<CharacterHealthBar, CharacterHealthBar.Factory>().FromComponentInNewPrefab(CharacterHealthBarPrefab).AsSingle();
        }
    }
}