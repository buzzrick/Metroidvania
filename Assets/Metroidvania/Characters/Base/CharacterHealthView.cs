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

        [Inject] 
        private void Initialise(WorldCharacterStatsData statsProvider)
        {
            _statsProvider = statsProvider;
        }


        private void Start()
        {
            Stats = _statsProvider.GetOrCreateStats(CharacterID, DefaultStats.Stats);
            PrintCharacterStats();
            Stats.OnDeath += Die;
            Stats.OnHealthChanged += OnHealthChanged;
        }

        private void OnHealthChanged(float oldHealth, float newHealth, float maxHealth)
        {
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