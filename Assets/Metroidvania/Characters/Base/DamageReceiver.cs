using Metroidvania.Characters.Base;
using Metroidvania.UI;
using Metroidvania.World;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace Metroidvania.Characters
{
    [RequireComponent(typeof(Collider))]
    public class DamageReceiver : MonoBehaviour
    {
        public string CharacterID;
        public CharacterStatsContainer DefaultStats;
        private CharacterStats Stats = default;
        private WorldCharacterStatsData _statsProvider;
        [SerializeField] CharacterHealthView? _healthView;

        [Inject]
        private void Initialise(WorldCharacterStatsData statsProvider,
            CharacterHealthBar.Factory healthBarFactory)
        {
            _statsProvider = statsProvider;
        }


        private void Start()
        {
            Stats = _statsProvider.GetOrCreateStats(CharacterID, DefaultStats.Stats);
            Stats.OnDeath += Die;

            _healthView?.StartView(Stats);
        }


        public void TakeDamage(float damage)
        {
            Stats.TakeDamage(damage);
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
        private void Heal()
        {
            Stats.Heal(10);
        }

        [Button]
        private void PrintCharacterStats()
        {
            Debug.Log($"{Stats}");
        }
    }
}