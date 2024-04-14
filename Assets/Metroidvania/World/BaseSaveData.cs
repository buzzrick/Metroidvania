using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace Metroidvania.World
{
    public abstract class BaseSaveData
    {
        [JsonIgnore] public abstract string SaveName { get; }
        [JsonIgnore] private string SavePath => Path.Combine(Application.persistentDataPath, $"{SaveName}.json");

        public virtual async UniTask SaveData()
        {
            Debug.Log($"Writing WorldData to {SavePath}");
            await File.WriteAllTextAsync(SavePath, ToJson());
        }

        public virtual async UniTask LoadData()
        {
            Debug.Log($"Loading WorldData from {SavePath}");
            if (File.Exists(SavePath))
            {
                string json = await File.ReadAllTextAsync(SavePath);

                Debug.Log($"Loading JSON:{json}");
                FromJson(json);
            }
        }

        protected virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        protected virtual void FromJson(string json)
        {
            JsonConvert.PopulateObject(json, this);
        }


        public virtual void ResetData()
        {
        }
    }
}