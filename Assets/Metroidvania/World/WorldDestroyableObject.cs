#nullable enable
using NaughtyAttributes;
using System;
using UnityEngine;
using Zenject;

namespace Metroidvania.World
{
    /// <summary>
    /// Add this to any object for it's destruction to be persistent upon reloading the scene
    /// </summary>
    public class WorldDestroyableObject : MonoBehaviour
    {
        private WorldUnlockData _worldUnlockData = null!;
        [SerializeField, ReadOnly]private string _uniqueID;
        private bool _isInitialised;

        [Inject]
        private void Initialise(WorldUnlockData worldUnlockData)
        {
            _worldUnlockData = worldUnlockData;

        }

        private void OnEnable()
        {
            if (_worldUnlockData.WorldDestroyedObjectsList.Contains(_uniqueID))
            {
                Debug.Log($"Destroying object {_uniqueID} on Enable");
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _isInitialised = true;
        }

        private void OnDestroy()
        {
            if (_isInitialised)
            {
                if (!_worldUnlockData.WorldDestroyedObjectsList.Contains(_uniqueID))
                {
                    Debug.Log($"Destroying object {_uniqueID}");
                    _worldUnlockData.WorldDestroyedObjectsList.Add(_uniqueID);
                }
                else
                {
                    Debug.Log($"Destroying WorldDestroyableObject {name} - unable to record duplicate destroy {_uniqueID}");
                }
            }
        }

#if UNITY_EDITOR
        private void Reset()
        {
            PopulateUniqueID();
        }

        private void OnValidate()
        {
            PopulateUniqueID();
        }

        private void PopulateUniqueID()
        {
            if (string.IsNullOrEmpty(_uniqueID))
            {
                _uniqueID = Guid.NewGuid().ToString();
            }
        }
#endif
    }
}