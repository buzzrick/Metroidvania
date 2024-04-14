#nullable enable
using Buzzrick.UnityLibs.Lists;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Metroidvania.World
{
    [JsonObject, Serializable]
    public class WorldCharacterStatsData : BaseSaveData
    {
        [JsonProperty, SerializeField] private CharacterDictionary AllCharacters = new();

        public override string SaveName => "CharacterData";

        protected override string ToJson()
        {
            //  I'm using JsonUtility here because the AllCharacters dictionary is a custom dictionary that doesn't work with JsonConvert
            return JsonUtility.ToJson(this);
        }

        protected override void FromJson(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }

        public override void ResetData()
        {
            AllCharacters.Clear();
        }

        public CharacterStats? GetStats(string characterID)
        {
            if (AllCharacters.TryGetValue(characterID, out var characterStats))
            {
                return characterStats;
            }
            return null;
        }

        public CharacterStats GetOrCreateStats(string characterID, CharacterStats defaultStats)
        {
            if (AllCharacters.ContainsKey(characterID))
            {
                return AllCharacters.GetValue(characterID);
            }
            else
            {
                CharacterStats newCharacterStats = new CharacterStats(characterID, defaultStats)
                {
                    CharacterID = characterID
                };
                AllCharacters.AddKeyValue(characterID, newCharacterStats);
                return newCharacterStats;
            }
        }

        private void PrintList()
        {
            Debug.Log($"CharacterList {SaveName}. Count = {AllCharacters.Count}");
            foreach (var character in AllCharacters.Values)
            {
                Debug.Log($"{character}");
            }

        }


        [JsonObject, Serializable]
        private class CharacterKeyValuePair : SerializableKeyValuePair<CharacterStats> { }

        [JsonObject, Serializable]
        private class CharacterDictionary : SerializableDictionary<CharacterStats, CharacterKeyValuePair>
        {
        }
    }
}