using Newtonsoft.Json;
using System;

namespace Metroidvania.World
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn), Serializable]
    public class CharacterStats
    {
        [JsonProperty]public string CharacterID;
        [JsonProperty]public float CurrentHealth;
        [JsonProperty]public float MaxHealth;

        public event Action OnDeath;
        public delegate void OnHealthChangedHandler(float oldHealth, float newHealth, float maxHealth);
        public event OnHealthChangedHandler OnHealthChanged;


        public CharacterStats(string characterId, CharacterStats defaultStats)
        {
            CharacterID = characterId;
            CurrentHealth = defaultStats.CurrentHealth;
            MaxHealth = defaultStats.MaxHealth;
        }

        public void TakeDamage(float damage)
        {
            float oldHealth = CurrentHealth;
            CurrentHealth -= damage;
            OnHealthChanged?.Invoke(oldHealth, CurrentHealth, MaxHealth);
            if (CurrentHealth <= 0)
            {
                OnDeath?.Invoke();
            }
        }

        public override string ToString()
        {
            return $"Character {CharacterID} stats: {CurrentHealth}/{MaxHealth}";
        }
    }
}