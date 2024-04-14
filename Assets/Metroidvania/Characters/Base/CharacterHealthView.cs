using Metroidvania.UI;
using Metroidvania.World;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace Metroidvania.Characters.Base
{
    public class CharacterHealthView : MonoBehaviour
    {
        public string CharacterID;
        public CharacterStatsContainer DefaultStats;
        [ShowNonSerializedField]private CharacterStats Stats = default;
        private CharacterHealthBar.Factory _healthBarFactory;

        private CharacterHealthBar? _healthBar;

        [Inject] 
        private void Initialise(
            CharacterHealthBar.Factory healthBarFactory)
        {
            _healthBarFactory = healthBarFactory;
        }

        public void StartView(CharacterStats stats)
        {
            Stats = stats;
            Stats.OnHealthChanged += OnHealthChanged;
            OnHealthChanged();
        }

        private void OnHealthChanged(float oldHealth, float newHealth, float maxHealth) => OnHealthChanged();

        private void OnHealthChanged()
        {
            if (Stats.CurrentHealth < Stats.MaxHealth)
            {
                LoadHealthBar();
            }
            else
            {
                RemoveHealthBar();
            }
        }

        private void RemoveHealthBar()
        {
            if (_healthBar != null)
            {
                Destroy(_healthBar.gameObject);
                _healthBar = null;
            }
        }

        private void LoadHealthBar()
        {
            if (_healthBar == null)
            {
                _healthBar = _healthBarFactory.Create();
                _healthBar.transform.SetParent(transform, false);
            }
            _healthBar?.SetValue(Stats.CurrentHealth, Stats.MaxHealth);
        }
    }
}