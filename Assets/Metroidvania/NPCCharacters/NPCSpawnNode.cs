using Buzzrick.UnityLibs.Attributes;
using UnityEngine;

namespace Metroidvania.Characters
{
    public class NPCSpawnNode : MonoBehaviour
    {
        [SerializeField, RequiredField] private NPCController _npcPrefab;
        private NPCController _npcController;
        public float SpawnTimeSeconds = 60f;
        public bool SpawnOnEnabled = true;
        
        private bool _isSpawned;
        private float _spawnTimer;

        private void OnEnable()
        {
            if (SpawnOnEnabled)
            {
                Spawn();
            }
        }

        private void OnDisable()
        {
            Despawn();
        }

        private void Update()
        {
            if (!_isSpawned)
            {
                _spawnTimer += Time.deltaTime;
                if (_spawnTimer > SpawnTimeSeconds)
                {
                    Spawn();
                }
            }
        }

        private void Spawn()
        {
            if (!_isSpawned)
            {
                _isSpawned = true;
                _npcController = Instantiate(_npcPrefab, transform.position, Quaternion.identity);
            }
        }
        
        private void Despawn()
        {
            if (_isSpawned)
            {
                _isSpawned = false;
                Destroy(_npcController.gameObject);
            }
        }
    }
}