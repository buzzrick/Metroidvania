using NaughtyAttributes;
using UnityEngine;


namespace Metroidvania.Configuration
{
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "Metroidvania/Game Configuration")]
    public class GameConfiguration : ScriptableObject
    {
        [SerializeField] private bool _debugOptionsEnabled;
        public bool DebugOptionsEnabled => _debugOptionsEnabled;

        [SerializeField, EnableIf("DebugOptionsEnabled")] private bool _freeWorldUnlocks;
        public bool FreeWorldUnlocks => DebugOptionsEnabled && _freeWorldUnlocks;
        
        /// <summary>
        /// Allows us to modify free world unlocks for debugging at runtime
        /// </summary>
        public bool FreeWorldUnlocksDebugging 
        { 
            get { return _freeWorldUnlocks;}
            set { _freeWorldUnlocks = value;}
        }
    }
}