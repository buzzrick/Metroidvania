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
        private WorldCharacterStatsData _statsProvider;
        private CharacterHealthBar.Factory _healthBarFactory;

        private CharacterHealthBar? _healthBar;

        [Inject] 
        private void Initialise(WorldCharacterStatsData statsProvider,
            CharacterHealthBar.Factory healthBarFactory)
        {
            _statsProvider = statsProvider;
            _healthBarFactory = healthBarFactory;
        }


        private void Start()
        {
            Stats = _statsProvider.GetOrCreateStats(CharacterID, DefaultStats.Stats);
            PrintCharacterStats();
            Stats.OnDeath += Die;
            Stats.OnHealthChanged += OnHealthChanged;
            LoadHealthBar();
        }

        private void OnHealthChanged(float oldHealth, float newHealth, float maxHealth)
        {
            _healthBar?.SetValue(Stats.CurrentHealth, Stats.MaxHealth);
        }

        private void LoadHealthBar()
        {
            _healthBar = _healthBarFactory.Create();
            _healthBar.transform.SetParent(transform, false);
            _healthBar.SetValue(Stats.CurrentHealth, Stats.MaxHealth);
        }

        private void Die()
        {
            Destroy(gameObject);
        }

        [Button]
        private void TakeDamage()
        {
            Stats.TakeDamage(10);
        }

        [Button]
        private void PrintCharacterStats()
        {
            Debug.Log($"{Stats}");
        }
    }
}